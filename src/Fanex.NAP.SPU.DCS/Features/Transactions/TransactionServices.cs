using Fanex.NAP.SPU.DCS.Features.Transactions.Entities;
using System.Collections.Generic;

namespace Fanex.NAP.SPU.DCS.Features.Transactions
{
    public interface ITransactionServices
    {
        
        IEnumerable<RawTransactionEntity> GetRawTransaction(int isProcessed, int numberOfTrans, int numberOfBatch);

        ITransactionServices Init(long fromId, long toId);

        ITransactionServices UserAgentProcessing();

        ITransactionServices TransactionProcessing();

        ITransactionServices UnhandleSubscriberAccountProcessing();
    }

    public class TransactionServices : ITransactionServices
    {
        private readonly ITransactionDataAccess transactionDataAccess;

        private long fromId;

        private long toId;

        public TransactionServices(ITransactionDataAccess transactionDataAccess)
        {
            this.transactionDataAccess = transactionDataAccess;
        }

        public IEnumerable<RawTransactionEntity> GetRawTransaction(int isProcessed, int numberOfTrans, int numberOfBatch)
        {
            return this.transactionDataAccess.GetRawTransaction(isProcessed, numberOfTrans, numberOfBatch);
        }

        public ITransactionServices Init(long fromId, long toId)
        {
            this.fromId = fromId;
            this.toId = toId;

            return this;
        }

        public ITransactionServices TransactionProcessing()
        {
            this.transactionDataAccess.InsertTransaction(fromId, toId);

            return this;
        }

        public ITransactionServices UnhandleSubscriberAccountProcessing()
        {
            this.transactionDataAccess.UnhandleSubscriberActionProcess(fromId, toId);

            return this;
        }

        public ITransactionServices UserAgentProcessing()
        {
            this.transactionDataAccess.InserUserAgent(fromId, toId);

            return this;
        }

        
    }
}
