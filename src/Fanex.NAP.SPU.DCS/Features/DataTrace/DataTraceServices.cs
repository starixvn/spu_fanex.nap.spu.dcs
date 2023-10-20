namespace Fanex.NAP.SPU.DCS.Features.DataTrace
{
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.SPU.DCS.Features.DataTrace.Model;
    using Fanex.NAP.SPU.DCS.Features.DataTrace.Model.Dto;
    using Microsoft.SqlServer.Server;
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

        Task<bool> ProcessSummaryBetTransactionAsync(DateTime? fromDate, DateTime? toDate);

        Task<bool> ProcessInitBetTransactionAsync(DateTime fromDate, DateTime toDate);

        Task<bool> ProcessSummaryMissingBetTransactionAsync(int batchSize);

        Task<bool> ProcessSummaryMissingLoginTransactionAsync(int batchSize);

        Task<bool> UpdateSystemSetting(int SystemSettingID, string SystemSettingValue);

        Task<bool> DeleteMissingTransactionByDate(DateTime transDate, int missingType);

        Task<IEnumerable<ReportLoginTransactionModel>> GetReportLoginTransaction(DateTime fromDate, DateTime toDate);

        Task<IEnumerable<ReportBotDTransactionModel>> GetReportBotDTransaction(DateTime fromDate, DateTime toDate);

        Task<IEnumerable<ReportMissingTransactionModel>> GetReportMissingTransaction(DateTime fromDate, DateTime toDate);

        Task<IEnumerable<ReportLoginTransactionDetailModel>> GetReportLoginTransactionDetail(DateTime transDate);
    }

    public class DataTraceServices : IDataTraceServices
    {
        private readonly IDataTraceDataAccess dataAccess;
        private readonly ISentryClient sentryClient;

        private const int batchSize = 50000;
        private const int dailyTaskFPSID = 104;
        private const int delayTime = 100;
        private const int CONST_Summary_maxBetTransactionDateID = 2;

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

                if (isDailyTaskReady)
                {
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
                                _ = await InsertSummaryBetTransactionByBatchAsync(TransformBetTransactionInfo(betTransactionInfos));
                            }
                        }
                    }
                    else
                    {
                        var maxBetTicketDateStr = await dataAccess.GetSystemSettingParameterByID(CONST_Summary_maxBetTransactionDateID);
                        DateTime maxBetTicketDate = DateTime.ParseExact(maxBetTicketDateStr, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                        DateTime yesterday = DateTime.Now.Date.AddDays(-1);

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
                                var isSuccessful = await InsertSummaryBetTransactionByBatchAsync(TransformBetTransactionInfo(betTransactionInfos));

                                if (isSuccessful)
                                {
                                    var transDateFormat = date.ToString("yyyy-MM-dd");
                                    await dataAccess.UpdateSystemSettingParameterByID(CONST_Summary_maxBetTransactionDateID, transDateFormat);
                                }
                                else
                                {
                                    LogService.Error($"DCS - CRUDSummaryBetTransaction is not success!");
                                }
                            }
                        }
                    }

                    return true;
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
                    var betTransactionInfos = await dataAccess.GetInitBetTransactionAsync(date);

                    if (betTransactionInfos != null && betTransactionInfos.Any())
                    {
                        _ = await dataAccess.DeleteBetTransactionSummaryByDateAsync(date);
                        await InsertSummaryBetTransactionByBatchAsync(TransformBetTransactionInfo(betTransactionInfos));
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
                await dataAccess.UpdateSystemSettingParameterByID(SystemSettingID, SystemSettingValue);

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

        public async Task<IEnumerable<ReportMissingTransactionModel>> GetReportMissingTransaction(DateTime fromDate, DateTime toDate)
        {
            return await dataAccess.GetReportMissingTransaction(fromDate, toDate);
        }
    }
}
