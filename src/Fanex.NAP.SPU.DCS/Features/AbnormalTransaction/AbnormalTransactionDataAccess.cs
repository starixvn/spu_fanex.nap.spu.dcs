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
        Task TransformTrans07ToAbnormalTransaction(int size);

        Task UpdateAbnormalTransactionsAsync(string transJson);

        Task<IEnumerable<AbnormalTransactionEntity>> GetAbnormalTransactionsAsync(int size);
    }

    public class AbnormalTransactionDataAccess : IAbnormalTransactionDataAccess
    {
        public async Task TransformTrans07ToAbnormalTransaction(int size)
        {
            var repository = new DynamicRepository();

            var criteria = new AbnormalTransasctionTransformCriteria
            {
                Size = size
            };

            await repository.ExecuteAsync(criteria);
        }

        public async Task UpdateAbnormalTransactionsAsync(string transJson)
        {
            var repository = new DynamicRepository();

            var criteria = new AbnormalTransasctionUpdateCriteria
            {
                TransJson = transJson
            };

            await repository.ExecuteAsync(criteria);
        }

        public async Task<IEnumerable<AbnormalTransactionEntity>> GetAbnormalTransactionsAsync(int size)
        {
            var criteria = new AbnormalTransasctionGetCriteria
            {
                Size = size
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return await db.QueryAsync<AbnormalTransactionEntity>(criteria);
            }
        }
    }
}
