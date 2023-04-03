using Fanex.Data;
using Fanex.Data.Repository;
using Fanex.NAP.SPU.DCS.Features.AbnormalTransaction.Criteria;
using Fanex.NAP.SPU.DCS.Features.AbnormalTransaction.Entities;
using System.Collections.Generic;

namespace Fanex.NAP.SPU.DCS.Features.AbnormalTransaction
{
    public interface IAbnormalTransactionDataAccess
    {
        void UpdateAbnormalTransasction(string transJson);

        IEnumerable<AbnormalTransactionEntity> GetAbnormalTransasction(int size);

        void TransformAbnormalTransasction(int size);
    }

    public class AbnormalTransactionDataAccess : IAbnormalTransactionDataAccess
    {
        public void UpdateAbnormalTransasction(string transJson)
        {
            var repository = new DynamicRepository();

            var criteria = new AbnormalTransasctionUpdateCriteria
            {
                TransJson = transJson
            };

            repository.Execute(criteria);
        }

        public IEnumerable<AbnormalTransactionEntity> GetAbnormalTransasction(int size)
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

        public void TransformAbnormalTransasction(int size)
        {
            var repository = new DynamicRepository();

            var criteria = new AbnormalTransasctionTransformCriteria
            {
                Size = size
            };

            repository.Execute(criteria);
        }
    }
}
