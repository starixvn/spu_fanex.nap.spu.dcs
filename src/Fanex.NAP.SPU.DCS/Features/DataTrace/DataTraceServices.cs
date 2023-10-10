namespace Fanex.NAP.SPU.DCS.Features.DataTrace
{
    using System;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using System.Linq;
    using System.Collections.Generic;
    using Sentry;
    using Fanex.NAP.SPU.DCS.Features.DataTrace.Model;

    public interface IDataTraceServices
    {
        Task<int> InsertSummaryLoginTransactionAsync(int batchSize);

        Task<bool> ProcessSummaryBetTransactionAsync(DateTime? fromDate, DateTime? toDate);

        Task<bool> ProcessSummaryMissingBetTransactionAsync(int batchSize);

        Task<bool> ProcessSummaryMissingLoginTransactionAsync(int batchSize);
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

                if (isDailyTaskReady)
                {
                    var transDate = DateTime.Now.AddDays(-1);
                    var fromTransDate = fromDate ?? transDate;
                    var toTransDate = fromDate ?? transDate;

                    var betTransactionInfos = await dataAccess.GetBetTransactionAsync(fromTransDate, toTransDate);

                    if (betTransactionInfos != null && betTransactionInfos.Any())
                    {
                        var isSuccessful = await InsertSummaryBetTransactionByBatchAsync(betTransactionInfos);

                        if (isSuccessful)
                        {
                            var transDateFormat = fromTransDate.ToString("yyyy-MM-dd");
                            await dataAccess.UpdateSystemSettingParameter(maxBetTransactionDateID, transDateFormat);

                            return true;
                        }
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                sentryClient.CaptureException(ex);
                throw;
            }
        }

        private async Task<bool> InsertSummaryBetTransactionByBatchAsync(IEnumerable<BetTransactionInfo> betTransactionInfos)
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
            if (missingLoginIDPackages != null&& missingLoginIDPackages.Any())
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
    }
}
