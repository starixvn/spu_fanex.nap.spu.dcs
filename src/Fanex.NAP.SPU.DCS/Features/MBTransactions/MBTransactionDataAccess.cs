using Fanex.Data;
using Fanex.Data.Repository;
using Fanex.NAP.SPU.DCS.Features.MBTransactions.Criteria;
using Fanex.NAP.SPU.DCS.Features.MBTransactions.Entities;
using Fanex.NAP.SPU.DCS.Features.Transactions.Criteria;
using System.Collections.Generic;

namespace Fanex.NAP.SPU.DCS.Features.MBTransactions
{
    public interface IMBTransactionDataAccess
    {
        IEnumerable<MBRawTransactionEntity> GetRawTransaction(int numberOfTrans, int numberOfBatch);

        void InsertTransaction(long fromId, long toId);
    }

    public class MBTransactionDataAccess : IMBTransactionDataAccess
    {
        private readonly IDynamicRepository repository;

        public MBTransactionDataAccess()
        {
            repository = new DynamicRepository();
        }

        public IEnumerable<MBRawTransactionEntity> GetRawTransaction(int numberOfTrans, int numberOfBatch)
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

        public void InsertTransaction(long fromId, long toId)
        {
            var criteria = new InsertMBTransactionCriteria
            {
                FromStagingTransId = fromId,
                ToStagingTransId = toId
            };

            repository.Execute(criteria);
        }
    }
}
