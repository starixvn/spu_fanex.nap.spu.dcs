using Fanex.NAP.SPU.DCS.Features.Transactions.Entities;
using System.Collections.Generic;

namespace Fanex.NAP.SPU.DCS.Features.Transactions
{
    public interface ITransactionServices
    {
        
        IEnumerable<RawTransactionEntity> GetRawTransaction(int isProcessed, int numberOfTrans, int numberOfBatch);

        ITransactionServices UserAgentProcessing(long fromId, long toId);

        ITransactionServices TransactionProcessing(long fromId, long toId);

        ITransactionServices UnhandleSubscriberAccountProcessing(long fromId, long toId);
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

        public ITransactionServices UnhandleSubscriberAccountProcessing(long fromId, long toId)
        {
            this.transactionDataAccess.UnhandleSubscriberActionProcess(fromId, toId);

            return this;
        }

        public ITransactionServices UserAgentProcessing(long fromId, long toId)
        {
            this.transactionDataAccess.InserUserAgent(fromId, toId);

            return this;
        }

        
    }
}
