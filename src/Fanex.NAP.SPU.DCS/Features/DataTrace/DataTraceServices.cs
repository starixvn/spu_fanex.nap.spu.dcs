namespace Fanex.NAP.SPU.DCS.Features.DataTrace
{
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.SPU.DCS.Features.DataTrace.Model;
    using Fanex.NAP.SPU.DCS.Features.DataTrace.Model.Dto;
    using Newtonsoft.Json;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;

    public interface IDataTraceServices
    {
        Task<int> InsertSummaryLoginTransactionAsync(int batchSize);

        Task<bool> ProcessSummaryBetTransactionAsync(DateTime? fromDate, DateTime? toDate, int batchSize);

        Task<bool> ProcessSummaryMissingBetTransactionAsync(DateTime? fromDate, DateTime? toDate, int batchSize);

        Task<bool> ProcessSummaryMissingLoginTransactionAsync(DateTime? fromDate, DateTime? toDate, int batchSize);

        Task<bool> UpdateSystemSetting(int SystemSettingID, string SystemSettingValue);

        Task<IEnumerable<ReportLoginTransactionModel>> GetReportLoginTransaction(DateTime fromDate, DateTime toDate);

        Task<IEnumerable<ReportBotDTransactionModel>> GetReportBotDTransaction(DateTime fromDate, DateTime toDate);

        Task<IEnumerable<ReportMissingTransactionModel>> GetReportMissingTransaction(DateTime fromDate, DateTime toDate);

        Task<IEnumerable<ReportLoginTransactionDetailModel>> GetReportLoginTransactionDetail(DateTime transDate);
    }

    public class DataTraceServices : IDataTraceServices
    {
        private readonly IDataTraceDataAccess dataAccess;
        private readonly ISentryClient sentryClient;

        private const int dailyTaskFPSID = 104;
        private const int delayTime = 100;
        private const int CONST_SystemSetting_maxBetTransactionDateID = 2;
        private const int CONST_SystemSetting_LastScannedLoginTransDate = 3;
        private const int CONST_SystemSetting_LastScannedBetTransDate = 4;
        private const int CONST_MissingTransaction_LoginTrans = 1;
        private const int CONST_MissingTransaction_BetTrans = 2;

        public DataTraceServices(IDataTraceDataAccess dataAccess, ISentryClient sentryClient)
        {
            this.dataAccess = dataAccess;
            this.sentryClient = sentryClient;
        }

        public async Task<int> InsertSummaryLoginTransactionAsync(int batchSize)
        {
            return await dataAccess.InsertSummaryLoginTransactionAsync(batchSize);
        }


        public async Task<bool> ProcessSummaryBetTransactionAsync(DateTime? fromDate, DateTime? toDate, int batchSize)
        {
            try
            {
                if (!await IsDailyTaskReadyAsync())
                    return false;

                if (fromDate != null && toDate != null)
                {
                    if (fromDate > toDate)
                    {
                        return true;
                    }

                    for (var date = (DateTime)fromDate; date <= toDate; date = date.AddDays(1))
                    {
                        var betTransactionInfos = await dataAccess.GetBetTransactionAsync(date);
                        if (betTransactionInfos != null && betTransactionInfos.Any())
                        {
                            _ = await dataAccess.DeleteBetTransactionSummaryByDateAsync(date);
                            _ = await InsertSummaryBetTransactionByBatchAsync(TransformBetTransactionInfo(betTransactionInfos), batchSize);
                        }
                    }
                }
                else
                {
                    var maxBetTicketDateStr = await dataAccess.GetSystemSettingParameterByID(CONST_SystemSetting_maxBetTransactionDateID);
                    DateTime maxBetTicketDate = DateTime.ParseExact(maxBetTicketDateStr, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    DateTime yesterday = DateTime.Now.Date.AddDays(-1);

                    maxBetTicketDate = maxBetTicketDate.AddDays(1);
                    if (maxBetTicketDate > yesterday)
                    {
                        return true;
                    }

                    for (var date = maxBetTicketDate; date <= yesterday; date = date.AddDays(1))
                    {
                        var betTransactionInfos = await dataAccess.GetBetTransactionAsync(date);
                        if (betTransactionInfos != null && betTransactionInfos.Any())
                        {
                            _ = await dataAccess.DeleteBetTransactionSummaryByDateAsync(date);
                            var isSuccessful = await InsertSummaryBetTransactionByBatchAsync(TransformBetTransactionInfo(betTransactionInfos), batchSize);

                            if (isSuccessful)
                            {
                                await dataAccess.UpdateSystemSettingParameterByID(CONST_SystemSetting_maxBetTransactionDateID, date.ToString("yyyy-MM-dd"));
                            }
                            else
                            {
                                LogService.Error($"DCS - InsertSummaryBetTransactionByBatchAsync is not success!");
                            }
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                HandleException(ex, "DCS - DataTrace summary bet tickets job execute exeption message: ");
            }

            return false;
        }

        public async Task<bool> ProcessSummaryMissingBetTransactionAsync(DateTime? fromDate, DateTime? toDate, int batchSize)
        {
            var isUpdateSystemSetting = true;
            if (fromDate != null && toDate != null)
            {
                isUpdateSystemSetting = false;
            }
            else
            {
                var lastScannedLoginTransDateStr = await dataAccess.GetSystemSettingParameterByID(CONST_SystemSetting_LastScannedBetTransDate);
                DateTime lastScannedLoginTransDate = DateTime.ParseExact(lastScannedLoginTransDateStr, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                fromDate = lastScannedLoginTransDate.AddDays(1);

                var maxBetTicketDateStr = await dataAccess.GetSystemSettingParameterByID(CONST_SystemSetting_maxBetTransactionDateID);
                DateTime maxBetTicketDate = DateTime.ParseExact(maxBetTicketDateStr, "yyyy-MM-dd", CultureInfo.InvariantCulture);

                if (toDate == null || toDate > maxBetTicketDate)
                {
                    toDate = maxBetTicketDate;
                }
            }

            if (fromDate > toDate)
            {
                return true;
            }

            for (var transDate = (DateTime)fromDate; transDate <= toDate; transDate = transDate.AddDays(1))
            {
                var missingBetTransactionIDPackages = await dataAccess.GetMissingBetTransactionPackageAsync(transDate, batchSize);
                if (missingBetTransactionIDPackages != null && missingBetTransactionIDPackages.Any())
                {
                    var tasks = new List<Task>();

                    await dataAccess.DeleteMissingTransactionByDate(transDate, CONST_MissingTransaction_BetTrans);

                    foreach (var package in missingBetTransactionIDPackages)
                    {
                        var task = Task.Run(async () =>
                        {
                            try
                            {
                                await dataAccess.InsertMissingBetTransactionAsync(transDate, package.MinID, package.MaxID);
                            }
                            catch (Exception ex)
                            {
                                sentryClient.CaptureException(ex);
                                throw;
                            }
                        });

                        await Task.Delay(delayTime);

                        tasks.Add(task);
                    }

                    await Task.WhenAll(tasks);

                    if (isUpdateSystemSetting)
                    {
                        await dataAccess.UpdateSystemSettingParameterByID(CONST_SystemSetting_LastScannedBetTransDate, transDate.ToString("yyyy-MM-dd"));
                    }
                }
                else
                {
                    LogService.Error($"DCS - GetMissingBetTransactionPackageAsync does not return data for date: {transDate:yyyy-MM-dd}");
                }
            }

            return true;
        }

        public async Task<bool> ProcessSummaryMissingLoginTransactionAsync(DateTime? fromDate, DateTime? toDate, int batchSize)
        {
            var isUpdateSystemSetting = true;
            if (fromDate != null && toDate != null)
            {
                isUpdateSystemSetting = false;
            }
            else
            {
                var lastScannedLoginTransDateStr = await dataAccess.GetSystemSettingParameterByID(CONST_SystemSetting_LastScannedLoginTransDate);
                DateTime lastScannedLoginTransDate = DateTime.ParseExact(lastScannedLoginTransDateStr, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                fromDate = lastScannedLoginTransDate.AddDays(1);

                var maxBetTicketDateStr = await dataAccess.GetSystemSettingParameterByID(CONST_SystemSetting_maxBetTransactionDateID);
                DateTime maxBetTicketDate = DateTime.ParseExact(maxBetTicketDateStr, "yyyy-MM-dd", CultureInfo.InvariantCulture);

                if (toDate == null || toDate > maxBetTicketDate)
                {
                    toDate = maxBetTicketDate;
                }
            }

            if (fromDate > toDate)
            {
                return true;
            }

            for (var transDate = (DateTime)fromDate; transDate <= toDate; transDate = transDate.AddDays(1))
            {
                var missingLoginIDPackages = await dataAccess.GetMissingLoginTranPackageAsync(transDate, batchSize);
                if (missingLoginIDPackages != null && missingLoginIDPackages.Any())
                {
                    var tasks = new List<Task>();

                    await dataAccess.DeleteMissingTransactionByDate(transDate, CONST_MissingTransaction_LoginTrans);

                    foreach (var package in missingLoginIDPackages)
                    {
                        var task = Task.Run(async () =>
                        {
                            try
                            {
                                await dataAccess.InsertMissingLoginTransactionAsync(transDate, package.MinID, package.MaxID);
                            }
                            catch (Exception ex)
                            {
                                sentryClient.CaptureException(ex);
                                throw;
                            }
                        });

                        await Task.Delay(delayTime);

                        tasks.Add(task);
                    }

                    await Task.WhenAll(tasks);

                    if (isUpdateSystemSetting)
                    {
                        await dataAccess.UpdateSystemSettingParameterByID(CONST_SystemSetting_LastScannedLoginTransDate, transDate.ToString("yyyy-MM-dd"));
                    }
                }
                else
                {
                    LogService.Error($"DCS - GetMissingLoginTranPackageAsync does not return data for date: {transDate:yyyy-MM-dd}");
                }
            }


            if (fromDate != null && toDate != null)
            {
                if (fromDate > toDate)
                {
                    return true;
                }

                for (var transDate = (DateTime)fromDate; transDate <= toDate; transDate = transDate.AddDays(1))
                {
                    var missingLoginIDPackages = await dataAccess.GetMissingLoginTranPackageAsync(transDate, batchSize);
                    if (missingLoginIDPackages != null && missingLoginIDPackages.Any())
                    {
                        var tasks = new List<Task>();

                        await dataAccess.DeleteMissingTransactionByDate(transDate, CONST_MissingTransaction_LoginTrans);

                        foreach (var package in missingLoginIDPackages)
                        {
                            var task = Task.Run(async () =>
                            {
                                try
                                {
                                    await dataAccess.InsertMissingLoginTransactionAsync(transDate, package.MinID, package.MaxID);
                                }
                                catch (Exception ex)
                                {
                                    sentryClient.CaptureException(ex);
                                    throw;
                                }
                            });

                            await Task.Delay(delayTime);

                            tasks.Add(task);
                        }

                        await Task.WhenAll(tasks);
                    }
                    else
                    {
                        LogService.Error($"DCS - GetMissingLoginTranPackageAsync does not return data for date: {transDate:yyyy-MM-dd}");
                    }
                }
            }
            else
            {
                var lastScannedLoginTransDateStr = await dataAccess.GetSystemSettingParameterByID(CONST_SystemSetting_LastScannedLoginTransDate);
                DateTime lastScannedLoginTransDate = DateTime.ParseExact(lastScannedLoginTransDateStr, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                lastScannedLoginTransDate = lastScannedLoginTransDate.AddDays(1);

                var maxBetTicketDateStr = await dataAccess.GetSystemSettingParameterByID(CONST_SystemSetting_maxBetTransactionDateID);
                DateTime maxBetTicketDate = DateTime.ParseExact(maxBetTicketDateStr, "yyyy-MM-dd", CultureInfo.InvariantCulture);

                if (toDate != null && toDate < maxBetTicketDate)
                {
                    maxBetTicketDate = (DateTime)toDate;
                }

                if (lastScannedLoginTransDate >= maxBetTicketDate)
                {
                    return true;
                }

                for (var transDate = lastScannedLoginTransDate; transDate <= maxBetTicketDate; transDate = transDate.AddDays(1))
                {
                    var missingLoginIDPackages = await dataAccess.GetMissingLoginTranPackageAsync(transDate, batchSize);
                    if (missingLoginIDPackages != null && missingLoginIDPackages.Any())
                    {
                        var tasks = new List<Task>();

                        await dataAccess.DeleteMissingTransactionByDate(transDate, CONST_MissingTransaction_LoginTrans);

                        foreach (var package in missingLoginIDPackages)
                        {
                            var task = Task.Run(async () =>
                            {
                                try
                                {
                                    await dataAccess.InsertMissingLoginTransactionAsync(transDate, package.MinID, package.MaxID);
                                }
                                catch (Exception ex)
                                {
                                    sentryClient.CaptureException(ex);
                                    throw;
                                }
                            });

                            await Task.Delay(delayTime);

                            tasks.Add(task);
                        }

                        await Task.WhenAll(tasks);
                    }
                    else
                    {
                        LogService.Error($"DCS - GetMissingLoginTranPackageAsync does not return data for date: {transDate:yyyy-MM-dd}");
                    }

                    await dataAccess.UpdateSystemSettingParameterByID(CONST_SystemSetting_LastScannedLoginTransDate, transDate.ToString("yyyy-MM-dd"));
                }
            }

            return true;
        }

        public async Task<bool> UpdateSystemSetting(int SystemSettingID, string SystemSettingValue)
        {
            try
            {
                await dataAccess.UpdateSystemSettingParameterByID(SystemSettingID, SystemSettingValue);

                return true;
            }
            catch (Exception ex)
            {
                HandleException(ex, "DCS - DataTrace UpdateSystemSetting job execute exeption message: ");
            }

            return false;
        }

        public async Task<bool> DeleteMissingTransactionByDate(DateTime transDate, int missingType)
        {
            try
            {
                await dataAccess.DeleteMissingTransactionByDate(transDate, missingType);

                return true;
            }
            catch (Exception ex)
            {
                HandleException(ex, "DCS - DataTrace DeleteMissingTransactionByDate job execute exeption message: ");
            }

            return false;
        }

        public async Task<IEnumerable<ReportLoginTransactionModel>> GetReportLoginTransaction(DateTime fromDate, DateTime toDate)
        {
            return await dataAccess.GetReportLoginTransaction(fromDate, toDate);
        }

        public async Task<IEnumerable<ReportBotDTransactionModel>> GetReportBotDTransaction(DateTime fromDate, DateTime toDate)
        {
            return await dataAccess.GetReportBotDTransaction(fromDate, toDate);
        }

        public async Task<IEnumerable<ReportLoginTransactionDetailModel>> GetReportLoginTransactionDetail(DateTime transDate)
        {
            return await dataAccess.GetReportLoginTransactionDetail(transDate);
        }

        public async Task<IEnumerable<ReportMissingTransactionModel>> GetReportMissingTransaction(DateTime fromDate, DateTime toDate)
        {
            return await dataAccess.GetReportMissingTransaction(fromDate, toDate);
        }


        private IEnumerable<BetTransactionInfoDto> TransformBetTransactionInfo(IEnumerable<BetTransactionInfo> model)
        {
            return model.OrderBy(x => x.CustID).Select(x => new BetTransactionInfoDto
            {
                TransDate = x.TransDate,
                CustID = x.CustID,
                BetCount = x.BetCount,
                FirstBetTransactionTime = x.FirstBetTicketTime,
                LastBetTransactionTime = x.LastBetTicketTime
            });
        }

        private async Task<bool> InsertSummaryBetTransactionByBatchAsync(IEnumerable<BetTransactionInfoDto> betTransactionInfos, int batchSize)
        {
            var tasks = new List<Task>();

            for (int startIndex = 0; startIndex < betTransactionInfos.Count(); startIndex += batchSize)
            {
                var batch = betTransactionInfos.Skip(startIndex).Take(batchSize);
                var rawTransJson = JsonConvert.SerializeObject(batch);

                var task = Task.Run(async () =>
                {
                    try
                    {
                        await dataAccess.InsertSummaryBetTransactionAsync(rawTransJson);
                    }
                    catch (Exception ex)
                    {
                        sentryClient.CaptureException(ex);
                        throw;
                    }
                });

                await Task.Delay(delayTime);

                tasks.Add(task);
            }

            await Task.WhenAll(tasks);

            return true;
        }

        private async Task<bool> IsDailyTaskReadyAsync()
        {
            return await dataAccess.IsDailyTaskReadyAsync(dailyTaskFPSID);
        }

        private void HandleException(Exception ex, string extraLogMessage)
        {
            sentryClient.CaptureException(ex);
            string logMessage = new LogMessage(ex.Message)
                .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

            LogService.Error(extraLogMessage + logMessage);
        }
    }
}
