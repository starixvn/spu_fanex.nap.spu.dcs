namespace Fanex.NAP.SPU.DCS.Features.DataTrace
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.DCS.Features.DataTrace.Criteria;
    using Fanex.NAP.SPU.DCS.Features.DataTrace.Model;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IDataTraceDataAccess
    {
        Task<int> InsertSummaryLoginTransactionAsync(int batchSize);

        Task<bool> IsDailyTaskReadyAsync(int taskID);

        Task<IEnumerable<BetTransactionInfo>> GetBetTransactionAsync(DateTime transDate);

        Task<IEnumerable<BetTransactionInfo>> GetInitBetTransactionAsync(DateTime transDate);

        Task<int> InsertSummaryBetTransactionAsync(string rawTransJson);

        Task<int> DeleteBetTransactionSummaryByDateAsync(DateTime transDate);

        Task<string> GetSystemSettingParameterByID(int id);

        Task<int> UpdateSystemSettingParameterByID(int id, string value);

        Task<IEnumerable<RangeIDPackage>> GetMissingBetTransactionPacketAsync(int batchSize);

        Task<int> InsertMissingBetTransactionAsync(int minID, int maxID);

        Task<IEnumerable<RangeIDPackage>> GetMissingLoginTranPacketAsync(int batchSize);

        Task<int> InsertMissingLoginTransactionAsync(int minID, int maxID);

        Task<int> DeleteMissingTransactionByDate(DateTime transDate, int missingType);

        Task<IEnumerable<ReportLoginTransactionModel>> GetReportLoginTransaction(DateTime fromDate, DateTime toDate);

        Task<IEnumerable<ReportLoginTransactionDetailModel>> GetReportLoginTransactionDetail(DateTime transDate);

        Task<IEnumerable<ReportBotDTransactionModel>> GetReportBotDTransaction(DateTime fromDate, DateTime toDate);

        Task<IEnumerable<ReportMissingTransactionModel>> GetReportMissingTransaction(DateTime fromDate, DateTime toDate);
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

        public async Task<IEnumerable<BetTransactionInfo>> GetBetTransactionAsync(DateTime transDate)
        {
            var criteria = new GetBetTransactionCriteria
            {
                FromTransDate = transDate,
                ToTransDate = transDate
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

        public async Task<int> DeleteBetTransactionSummaryByDateAsync(DateTime transDate)
        {
            var criteria = new DeleteBetTransactionSummaryByDateCriteria
            {
                TransDate = transDate
            };

            return await repository.ExecuteAsync(criteria);
        }

        public async Task<string> GetSystemSettingParameterByID(int parameterID)
        {
            var criteria = new GetSystemSettingParameterByIDCriteria
            {
                ParameterID = parameterID
            };

            using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
            return await db.QueryEntityAsync<string>(criteria);
        }

        public async Task<int> UpdateSystemSettingParameterByID(int id, string value)
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

        public async Task<IEnumerable<BetTransactionInfo>> GetInitBetTransactionAsync(DateTime transDate)
        {
            var criteria = new GetInitBetTransactionCriteria
            {
                FromTransDate = transDate,
                ToTransDate = transDate
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

        public async Task<IEnumerable<ReportLoginTransactionModel>> GetReportLoginTransaction(DateTime fromDate, DateTime toDate)
        {
            var criteria = new GetReportLoginTransactionCriteria
            {
                FromDate = fromDate,
                ToDate = toDate
            };

            return await repository.FetchAsync<ReportLoginTransactionModel>(criteria);
        }

        public async Task<IEnumerable<ReportBotDTransactionModel>> GetReportBotDTransaction(DateTime fromDate, DateTime toDate)
        {
            var criteria = new GetReportBotDTransactionCriteria
            {
                FromDate = fromDate,
                ToDate = toDate
            };

            return await repository.FetchAsync<ReportBotDTransactionModel>(criteria);
        }

        public async Task<IEnumerable<ReportLoginTransactionDetailModel>> GetReportLoginTransactionDetail(DateTime transDate)
        {
            var criteria = new GetReportLoginTransactionDetailCriteria
            {
                TransDate = transDate
            };

            return await repository.FetchAsync<ReportLoginTransactionDetailModel>(criteria);
        }

        public async Task<IEnumerable<ReportMissingTransactionModel>> GetReportMissingTransaction(DateTime fromDate, DateTime toDate)
        {
            var criteria = new GetReportMissingTransactionCriteria
            {
                FromDate = fromDate,
                ToDate = toDate
            };

            return await repository.FetchAsync<ReportMissingTransactionModel>(criteria);
        }
    }
}
