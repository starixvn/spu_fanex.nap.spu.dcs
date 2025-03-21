namespace Fanex.NAP.SPU.DCS.Features.DataTrace
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.DCS.Features.DataTrace.Criteria;
    using Fanex.NAP.SPU.DCS.Features.DataTrace.Model;

    public interface IDataTraceDataAccess
    {
        Task<bool> IsDailyTaskReadyAsync(int taskID);

        Task<IEnumerable<BetTransactionInfo>> GetBetTransactionAsync(DateTime transDate);

        Task<int> InsertSummaryBetTransactionAsync(string rawTransJson);

        Task<int> DeleteBetTransactionSummaryByDateAsync(DateTime transDate);

        Task<string> GetSystemSettingParameterByID(int parameterID);

        Task<int> UpdateSystemSettingParameterByID(int id, string value);

        Task<IEnumerable<string>> GetMissingBetTransactionPackageAsync(DateTime transDate, int batchSize);

        Task<int> InsertMissingBetTransactionAsync(DateTime transDate, string listAccountID);

        Task<IEnumerable<RangeIDPackage>> GetMissingLoginTranPackageAsync(DateTime transDate, int batchSize);

        Task<int> InsertMissingLoginTransactionAsync(DateTime transDate, int minID, int maxID);

        Task<int> DeleteMissingTransactionByDate(DateTime transDate, int missingType);

        Task<int> CookDataLoginTransaction(int batchSize);

        IEnumerable<TransactionLogstashModel> GetTransactionForLogtash(int batchSize);

        Task<IEnumerable<ReportMissingTransactionModel>> GetReportMissingTransaction(DateTime fromDate, DateTime toDate);

        Task<IEnumerable<ReportLoginTransactionBySubscriberModel>> GetReportLoginTransactionBySubscriber(DateTime fromDate, DateTime toDate);

        Task<IEnumerable<ReportLoginTransactionBySiteModel>> GetReportLoginTransactionBySite(DateTime fromDate, DateTime toDate);

        Task<IEnumerable<ReportLoginTransactionByAccountModel>> GetReportLoginTransactionByAccount(DateTime fromDate, DateTime toDate);

        Task<IEnumerable<ReportLoginTransactionByBrowserModel>> GetReportLoginTransactionByBrowser(DateTime fromDate, DateTime toDate);

        Task<IEnumerable<ReportLoginTransactionByFlaggedModel>> GetReportLoginTransactionByFlagged(DateTime fromDate, DateTime toDate);
    }
    public class DataTraceDataAccess : IDataTraceDataAccess
    {
        private readonly IDynamicRepository repository;

        public DataTraceDataAccess(IDynamicRepository repository)
        {
            this.repository = repository ?? new DynamicRepository();
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


        public async Task<IEnumerable<string>> GetMissingBetTransactionPackageAsync(DateTime transDate, int batchSize)
        {
            var criteria = new GetMissingBetTransactionPackageCriteria
            {
                TransDate = transDate,
                BatchSize = batchSize,
            };

            return await repository.FetchAsync<string>(criteria);
        }

        public async Task<int> InsertMissingBetTransactionAsync(DateTime transDate, string listAccountID)
        {
            var criteria = new InsertMissingBetTransactionCriteria
            {
                TransDate = transDate,
                ListAccountID = listAccountID
            };

            return await repository.ExecuteAsync(criteria);
        }

        public async Task<IEnumerable<RangeIDPackage>> GetMissingLoginTranPackageAsync(DateTime transDate, int batchSize)
        {
            var criteria = new GetMissingLoginTransactionPackageCriteria
            {
                TransDate = transDate,
                BatchSize = batchSize,
            };

            return await repository.FetchAsync<RangeIDPackage>(criteria);
        }

        public async Task<int> InsertMissingLoginTransactionAsync(DateTime transDate, int minID, int maxID)
        {
            var criteria = new InsertMissingLoginTransactionCriteria
            {
                TransDate = transDate,
                MinID = minID,
                MaxID = maxID
            };

            return await repository.ExecuteAsync(criteria);
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

        public async Task<int> CookDataLoginTransaction(int batchSize)
        {
            var criteria = new CookDataLoginTransactionCriteria
            {
                BatchSize = batchSize
            };

            return await repository.ExecuteAsync(criteria);
        }

        #region Report

        public async Task<IEnumerable<ReportMissingTransactionModel>> GetReportMissingTransaction(DateTime fromDate, DateTime toDate)
        {
            var criteria = new GetReportMissingTransactionCriteria
            {
                FromDate = fromDate,
                ToDate = toDate
            };

            return await repository.FetchAsync<ReportMissingTransactionModel>(criteria);
        }

        public async Task<IEnumerable<ReportLoginTransactionBySubscriberModel>> GetReportLoginTransactionBySubscriber(DateTime fromDate, DateTime toDate)
        {
            var criteria = new GetReportLoginTransactionBySubscriberCriteria
            {
                FromDate = fromDate,
                ToDate = toDate
            };

            return await repository.FetchAsync<ReportLoginTransactionBySubscriberModel>(criteria);
        }

        public async Task<IEnumerable<ReportLoginTransactionBySiteModel>> GetReportLoginTransactionBySite(DateTime fromDate, DateTime toDate)
        {
            var criteria = new GetReportLoginTransactionBySiteCriteria
            {
                FromDate = fromDate,
                ToDate = toDate
            };

            return await repository.FetchAsync<ReportLoginTransactionBySiteModel>(criteria);
        }

        public async Task<IEnumerable<ReportLoginTransactionByAccountModel>> GetReportLoginTransactionByAccount(DateTime fromDate, DateTime toDate)
        {
            var criteria = new GetReportLoginTransactionByAccountCriteria
            {
                FromDate = fromDate,
                ToDate = toDate
            };

            return await repository.FetchAsync<ReportLoginTransactionByAccountModel>(criteria);
        }

        public async Task<IEnumerable<ReportLoginTransactionByBrowserModel>> GetReportLoginTransactionByBrowser(DateTime fromDate, DateTime toDate)
        {
            var criteria = new GetReportLoginTransactionByBrowserCriteria
            {
                FromDate = fromDate,
                ToDate = toDate
            };

            return await repository.FetchAsync<ReportLoginTransactionByBrowserModel>(criteria);
        }

        public async Task<IEnumerable<ReportLoginTransactionByFlaggedModel>> GetReportLoginTransactionByFlagged(DateTime fromDate, DateTime toDate)
        {
            var criteria = new GetReportLoginTransactionByFlaggedCriteria
            {
                FromDate = fromDate,
                ToDate = toDate
            };

            return await repository.FetchAsync<ReportLoginTransactionByFlaggedModel>(criteria);
        }

        public IEnumerable<TransactionLogstashModel> GetTransactionForLogtash(int batchSize)
        {
            var criteria = new GetTranPackageCriteria {
                BatchSize = batchSize
            };

            return repository.Fetch<TransactionLogstashModel>(criteria);
        }
        #endregion
    }
}
