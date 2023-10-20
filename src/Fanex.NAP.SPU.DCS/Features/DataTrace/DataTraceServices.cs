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
    using System.Linq;
    using System.Threading.Tasks;

    public interface IDataTraceServices
    {
        Task<int> InsertSummaryLoginTransactionAsync(int batchSize);

        Task<bool> ProcessSummaryBetTransactionAsync(DateTime? fromDate, DateTime? toDate);

        Task<bool> ProcessInitBetTransactionAsync(DateTime fromDate, DateTime toDate);

        Task<bool> ProcessSummaryMissingBetTransactionAsync(int batchSize);

        Task<bool> ProcessSummaryMissingLoginTransactionAsync(int batchSize);

        Task<bool> UpdateSystemSetting(int SystemSettingID, string SystemSettingValue);

        Task<bool> DeleteMissingTransactionByDate(DateTime transDate, int missingType);

        Task<IEnumerable<ReportLoginTransactionModel>> GetReportLoginTransaction(DateTime fromDate, DateTime toDate);

        Task<IEnumerable<ReportBotDTransactionModel>> GetReportBotDTransaction(DateTime fromDate, DateTime toDate);

        Task<IEnumerable<ReportLoginTransactionDetailModel>> GetReportLoginTransactionDetail(DateTime transDate);
    }

    public class DataTraceServices : IDataTraceServices
    {
        private readonly IDataTraceDataAccess dataAccess;
        private readonly ISentryClient sentryClient;

        private const int batchSize = 50000;
        private const int dailyTaskFPSID = 104;
        private const int delayTime = 100;
        private const int maxBetTransactionDateID = 2;

        public DataTraceServices(IDataTraceDataAccess dataAccess, ISentryClient sentryClient)
        {
            this.dataAccess = dataAccess;
            this.sentryClient = sentryClient;
        }

        public async Task<int> InsertSummaryLoginTransactionAsync(int batchSize)
        {
            return await dataAccess.InsertSummaryLoginTransactionAsync(batchSize);
        }

        public async Task<bool> ProcessSummaryBetTransactionAsync(DateTime? fromDate, DateTime? toDate)
        {
            try
            {
                var isDailyTaskReady = await dataAccess.IsDailyTaskReadyAsync(dailyTaskFPSID);

                sentryClient.CaptureMessage("Trace log: isDailyTaskReady = " + isDailyTaskReady.ToString());
                if (isDailyTaskReady)
                {
                    var transDate = DateTime.Now.AddDays(-1);
                    var fromTransDate = fromDate ?? transDate;
                    var toTransDate = fromDate ?? transDate;

                    sentryClient.CaptureMessage("Trace log: transDate = " + transDate.ToString());
                    var betTransactionInfos = await dataAccess.GetBetTransactionAsync(fromTransDate, toTransDate);

                    if (betTransactionInfos != null && betTransactionInfos.Any())
                    {
                        var isSuccessful = await InsertSummaryBetTransactionByBatchAsync(TransformBetTransactionInfo(betTransactionInfos));

                        if (isSuccessful)
                        {
                            var transDateFormat = fromTransDate.ToString("yyyy-MM-dd");
                            await dataAccess.UpdateSystemSettingParameter(maxBetTransactionDateID, transDateFormat);

                            return true;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                sentryClient.CaptureException(ex);
                var logMessage = new LogMessage(ex.Message)
                        .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                        .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error($"DCS - DataTrace summary bet tickets job execute exeption message:  {logMessage}");
            }

            return false;
        }

        private IEnumerable<BetTransactionInfoDto> TransformBetTransactionInfo(IEnumerable<BetTransactionInfo> model)
        {
            return model.Select(x => new BetTransactionInfoDto
            {
                TransDate = x.TransDate,
                CustID = x.CustID,
                BetCount = x.BetCount,
                FirstBetTransactionTime = x.FirstBetTicketTime,
                LastBetTransactionTime = x.LastBetTicketTime
            });
        }

        private async Task<bool> InsertSummaryBetTransactionByBatchAsync(IEnumerable<BetTransactionInfoDto> betTransactionInfos)
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

        public async Task<bool> ProcessSummaryMissingBetTransactionAsync(int batchSize)
        {
            var missingBetTransactionIDPackages = await dataAccess.GetMissingBetTransactionPacketAsync(batchSize);
            if (missingBetTransactionIDPackages != null && missingBetTransactionIDPackages.Any())
            {
                var tasks = new List<Task>();

                foreach (var package in missingBetTransactionIDPackages)
                {
                    var task = Task.Run(async () =>
                    {
                        try
                        {
                            await dataAccess.InsertMissingBetTransactionAsync(package.MinID, package.MaxID);
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

            return false;
        }

        public async Task<bool> ProcessSummaryMissingLoginTransactionAsync(int batchSize)
        {
            var missingLoginIDPackages = await dataAccess.GetMissingLoginTranPacketAsync(batchSize);
            if (missingLoginIDPackages != null && missingLoginIDPackages.Any())
            {
                var tasks = new List<Task>();

                foreach (var package in missingLoginIDPackages)
                {
                    var task = Task.Run(async () =>
                    {
                        try
                        {
                            await dataAccess.InsertMissingLoginTransactionAsync(package.MinID, package.MaxID);
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

            return false;
        }

        public async Task<bool> ProcessInitBetTransactionAsync(DateTime fromDate, DateTime toDate)
        {
            try
            {
                for (var date = fromDate; date <= toDate; date = date.AddDays(1))
                {
                    var betTransactionInfos = await dataAccess.GetInitBetTransactionAsync(date, date);

                    if (betTransactionInfos != null && betTransactionInfos.Any())
                    {
                        var isSuccessful = await InsertSummaryBetTransactionByBatchAsync(TransformBetTransactionInfo(betTransactionInfos));

                        if (isSuccessful)
                        {
                            var transDateFormat = date.ToString("yyyy-MM-dd");
                            await dataAccess.UpdateSystemSettingParameter(maxBetTransactionDateID, transDateFormat);

                        }
                    }
                }

                return true;

            }
            catch (Exception ex)
            {
                sentryClient.CaptureException(ex);
                var logMessage = new LogMessage(ex.Message)
                        .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                        .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error($"DCS - DataTrace summary bet tickets job execute exeption message:  {logMessage}");
            }

            return false;
        }

        public async Task<bool> UpdateSystemSetting(int SystemSettingID, string SystemSettingValue)
        {
            try
            {
                await dataAccess.UpdateSystemSettingParameter(SystemSettingID, SystemSettingValue);

                return true;
            }
            catch (Exception ex)
            {
                sentryClient.CaptureException(ex);
                var logMessage = new LogMessage(ex.Message)
                        .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                        .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error($"DCS - DataTrace UpdateSystemSetting job execute exeption message:  {logMessage}");
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
                sentryClient.CaptureException(ex);
                var logMessage = new LogMessage(ex.Message)
                        .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                        .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error($"DCS - DataTrace DeleteMissingTransactionByDate job execute exeption message:  {logMessage}");
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
    }
}
