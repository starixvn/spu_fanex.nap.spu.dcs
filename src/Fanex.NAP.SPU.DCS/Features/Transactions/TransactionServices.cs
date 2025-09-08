namespace Fanex.NAP.SPU.DCS.Features.Transactions
{
    using System.Collections.Generic;
    using Fanex.NAP.SPU.DCS.Features.Transactions.Entities;

    public interface ITransactionServices
    {

        IEnumerable<RawTransactionEntity> GetRawTransaction(int isProcessed, int numberOfTrans, int numberOfBatch, bool isReadyDbExtra);

        ITransactionServices UserAgentProcessing(long fromId, long toId, bool isReadyDbExtra);

        ITransactionServices TransactionProcessing(long fromId, long toId, bool isReadyDbExtra);

        ITransactionServices ProcessRawFingerprintQueue(int batchSize);
    }

    public class TransactionServices : ITransactionServices
    {
        private readonly ITransactionDataAccess transactionDataAccess;

        public TransactionServices(ITransactionDataAccess transactionDataAccess)
        {
            this.transactionDataAccess = transactionDataAccess;
        }

        public IEnumerable<RawTransactionEntity> GetRawTransaction(int isProcessed, int numberOfTrans, int numberOfBatch, bool isReadyDbExtra)
        {
            return this.transactionDataAccess.GetRawTransaction(isProcessed, numberOfTrans, numberOfBatch, isReadyDbExtra);
        }

        public ITransactionServices TransactionProcessing(long fromId, long toId, bool isReadyDbExtra)
        {
            this.transactionDataAccess.InsertTransaction(fromId, toId, isReadyDbExtra);
            return this;
        }

        public ITransactionServices UserAgentProcessing(long fromId, long toId, bool isReadyDbExtra)
        {
            this.transactionDataAccess.InserUserAgent(fromId, toId, isReadyDbExtra);
            return this;
        }

        public ITransactionServices ProcessRawFingerprintQueue(int batchSize)
        {
            this.transactionDataAccess.ProcessRawFingerprintQueue(batchSize);
            return this;
        }
    }
}
