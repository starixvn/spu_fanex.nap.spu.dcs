namespace Fanex.NAP.SPU.DCS.ServiceAPI.Services
{
    using Fanex.Logging.Extensions.Database;
    using Fanex.Logging.Extensions;
    using Fanex.Logging;
    using Fanex.NAP.SPU.DCS.Features.DataTrace;
    using Fanex.NAP.SPU.DCS.Features.DataTrace.Model;
    using Fanex.NAP.SPU.DCS.Features.DataTrace.Model.Dto;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text.Json;
    using System.Threading.Tasks;

    public interface IDataTraceServices
    {
        Task<bool> ProcessSummaryBetTransactionAsync(DateTime? fromDate, DateTime? toDate, int batchSize);

        Task<bool> ProcessSummaryMissingBetTransactionAsync(DateTime? fromDate, DateTime? toDate, int batchSize);

        Task<bool> ProcessSummaryMissingLoginTransactionAsync(DateTime? fromDate, DateTime? toDate, int batchSize);

        Task<bool> UpdateSystemSetting(int SystemSettingID, string SystemSettingValue);

        Task<int> CountDWBetTransactionByDate(DateTime transDate);

        Task<int> CookDataLoginTransaction(int batchSize);

        Task<IEnumerable<ReportTransactionLogstashModel>> GetTransactionLogstash(string subscriberName, string accountName, DateTime fromDate, DateTime toDate, int batchSize);

        Task<IEnumerable<ReportMissingTransactionModel>> GetReportMissingTransaction(DateTime fromDate, DateTime toDate);

        Task<IEnumerable<ReportLoginTransactionBySubscriberModel>> GetReportLoginTransactionBySubscriber(DateTime fromDate, DateTime toDate);

        Task<IEnumerable<ReportLoginTransactionBySiteModel>> GetReportLoginTransactionBySite(DateTime fromDate, DateTime toDate);

        Task<IEnumerable<ReportLoginTransactionByAccountModel>> GetReportLoginTransactionByAccount(DateTime fromDate, DateTime toDate);

        Task<IEnumerable<ReportLoginTransactionByBrowserModel>> GetReportLoginTransactionByBrowser(DateTime fromDate, DateTime toDate);

        Task<IEnumerable<ReportLoginTransactionByFlaggedModel>> GetReportLoginTransactionByFlagged(DateTime fromDate, DateTime toDate);
    }

    public class DataTraceServices : IDataTraceServices
    {
        private readonly IDataTraceDataAccess dataAccess;
        private readonly ILogService logService;

        private const int dailyTaskFPSID = 104;
        private const int delayTime = 100;
        private const string formatDatetime = "yyyy-MM-dd";

        private const int CONST_SystemSetting_maxBetTransactionDateID = 2;
        private const int CONST_SystemSetting_LastScannedLoginTransDate = 3;
        private const int CONST_SystemSetting_LastScannedBetTransDate = 4;
        private const int CONST_MissingTransaction_LoginTrans = 1;
        private const int CONST_MissingTransaction_BetTrans = 2;

        public DataTraceServices(IDataTraceDataAccess dataAccess, ILogService logService)
        {
            this.dataAccess = dataAccess;
            this.logService = logService;
        }

        public async Task<bool> ProcessSummaryBetTransactionAsync(DateTime? fromDate, DateTime? toDate, int batchSize)
        {
            try
            {
                if (!await IsDailyTaskReadyAsync())
                {
                    return false;
                }

                if (fromDate != null && toDate != null)
                {
                    if (fromDate > toDate)
                    {
                        return true;
                    }

                    for (var date = (DateTime)fromDate; date <= toDate; date = date.AddDays(1))
                    {
                        var betTransactions = await dataAccess.GetBetTransactionAsync(date);
                        if (betTransactions != null && betTransactions.Any())
                        {
                            _ = await dataAccess.DeleteBetTransactionSummaryByDateAsync(date);
                            await InsertSummaryBetTransactionByBatchAsync(TransformBetTransactionInfo(betTransactions), batchSize);
                        }
                    }
                }
                else
                {
                    return await ProcessInsertSummaryBetTransaction(batchSize);
                }

                return true;
            }
            catch (Exception ex)
            {
                HandleException(ex, "Error in ProcessSummaryBetTransactionAsync: ");
                return false;
            }
        }

        private async Task<bool> ProcessInsertSummaryBetTransaction(int batchSize)
        {
            try
            {
                var maxBetTicketDateStr = await dataAccess.GetSystemSettingParameterByID(CONST_SystemSetting_maxBetTransactionDateID);
                DateTime maxBetTicketDate = ParseExactDate(maxBetTicketDateStr);
                DateTime yesterday = DateTimeOffset.Now.Date.AddDays(-1);

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

                        await InsertSummaryBetTransactionByBatchAsync(TransformBetTransactionInfo(betTransactionInfos), batchSize);
                        await dataAccess.UpdateSystemSettingParameterByID(CONST_SystemSetting_maxBetTransactionDateID, date.ToString(formatDatetime));
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                HandleException(ex, "Error in ProcessInsertSummaryBetTransaction: ");
                return false;
            }
        }

        public async Task<bool> ProcessSummaryMissingBetTransactionAsync(DateTime? fromDate, DateTime? toDate, int batchSize)
        {
            try
            {
                var isUpdateSystemSetting = true;
                if (fromDate != null && toDate != null)
                {
                    isUpdateSystemSetting = false;
                }
                else
                {
                    var lastScannedLoginTransDateStr = await dataAccess.GetSystemSettingParameterByID(CONST_SystemSetting_LastScannedBetTransDate);
                    DateTime lastScannedLoginTransDate = ParseExactDate(lastScannedLoginTransDateStr);
                    fromDate = lastScannedLoginTransDate.AddDays(1);

                    var maxBetTicketDateStr = await dataAccess.GetSystemSettingParameterByID(CONST_SystemSetting_maxBetTransactionDateID);
                    DateTime maxBetTicketDate = ParseExactDate(maxBetTicketDateStr);

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
                    var listAccountIDByBatch = await dataAccess.GetMissingBetTransactionPackageAsync(transDate, batchSize);
                    if (listAccountIDByBatch != null && listAccountIDByBatch.Any())
                    {
                        await dataAccess.DeleteMissingTransactionByDate(transDate, CONST_MissingTransaction_BetTrans);
                        await ProcessInsertMissingBetTransactionAsync(transDate, listAccountIDByBatch);

                        if (isUpdateSystemSetting)
                        {
                            await dataAccess.UpdateSystemSettingParameterByID(CONST_SystemSetting_LastScannedBetTransDate, transDate.ToString(formatDatetime));
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                HandleException(ex, "Error in ProcessSummaryMissingBetTransactionAsync: ");
                return false;
            }
        }

        private async Task ProcessInsertMissingBetTransactionAsync(DateTime transDate, IEnumerable<string> listAccountIDByBatch)
        {
            var tasks = new List<Task>();

            foreach (var listAccountID in listAccountIDByBatch)
            {
                var task = Task.Run(async () =>
                {
                    try
                    {
                        await dataAccess.InsertMissingBetTransactionAsync(transDate, listAccountID);
                    }
                    catch (Exception ex)
                    {
                        HandleException(ex, "Error in ProcessInsertMissingBetTransactionAsync: ");
                        throw;
                    }
                });

                await Task.Delay(delayTime);

                tasks.Add(task);
            }

            await Task.WhenAll(tasks);
        }

        public async Task<bool> ProcessSummaryMissingLoginTransactionAsync(DateTime? fromDate, DateTime? toDate, int batchSize)
        {
            try
            {
                var isUpdateSystemSetting = true;
                if (fromDate != null && toDate != null)
                {
                    isUpdateSystemSetting = false;
                }
                else
                {
                    var lastScannedLoginTransDateStr = await dataAccess.GetSystemSettingParameterByID(CONST_SystemSetting_LastScannedLoginTransDate);
                    DateTime lastScannedLoginTransDate = ParseExactDate(lastScannedLoginTransDateStr);
                    fromDate = lastScannedLoginTransDate.AddDays(1);

                    var maxBetTicketDateStr = await dataAccess.GetSystemSettingParameterByID(CONST_SystemSetting_maxBetTransactionDateID);
                    DateTime maxBetTicketDate = ParseExactDate(maxBetTicketDateStr);

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
                        await dataAccess.DeleteMissingTransactionByDate(transDate, CONST_MissingTransaction_LoginTrans);
                        await ProcessInsertMissingLoginTransactionAsync(isUpdateSystemSetting, transDate, missingLoginIDPackages);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                HandleException(ex, "Error in ProcessSummaryMissingLoginTransactionAsync: ");
                return false;
            }
        }

        private async Task ProcessInsertMissingLoginTransactionAsync(bool isUpdateSystemSetting, DateTime transDate, IEnumerable<RangeIDPackage> missingLoginIDPackages)
        {
            var tasks = new List<Task>();

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
                        HandleException(ex, "Error in ProcessInsertMissingLoginTransactionAsync: ");
                        throw;
                    }
                });

                await Task.Delay(delayTime);

                tasks.Add(task);
            }

            await Task.WhenAll(tasks);

            if (isUpdateSystemSetting)
            {
                await dataAccess.UpdateSystemSettingParameterByID(CONST_SystemSetting_LastScannedLoginTransDate, transDate.ToString(formatDatetime));
            }
        }

        private static DateTime ParseExactDate(string dateTimeString)
        {
            return DateTime.ParseExact(dateTimeString, formatDatetime, CultureInfo.InvariantCulture);
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
                HandleException(ex, "Error in UpdateSystemSetting: ");
                return false;
            }
        }

        public async Task<int> CountDWBetTransactionByDate(DateTime transDate)
        {
            try
            {
                if (!await IsDailyTaskReadyAsync())
                {
                    return 0;
                }

                var data = await dataAccess.GetBetTransactionAsync(transDate);
                return data != null && data.Any() ? data.Count() : 0;
            }
            catch (Exception ex)
            {
                HandleException(ex, "Error in CountDWBetTransactionByDate: ");
                return 0;
            }
        }

        private static IEnumerable<BetTransactionInfoDto> TransformBetTransactionInfo(IEnumerable<BetTransactionInfo> model)
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

        private async Task InsertSummaryBetTransactionByBatchAsync(IEnumerable<BetTransactionInfoDto> betTransactionInfos, int batchSize)
        {
            var tasks = new List<Task>();

            for (int startIndex = 0; startIndex < betTransactionInfos.Count(); startIndex += batchSize)
            {
                var batch = betTransactionInfos.Skip(startIndex).Take(batchSize);
                var rawTransJson = JsonSerializer.Serialize(batch);

                var task = Task.Run(async () =>
                {
                    try
                    {
                        await dataAccess.InsertSummaryBetTransactionAsync(rawTransJson);
                    }
                    catch (Exception ex)
                    {
                        HandleException(ex, "Error in InsertSummaryBetTransactionByBatchAsync: ");
                        throw;
                    }
                });

                await Task.Delay(delayTime);

                tasks.Add(task);
            }

            await Task.WhenAll(tasks);
        }

        private async Task<bool> IsDailyTaskReadyAsync()
        {
            try
            {
                return await dataAccess.IsDailyTaskReadyAsync(dailyTaskFPSID);
            }
            catch (Exception ex)
            {
                HandleException(ex, "Error in IsDailyTaskReadyAsync: ");
                return false;
            }
        }

        public async Task<int> CookDataLoginTransaction(int batchSize)
        {
            try
            {
                return await dataAccess.CookDataLoginTransaction(batchSize);
            }
            catch (Exception ex)
            {
                HandleException(ex, "Error in CookDataLoginTransaction: ");
                return 0;
            }
        }

        #region Report

        public async Task<IEnumerable<ReportMissingTransactionModel>> GetReportMissingTransaction(DateTime fromDate, DateTime toDate)
        {
            try
            {
                return await dataAccess.GetReportMissingTransaction(fromDate, toDate);
            }
            catch (Exception ex)
            {
                HandleException(ex, "Error in GetReportMissingTransaction: ");
                return Enumerable.Empty<ReportMissingTransactionModel>();
            }
        }

        public async Task<IEnumerable<ReportLoginTransactionBySubscriberModel>> GetReportLoginTransactionBySubscriber(DateTime fromDate, DateTime toDate)
        {
            try
            {
                return await dataAccess.GetReportLoginTransactionBySubscriber(fromDate, toDate);
            }
            catch (Exception ex)
            {
                HandleException(ex, "Error in GetReportLoginTransactionBySubscriber: ");
                return Enumerable.Empty<ReportLoginTransactionBySubscriberModel>();
            }
        }

        public async Task<IEnumerable<ReportLoginTransactionBySiteModel>> GetReportLoginTransactionBySite(DateTime fromDate, DateTime toDate)
        {
            try
            {
                return await dataAccess.GetReportLoginTransactionBySite(fromDate, toDate);
            }
            catch (Exception ex)
            {
                HandleException(ex, "Error in GetReportLoginTransactionBySite: ");
                return Enumerable.Empty<ReportLoginTransactionBySiteModel>();
            }
        }

        public async Task<IEnumerable<ReportLoginTransactionByAccountModel>> GetReportLoginTransactionByAccount(DateTime fromDate, DateTime toDate)
        {
            try
            {
                return await dataAccess.GetReportLoginTransactionByAccount(fromDate, toDate);
            }
            catch (Exception ex)
            {
                HandleException(ex, "Error in GetReportLoginTransactionByAccount: ");
                return Enumerable.Empty<ReportLoginTransactionByAccountModel>();
            }
        }

        public async Task<IEnumerable<ReportLoginTransactionByBrowserModel>> GetReportLoginTransactionByBrowser(DateTime fromDate, DateTime toDate)
        {
            try
            {
                return await dataAccess.GetReportLoginTransactionByBrowser(fromDate, toDate);
            }
            catch (Exception ex)
            {
                HandleException(ex, "Error in GetReportLoginTransactionByBrowser: ");
                return Enumerable.Empty<ReportLoginTransactionByBrowserModel>();
            }
        }

        public async Task<IEnumerable<ReportLoginTransactionByFlaggedModel>> GetReportLoginTransactionByFlagged(DateTime fromDate, DateTime toDate)
        {
            try
            {
                return await dataAccess.GetReportLoginTransactionByFlagged(fromDate, toDate);
            }
            catch (Exception ex)
            {
                HandleException(ex, "Error in GetReportLoginTransactionByFlagged: ");
                return Enumerable.Empty<ReportLoginTransactionByFlaggedModel>();
            }
        }

        public async Task<IEnumerable<ReportTransactionLogstashModel>> GetTransactionLogstash(string subscriberName, string accountName, DateTime fromDate, DateTime toDate, int batchSize)
        {
            try
            {
                return await dataAccess.GetTranPackageAsync(subscriberName, accountName, fromDate, toDate, batchSize);
            }
            catch (Exception ex)
            {
                HandleException(ex, "Error in GetTransactionLogstash: ");
                return Enumerable.Empty<ReportTransactionLogstashModel>();
            }
        }
        #endregion

        private void HandleException(Exception ex, string extraLogMessage)
        {
            string logMessage = new LogMessage(ex.Message)
                .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

            logService.ErrorMessageAsync(extraLogMessage + logMessage);
        }
    }
}
