using Fanex.Data.Repository;
using Fanex.NAP.SPU.DCS.Features.AbnormalTransaction.Criteria;
using Fanex.NAP.SPU.DCS.Features.AbnormalTransaction.Entities;
using Fanex.NAP.SPU.DCS.Features.AbnormalTransaction.Model;
using System.Collections.Generic;

namespace Fanex.NAP.SPU.DCS.Features.AbnormalTransaction
{
    public interface IAbnormalTransactionDataAccess
    {
        (IEnumerable<AbnormalTransactionEntity>, IEnumerable<TransIDMinMaxModel>) GetAbnormalTransactionsAsync(int size);

        void UpdateAbnormalTransactionsAsync(string transJson, ulong minId, ulong maxId);
    }

    public class AbnormalTransactionDataAccess : IAbnormalTransactionDataAccess
    {
        private readonly IDynamicRepository repository;

        public AbnormalTransactionDataAccess()
        {
            repository = new DynamicRepository();
        }

        public (IEnumerable<AbnormalTransactionEntity>, IEnumerable<TransIDMinMaxModel>) GetAbnormalTransactionsAsync(int size)
        {
            var criteria = new AbnormalTransactionGetCriteria
            {
                Size = size
            };

            return repository.FetchMultiple<AbnormalTransactionEntity, TransIDMinMaxModel>(criteria);
        }

        public void UpdateAbnormalTransactionsAsync(string transJson, ulong minId, ulong maxId)
        {
            var criteria = new AbnormalTransactionUpdateCriteria
            {
                TransJson = transJson,
                MinID = minId,
                MaxID = maxId
            };

            repository.ExecuteAsync(criteria);
        }
    }
}
