namespace Fanex.NAP.SPU.DCS.Features.Transactions
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.DCS.Features.Transactions.Criteria;
    using Fanex.NAP.SPU.DCS.Features.Transactions.Entities;
    using System.Collections.Generic;

    public interface ITransactionDataAccess
    {
        IEnumerable<RawTransactionEntity> GetRawTransaction(int isProcessed, int numberOfTrans, int numberOfBatch, bool isReadyDbExtra);

        void InserUserAgent(long fromId, long toId, bool isReadyDbExtra);

        void InsertTransaction(long fromId, long toId, bool isReadyDbExtra);

        void UnhandleSubscriberActionProcess(long fromId, long toId, bool isReadyDbExtra);
    }
    public class TransactionDataAccess : ITransactionDataAccess
    {
        private readonly IDynamicRepository repository;

        public TransactionDataAccess()
        {
            repository = new DynamicRepository();
        }

        public IEnumerable<RawTransactionEntity> GetRawTransaction(int isProcessed, int numberOfTrans, int numberOfBatch, bool isReadyDbExtra)
        {
            var criteria = new GetRawTransactionCriteria
            {
                IsProcessed = isProcessed,
                NumberOfTickets = numberOfTrans,
                NumberOfBatch = numberOfBatch,
                IsReadyDbExtra = isReadyDbExtra
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<RawTransactionEntity>(criteria);
            }
        }

        public void InsertTransaction(long fromId, long toId, bool isReadyDbExtra)
        {
            var criteria = new InsertTransactionCriteria
            {
                FromStagingTransId = fromId,
                ToStagingTransId = toId,
                IsReadyDbExtra = isReadyDbExtra
            };

            repository.Execute(criteria);
        }

        public void InserUserAgent(long fromId, long toId, bool isReadyDbExtra)
        {
            var criteria = new InsertUserAgentCriteria
            {
                FromStagingTransId = fromId,
                ToStagingTransId = toId,
                IsReadyDbExtra = isReadyDbExtra
            };

            repository.Execute(criteria);
        }

        public void UnhandleSubscriberActionProcess(long fromId, long toId, bool isReadyDbExtra)
        {
            var criteria = new UnhandleSubscriberActionCriteria
            {
                FromTransId = fromId,
                ToTransId = toId,
                IsReadyDbExtra = isReadyDbExtra
            };

            repository.Execute(criteria);
        }
    }
}
