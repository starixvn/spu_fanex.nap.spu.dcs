namespace Fanex.NAP.SPU.DCS.Features.DataTrace
{
    using System;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using System.Linq;

    public interface IDataTraceServices
    {
        Task<int> InsertSummaryLoginTransactionAsync(int batchSize);

        Task<bool> ProcessSummaryBetTicketAsync(DateTime? fromDate, DateTime? toDate);

        Task<int> InsertSummaryMissingLoginTransBetTicketAsync();
    }

    public class DataTraceServices : IDataTraceServices
    {
        private readonly IDataTraceDataAccess dataAccess;
        private const int dailyTaskFPSID = 104;

        public DataTraceServices(IDataTraceDataAccess dataAccess)
        {
            this.dataAccess = dataAccess;
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
                var betTicketInfos = await dataAccess.GetBetTicketAsync(fromDate, toDate);

                if (betTicketInfos != null && betTicketInfos.Any())
                {
                    var rawTransJson = JsonConvert.SerializeObject(betTicketInfos);
                    await dataAccess.InsertSummaryBetTicketAsync(rawTransJson);

                    return true;
                }
            }

            return false;
        }

        public async Task<int> InsertSummaryMissingLoginTransBetTicketAsync()
        {
            return await dataAccess.InsertSummaryMissingLoginTransBetTicketAsync();
        }
    }
}
