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

        Task<IEnumerable<BetTransactionInfo>> GetBetTransactionAsync(DateTime fromTransDate, DateTime toTransDate);

        Task<IEnumerable<BetTransactionInfo>> GetInitBetTransactionAsync(DateTime fromTransDate, DateTime toTransDate);

        Task<int> InsertSummaryBetTransactionAsync(string rawTransJson);

        Task<int> UpdateSystemSettingParameter(int id, string value);

        Task<IEnumerable<RangeIDPackage>> GetMissingBetTransactionPacketAsync(int batchSize);

        Task<int> InsertMissingBetTransactionAsync(int minID, int maxID);

        Task<IEnumerable<RangeIDPackage>> GetMissingLoginTranPacketAsync(int batchSize);

        Task<int> InsertMissingLoginTransactionAsync(int minID, int maxID);

        Task<int> DeleteMissingTransactionByDate(DateTime transDate, int missingType);
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

        public async Task<IEnumerable<BetTransactionInfo>> GetBetTransactionAsync(DateTime fromTransDate, DateTime toTransDate)
        {
            var criteria = new GetBetTransactionCriteria
            {
                FromTransDate = fromTransDate,
                ToTransDate = toTransDate
            };

            return await repository.FetchAsync<BetTransactionInfo>(criteria);
        }

        public async Task<int> InsertSummaryBetTransactionAsync(string rawTransJson)
        {
            var criteria = new InsertSummaryBetTransactionCriteria
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


        public async Task<IEnumerable<RangeIDPackage>> GetMissingBetTransactionPacketAsync(int batchSize)
        {
            var criteria = new GetMissingBetTransactionPackageCriteria
            {
                BatchSize = batchSize,
            };

            return await repository.FetchAsync<RangeIDPackage>(criteria);
        }

        public async Task<int> InsertMissingBetTransactionAsync(int minID, int maxID)
        {
            var criteria = new InsertMissingBetTransactionCriteria
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

        public async Task<int> InsertMissingLoginTransactionAsync(int minID, int maxID)
        {
            var criteria = new InsertMissingLoginTransactionCriteria
            {
                MinID = minID,
                MaxID = maxID
            };

            return await repository.ExecuteAsync(criteria);
        }

        public async Task<IEnumerable<BetTransactionInfo>> GetInitBetTransactionAsync(DateTime fromTransDate, DateTime toTransDate)
        {
            var criteria = new GetInitBetTransactionCriteria
            {
                FromTransDate = fromTransDate,
                ToTransDate = toTransDate
            };

            return await repository.FetchAsync<BetTransactionInfo>(criteria);
        }

        public async Task<int> DeleteMissingTransactionByDate(DateTime transDate, int missingType)
        {
            var criteria = new DeleteMissingTransactionByDateCriteria
            {
                TransDate = transDate,
                MissingType = missingType
            };

            return await repository.ExecuteAsync(criteria);
        }
    }
}
