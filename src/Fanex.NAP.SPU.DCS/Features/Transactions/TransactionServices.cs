namespace Fanex.NAP.SPU.DCS.Features.Transactions
{
    using Fanex.NAP.SPU.DCS.Features.Transactions.Entities;
    using System.Collections.Generic;

    public interface ITransactionServices
    {
        
        IEnumerable<RawTransactionEntity> GetRawTransaction(int isProcessed, int numberOfTrans, int numberOfBatch, bool isReadyDbExtra);

        ITransactionServices UserAgentProcessing(long fromId, long toId, bool isReadyDbExtra);

        ITransactionServices TransactionProcessing(long fromId, long toId, bool isReadyDbExtra);

        ITransactionServices UnhandleSubscriberAccountProcessing(long fromId, long toId, bool isReadyDbExtra);
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

        public ITransactionServices UnhandleSubscriberAccountProcessing(long fromId, long toId, bool isReadyDbExtra)
        {
            this.transactionDataAccess.UnhandleSubscriberActionProcess(fromId, toId, isReadyDbExtra);

            return this;
        }

        public ITransactionServices UserAgentProcessing(long fromId, long toId, bool isReadyDbExtra)
        {
            this.transactionDataAccess.InserUserAgent(fromId, toId, isReadyDbExtra);

            return this;
        }

        
    }
}
