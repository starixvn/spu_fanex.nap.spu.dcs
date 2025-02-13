namespace Fanex.NAP.SPU.DCS.Features.DataTrace
{
    using Fanex.NAP.SPU.DCS.Features.DataTrace.Model;
    using Fanex.NAP.SPU.DCS.Features.DataTrace.Model.Dto;
    using Fanex.NAP.SPU.DCS.JobService.Services;
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

        private async Task<bool> ProcessInsertSummaryBetTransaction(int batchSize)
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
                else
                {
                    await logService.ErrorMessageAsync($"DCS - DataTrace GetMissingBetTransactionPackageAsync does not return data for date: {transDate:yyyy-MM-dd}");
                }
            }

            return true;
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
                        await logService.ErrorAsync(ex);
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
                else
                {
                    await logService.ErrorMessageAsync($"DCS - DataTrace GetMissingLoginTranPackageAsync does not return data for date: {transDate:yyyy-MM-dd}");
                }
            }

            return true;
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
                        await logService.ErrorAsync(ex);
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
            await dataAccess.UpdateSystemSettingParameterByID(SystemSettingID, SystemSettingValue);

            return true;
        }

        public async Task<int> CountDWBetTransactionByDate(DateTime transDate)
        {
            if (!await IsDailyTaskReadyAsync())
            {
                return 0;
            }

            var data = await dataAccess.GetBetTransactionAsync(transDate);

            return data != null && data.Any() ? data.Count() : 0;
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
                        await logService.ErrorAsync(ex);
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
            return await dataAccess.IsDailyTaskReadyAsync(dailyTaskFPSID);
        }

        public async Task<int> CookDataLoginTransaction(int batchSize)
        {
            return await dataAccess.CookDataLoginTransaction(batchSize);
        }

        #region Report

        public async Task<IEnumerable<ReportMissingTransactionModel>> GetReportMissingTransaction(DateTime fromDate, DateTime toDate)
        {
            return await dataAccess.GetReportMissingTransaction(fromDate, toDate);
        }

        public async Task<IEnumerable<ReportLoginTransactionBySubscriberModel>> GetReportLoginTransactionBySubscriber(DateTime fromDate, DateTime toDate)
        {
            return await dataAccess.GetReportLoginTransactionBySubscriber(fromDate, toDate);
        }

        public async Task<IEnumerable<ReportLoginTransactionBySiteModel>> GetReportLoginTransactionBySite(DateTime fromDate, DateTime toDate)
        {
            return await dataAccess.GetReportLoginTransactionBySite(fromDate, toDate);
        }

        public async Task<IEnumerable<ReportLoginTransactionByAccountModel>> GetReportLoginTransactionByAccount(DateTime fromDate, DateTime toDate)
        {
            return await dataAccess.GetReportLoginTransactionByAccount(fromDate, toDate);
        }

        public async Task<IEnumerable<ReportLoginTransactionByBrowserModel>> GetReportLoginTransactionByBrowser(DateTime fromDate, DateTime toDate)
        {
            return await dataAccess.GetReportLoginTransactionByBrowser(fromDate, toDate);
        }

        public async Task<IEnumerable<ReportLoginTransactionByFlaggedModel>> GetReportLoginTransactionByFlagged(DateTime fromDate, DateTime toDate)
        {
            return await dataAccess.GetReportLoginTransactionByFlagged(fromDate, toDate);
        }

        public async Task<IEnumerable<ReportTransactionLogstashModel>> GetTransactionLogstash(string subscriberName, string accountName, DateTime fromDate, DateTime toDate, int batchSize)
        {
            return await dataAccess.GetTranPackageAsync(subscriberName, accountName, fromDate, toDate, batchSize);
        }
        #endregion
    }
}
