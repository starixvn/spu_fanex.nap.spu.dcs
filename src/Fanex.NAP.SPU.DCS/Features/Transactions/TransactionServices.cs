namespace Fanex.NAP.SPU.DCS.Features.Transactions
{
    using System.Collections.Generic;
    using Fanex.NAP.SPU.DCS.Features.Transactions.Entities;

    public interface ITransactionServices
    {

        IEnumerable<RawTransactionEntity> GetRawTransaction(int isProcessed, int numberOfTrans, int numberOfBatch);

        ITransactionServices UserAgentProcessing(long fromId, long toId);

        ITransactionServices TransactionProcessing(long fromId, long toId);

        ITransactionServices ProcessRawFingerprintQueue(int batchSize);
    }

    public class TransactionServices : ITransactionServices
    {
        private readonly ITransactionDataAccess transactionDataAccess;

        public TransactionServices(ITransactionDataAccess transactionDataAccess)
        {
            this.transactionDataAccess = transactionDataAccess;
        }

        public IEnumerable<RawTransactionEntity> GetRawTransaction(int isProcessed, int numberOfTrans, int numberOfBatch)
        {
            return this.transactionDataAccess.GetRawTransaction(isProcessed, numberOfTrans, numberOfBatch);
        }

        public ITransactionServices TransactionProcessing(long fromId, long toId)
        {
            this.transactionDataAccess.InsertTransaction(fromId, toId);
            return this;
        }

        public ITransactionServices UserAgentProcessing(long fromId, long toId)
        {
            this.transactionDataAccess.InserUserAgent(fromId, toId);
            return this;
        }

        public ITransactionServices ProcessRawFingerprintQueue(int batchSize)
        {
            this.transactionDataAccess.ProcessRawFingerprintQueue(batchSize);
            return this;
        }
    }
}
