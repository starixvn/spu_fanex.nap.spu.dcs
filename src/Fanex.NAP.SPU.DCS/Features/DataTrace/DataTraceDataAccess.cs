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

        Task<IEnumerable<BetTicketInfo>> GetBetTicketAsync(DateTime fromTransDate, DateTime toTransDate);

        Task<int> InsertSummaryBetTicketAsync(string rawTransJson);

        Task<int> UpdateSystemSettingParameter(int id, string value);

        Task<IEnumerable<RangeIDPackage>> GetMissingBetTicketsPacketAsync(int batchSize);

        Task<int> InsertSummaryMissingBetTicketAsync(int minID, int maxID);

        Task<IEnumerable<RangeIDPackage>> GetMissingLoginTranPacketAsync(int batchSize);

        Task<int> InsertSummaryMissingLoginTransactionAsync(int minID, int maxID);
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

        public async Task<IEnumerable<BetTicketInfo>> GetBetTicketAsync(DateTime fromTransDate, DateTime toTransDate)
        {
            var criteria = new GetBetTickeCriteria
            {
                FromTransDate = fromTransDate,
                ToTransDate = toTransDate
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

        public async Task<int> UpdateSystemSettingParameter(int id, string value)
        {
            var criteria = new UpdateSystemSettingParameterCriteria
            {
                ID = id,
                Value = value
            };

            return await repository.ExecuteAsync(criteria);
        }


        public async Task<IEnumerable<RangeIDPackage>> GetMissingBetTicketsPacketAsync(int batchSize)
        {
            var criteria = new GetMissingBetTicketPackageCriteria
            {
                BatchSize = batchSize,
            };

            return await repository.FetchAsync<RangeIDPackage>(criteria);
        }

        public async Task<int> InsertSummaryMissingBetTicketAsync(int minID, int maxID)
        {
            var criteria = new InsertSummaryMissingBetTicketCriteria
            {
                MinID = minID,
                MaxID = maxID
            };

            return await repository.ExecuteAsync(criteria);
        }

        public async Task<IEnumerable<RangeIDPackage>> GetMissingLoginTranPacketAsync(int batchSize)
        {
            var criteria = new GetMissingLoginTransactionPackageCriteria
            {
                BatchSize = batchSize,
            };

            return await repository.FetchAsync<RangeIDPackage>(criteria);
        }

        public async Task<int> InsertSummaryMissingLoginTransactionAsync(int minID, int maxID)
        {
            var criteria = new InsertSummaryMissingLoginTransactionCriteria
            {
                MinID = minID,
                MaxID = maxID
            };

            return await repository.ExecuteAsync(criteria);
        }
    }
}
