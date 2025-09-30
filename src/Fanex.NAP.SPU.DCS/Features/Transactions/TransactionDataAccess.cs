namespace Fanex.NAP.SPU.DCS.Features.Transactions
{
    using System.Collections.Generic;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.DCS.Features.Transactions.Criteria;
    using Fanex.NAP.SPU.DCS.Features.Transactions.Entities;

    public interface ITransactionDataAccess
    {
        IEnumerable<RawTransactionEntity> GetRawTransaction(int isProcessed, int numberOfTrans, int numberOfBatch);

        void InserUserAgent(long fromId, long toId);

        void InsertTransaction(long fromId, long toId);

        void ProcessRawFingerprintQueue(int batchSize);
    }
    public class TransactionDataAccess : ITransactionDataAccess
    {
        private readonly IDynamicRepository repository;

        public TransactionDataAccess(IDynamicRepository repository)
        {
            this.repository = repository ?? new DynamicRepository();
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

        public void ProcessRawFingerprintQueue(int batchSize)
        {
            var criteria = new ProcessRawFingerprintQueueCriteria
            {
                BatchSize = batchSize
            };

            repository.Execute(criteria);
        }
    }
}
