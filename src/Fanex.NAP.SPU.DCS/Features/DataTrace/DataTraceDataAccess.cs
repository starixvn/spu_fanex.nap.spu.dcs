namespace Fanex.NAP.SPU.DCS.Features.DataTrace
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.DCS.Features.DataTrace.Criteria;
    using Fanex.NAP.SPU.DCS.Features.DataTrace.Model;

    public interface IDataTraceDataAccess
    {
        Task<int> InsertSummaryLoginTransactionAsync(int batchSize);

        Task<bool> IsDailyTaskReadyAsync(int taskID);

        Task<IEnumerable<BetTicketInfo>> GetBetTicketAsync(DateTime? fromDate, DateTime? toDate);

        Task<int> InsertSummaryBetTicketAsync(string rawTransJson);

        Task<int> InsertSummaryMissingLoginTransBetTicketAsync();
    }
    public class DataTraceDataAccess : IDataTraceDataAccess
    {
        private readonly IDynamicRepository repository;

        public DataTraceDataAccess()
        {
            repository = new DynamicRepository();
        }

        public async Task<int> InsertSummaryLoginTransactionAsync(int batchSize)
        {
            var criteria = new InsertSumaryLoginTransactionCriteria
            {
                BatchSize = batchSize
            };

            return await repository.ExecuteAsync(criteria);
        }

        public async Task<bool> IsDailyTaskReadyAsync(int taskID)
        {
            var criteria = new CheckDailyTaskReadyCriteria
            {
                TaskID = taskID
            };

            return await repository.GetAsync<bool>(criteria);
        }

        public async Task<IEnumerable<BetTicketInfo>> GetBetTicketAsync(DateTime? fromDate, DateTime? toDate)
        {
            var criteria = new GetBetTickeCriteria
            {
                FromDate = fromDate ?? DateTime.Now.AddDays(-1),
                ToDate = toDate ?? DateTime.Now.AddDays(-1)
            };

            return await repository.FetchAsync<BetTicketInfo>(criteria);
        }

        public async Task<int> InsertSummaryBetTicketAsync(string rawTransJson)
        {
            var criteria = new InsertSummaryBetTicketCriteria
            {
                RawTransJson = rawTransJson
            };

            return await repository.ExecuteAsync(criteria);
        }

        public async Task<int> InsertSummaryMissingLoginTransBetTicketAsync()
        {
            var criteria = new InsertSummaryMissingLoginTransBetTicketCriteria();

            return await repository.ExecuteAsync(criteria);
        }
    }
}
