using Fanex.NAP.SPU.DCS.Features.MBTransactions.Entities;
using System.Collections.Generic;

namespace Fanex.NAP.SPU.DCS.Features.MBTransactions
{
    public interface IMBTransactionServices
    {

        IEnumerable<MBRawTransactionEntity> GetRawTransaction(int numberOfTrans, int numberOfBatch);

        IMBTransactionServices TransactionProcessing(long fromId, long toId);
    }

    public class MBTransactionServices : IMBTransactionServices
    {
        private readonly IMBTransactionDataAccess transactionDataAccess;

        public MBTransactionServices(IMBTransactionDataAccess transactionDataAccess)
        {
            this.transactionDataAccess = transactionDataAccess;
        }

        public IEnumerable<MBRawTransactionEntity> GetRawTransaction(int numberOfTrans, int numberOfBatch)
        {
            return this.transactionDataAccess.GetRawTransaction(numberOfTrans, numberOfBatch);
        }

        public IMBTransactionServices TransactionProcessing(long fromId, long toId)
        {
            this.transactionDataAccess.InsertTransaction(fromId, toId);
            return this;
        }
    }
}
