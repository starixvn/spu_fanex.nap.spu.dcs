using Fanex.Data;
using Fanex.Data.Repository;
using Fanex.NAP.SPU.DCS.Features.AbnormalTransaction.Criteria;
using Fanex.NAP.SPU.DCS.Features.AbnormalTransaction.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fanex.NAP.SPU.DCS.Features.AbnormalTransaction
{
    public interface IAbnormalTransactionDataAccess
    {
        void TransformTrans07ToAbnormalTransaction(int size);

        void UpdateAbnormalTransactionsAsync(string transJson);

        IEnumerable<AbnormalTransactionEntity> GetAbnormalTransactionsAsync(int size);
    }

    public class AbnormalTransactionDataAccess : IAbnormalTransactionDataAccess
    {
        public void TransformTrans07ToAbnormalTransaction(int size)
        {
            var repository = new DynamicRepository();

            var criteria = new AbnormalTransasctionTransformCriteria
            {
                Size = size
            };

            repository.ExecuteAsync(criteria);
        }

        public void UpdateAbnormalTransactionsAsync(string transJson)
        {
            var repository = new DynamicRepository();

            var criteria = new AbnormalTransasctionUpdateCriteria
            {
                TransJson = transJson
            };

            repository.ExecuteAsync(criteria);
        }

        public IEnumerable<AbnormalTransactionEntity> GetAbnormalTransactionsAsync(int size)
        {
            var criteria = new AbnormalTransasctionGetCriteria
            {
                Size = size
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<AbnormalTransactionEntity>(criteria);
            }
        }
    }
}
