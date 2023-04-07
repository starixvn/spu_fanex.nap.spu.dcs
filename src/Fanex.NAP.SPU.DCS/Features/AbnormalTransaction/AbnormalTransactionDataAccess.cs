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

        void UpdateAbnormalTransactionsAsync(string transJson, decimal maxId);

        (IEnumerable<AbnormalTransactionEntity>, IEnumerable<decimal>) GetAbnormalTransactionsAsync(int size);
    }

    public class AbnormalTransactionDataAccess : IAbnormalTransactionDataAccess
    {
        private readonly IDynamicRepository repository;

        public AbnormalTransactionDataAccess()
        {
            repository = new DynamicRepository();
        }

        public void TransformTrans07ToAbnormalTransaction(int size)
        {
            var criteria = new AbnormalTransasctionTransformCriteria
            {
                Size = size
            };

            repository.ExecuteAsync(criteria);
        }

        public void UpdateAbnormalTransactionsAsync(string transJson, decimal maxID)
        {
            var criteria = new AbnormalTransasctionUpdateCriteria
            {
                TransJson = transJson,
                MaxID = maxID
            };

            repository.ExecuteAsync(criteria);
        }

        public (IEnumerable<AbnormalTransactionEntity>, IEnumerable<decimal>) GetAbnormalTransactionsAsync(int size)
        {
            var criteria = new AbnormalTransasctionGetCriteria
            {
                Size = size
            };

            return repository.FetchMultiple<AbnormalTransactionEntity, decimal>(criteria);
        }
    }
}
