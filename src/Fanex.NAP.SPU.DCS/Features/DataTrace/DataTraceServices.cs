namespace Fanex.NAP.SPU.DCS.Features.DataTrace
{
    using System;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using System.Linq;
    using System.Collections.Generic;
    using Sentry;

    public interface IDataTraceServices
    {
        Task<int> InsertSummaryLoginTransactionAsync(int batchSize);

        Task<bool> ProcessSummaryBetTicketAsync(DateTime? fromDate, DateTime? toDate);

        Task<bool> ProcessSummaryMissingBetTicketAsync(int batchSize);

        Task<bool> ProcessSummaryMissingLoginTransactionAsync(int batchSize);
    }

    public class DataTraceServices : IDataTraceServices
    {
        private readonly IDataTraceDataAccess dataAccess;
        private const int dailyTaskFPSID = 104;
        private readonly ISentryClient sentryClient;
        private const int delayTime = 100; // milliseconds
        private const int maxBetTicketDateID = 2;

        public DataTraceServices(IDataTraceDataAccess dataAccess, ISentryClient sentryClient)
        {
            this.dataAccess = dataAccess;
            this.sentryClient = sentryClient;
        }

        public async Task<int> InsertSummaryLoginTransactionAsync(int batchSize)
        {
            return await dataAccess.InsertSummaryLoginTransactionAsync(batchSize);
        }

        public async Task<bool> ProcessSummaryBetTicketAsync(DateTime? fromDate, DateTime? toDate)
        {
            var isDailyTaskReady = await dataAccess.IsDailyTaskReadyAsync(dailyTaskFPSID);

            if (isDailyTaskReady)
            {
                var transDate = DateTime.Now.AddDays(-1);
                var fromTransDate = fromDate ?? transDate;
                var toTransDate = fromDate ?? transDate;

                var betTicketInfos = await dataAccess.GetBetTicketAsync(fromTransDate, toTransDate);

                if (betTicketInfos != null && betTicketInfos.Any())
                {
                    var rawTransJson = JsonConvert.SerializeObject(betTicketInfos);
                    await dataAccess.InsertSummaryBetTicketAsync(rawTransJson);

                    var transDateFormat = transDate.ToString("yyyy-MM-dd");
                    await dataAccess.UpdateSystemSettingParameter(maxBetTicketDateID, transDateFormat);

                    return true;
                }
            }

            return false;
        }

        public async Task<bool> ProcessSummaryMissingBetTicketAsync(int batchSize)
        {
            var missingBetTicketIDPackages = await dataAccess.GetMissingBetTicketsPacketAsync(batchSize);
            if (missingBetTicketIDPackages != null && missingBetTicketIDPackages.Any())
            {
                var tasks = new List<Task>();

                foreach (var package in missingBetTicketIDPackages)
                {
                    var task = Task.Run(() =>
                    {
                        try
                        {
                            dataAccess.InsertSummaryMissingBetTicketAsync(package.MinID, package.MaxID);
                        }
                        catch (Exception ex)
                        {
                            sentryClient.CaptureException(ex);
                        }
                    });

                    await Task.Delay(delayTime);

                    tasks.Add(task);
                }

                await Task.WhenAll(tasks.ToArray());

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
                    var task = Task.Run(() =>
                    {
                        try
                        {
                            dataAccess.InsertSummaryMissingLoginTransactionAsync(package.MinID, package.MaxID);
                        }
                        catch (Exception ex)
                        {
                            sentryClient.CaptureException(ex);
                        }
                    });

                    await Task.Delay(delayTime);

                    tasks.Add(task);
                }

                await Task.WhenAll(tasks.ToArray());

                return true;
            }

            return false;
        }
    }
}
