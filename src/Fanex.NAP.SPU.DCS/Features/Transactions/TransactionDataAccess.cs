using Fanex.Data;
using Fanex.Data.Repository;
using Fanex.NAP.SPU.DCS.Features.Transactions.Criteria;
using Fanex.NAP.SPU.DCS.Features.Transactions.Entities;
using System.Collections.Generic;

namespace Fanex.NAP.SPU.DCS.Features.Transactions
{
    public interface ITransactionDataAccess
    {
        IEnumerable<RawTransactionEntity> GetRawTransaction(int isProcessed, int numberOfTrans, int numberOfBatch);

        void InserUserAgent(long fromId, long toId);

        void InsertTransaction(long fromId, long toId);

        void UnhandleSubscriberActionProcess(long fromId, long toId);
    }
    public class TransactionDataAccess : ITransactionDataAccess
    {
        private readonly IDynamicRepository repository;

        public TransactionDataAccess()
        {
            repository = new DynamicRepository();
        }

        public IEnumerable<RawTransactionEntity> GetRawTransaction(int isProcessed, int numberOfTrans, int numberOfBatch)
        {
            var criteria = new GetRawTransactionCriteria
            {
                IsProcessed = isProcessed,
                NumberOfTickets = numberOfTrans,
                NumberOfBatch = numberOfBatch
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<RawTransactionEntity>(criteria);
            }
        }

        public void InsertTransaction(long fromId, long toId)
        {
            var criteria = new InsertTransactionCriteria
            {
                FromStagingTransId = fromId,
                ToStagingTransId = toId
            };

            repository.Execute(criteria);
        }

        public void InserUserAgent(long fromId, long toId)
        {
            var criteria = new InsertUserAgentCriteria
            {
                FromStagingTransId = fromId,
                ToStagingTransId = toId
            };

            repository.Execute(criteria);
        }

        public void UnhandleSubscriberActionProcess(long fromId, long toId)
        {
            var criteria = new UnhandleSubscriberActionCriteria
            {
                FromTransId = fromId,
                ToTransId = toId
            };

            repository.Execute(criteria);
        }
    }
}
