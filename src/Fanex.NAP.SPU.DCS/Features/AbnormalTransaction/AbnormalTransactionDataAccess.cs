using System.Collections.Generic;
using Fanex.Data;
using Fanex.Data.Repository;
using Fanex.NAP.SPU.DCS.Features.AbnormalTransaction.Criteria;
using Fanex.NAP.SPU.DCS.Features.AbnormalTransaction.Entities;
using Fanex.NAP.SPU.DCS.Features.AbnormalTransaction.Model;

namespace Fanex.NAP.SPU.DCS.Features.AbnormalTransaction
{
    public interface IAbnormalTransactionDataAccess
    {
        (IEnumerable<AbnormalTransactionEntity>, RangeIDModel) GetAbnormalTransactionsAsync(int size);

        void UpdateAbnormalTransactions(string transJson, ulong minId, ulong maxId);
    }

    public class AbnormalTransactionDataAccess : IAbnormalTransactionDataAccess
    {
        private readonly IDynamicRepository repository;

        public AbnormalTransactionDataAccess(IDynamicRepository repository)
        {
            this.repository = repository ?? new DynamicRepository();
        }

        public (IEnumerable<AbnormalTransactionEntity>, RangeIDModel) GetAbnormalTransactionsAsync(int size)
        {
            var criteria = new AbnormalTransactionGetCriteria
            {
                Size = size,

                MinID = 0,
                MaxID = 0
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var result = db.Query<AbnormalTransactionEntity>(criteria);
                var rangeID = new RangeIDModel
                {
                    MinID = criteria.MinID,
                    MaxID = criteria.MaxID
                };
                return (result, rangeID);
            }
        }

        public void UpdateAbnormalTransactions(string transJson, ulong minId, ulong maxId)
        {
            var criteria = new AbnormalTransactionUpdateCriteria
            {
                TransJson = transJson,
                MinID = minId,
                MaxID = maxId
            };

            repository.Execute(criteria);
        }
    }
}
