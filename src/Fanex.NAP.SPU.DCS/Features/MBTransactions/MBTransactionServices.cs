using Fanex.NAP.SPU.DCS.Features.MBTransactions.Entities;
using System.Collections.Generic;

namespace Fanex.NAP.SPU.DCS.Features.MBTransactions
{
    public interface IMBTransactionServices
    {

        IEnumerable<MBRawTransactionEntity> GetMBRawTransaction(int numberOfTrans, int numberOfBatch);

        void InsertMBTransaction(ulong fromID, ulong toID);
    }

    public class MBTransactionServices : IMBTransactionServices
    {
        private readonly IMBTransactionDataAccess transactionDataAccess;

        public MBTransactionServices(IMBTransactionDataAccess transactionDataAccess)
        {
            this.transactionDataAccess = transactionDataAccess;
        }

        public IEnumerable<MBRawTransactionEntity> GetMBRawTransaction(int numberOfTrans, int numberOfBatch)
        {
            return transactionDataAccess.GetMBRawTransaction(numberOfTrans, numberOfBatch);
        }

        public void InsertMBTransaction(ulong fromID, ulong toID)
        {
            transactionDataAccess.InsertMBTransaction(fromID, toID);
        }
    }
}
