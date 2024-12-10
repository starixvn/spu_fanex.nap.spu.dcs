using Fanex.Data;
using Fanex.Data.Repository;
using Fanex.NAP.SPU.DCS.Features.MBTransactions.Criteria;
using Fanex.NAP.SPU.DCS.Features.MBTransactions.Entities;
using System.Collections.Generic;

namespace Fanex.NAP.SPU.DCS.Features.MBTransactions
{
    public interface IMBTransactionDataAccess
    {
        IEnumerable<MBRawTransactionEntity> GetMBRawTransaction(int numberOfTrans, int numberOfBatch);

        void InsertMBTransaction(ulong fromID, ulong toID);
    }

    public class MBTransactionDataAccess : IMBTransactionDataAccess
    {
        private readonly IDynamicRepository repository;

        public MBTransactionDataAccess()
        {
            repository = new DynamicRepository();
        }

        public IEnumerable<MBRawTransactionEntity> GetMBRawTransaction(int numberOfTrans, int numberOfBatch)
        {
            var criteria = new GetMBRawTransactionCriteria
            {
                NumberOfTickets = numberOfTrans,
                NumberOfBatch = numberOfBatch
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<MBRawTransactionEntity>(criteria);
            }
        }

        public void InsertMBTransaction(ulong fromID, ulong toID)
        {
            var criteria = new InsertMBTransactionCriteria
            {
                FromTransID = fromID,
                ToTransID = toID
            };

            repository.Execute(criteria);
        }
    }
}
