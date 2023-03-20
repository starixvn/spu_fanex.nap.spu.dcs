using Fanex.Data;
using Fanex.NAP.SPU.DCS.Features.AbnormalTransaction.Criteria;
using Fanex.NAP.SPU.DCS.Features.AbnormalTransaction.Entities;
using System.Collections.Generic;

namespace Fanex.NAP.SPU.DCS.Features.AbnormalTransaction
{
    public interface IAbnormalTransactionDataAccess
    {
        IEnumerable<AbnormalTransactionEntity> GetAbnormalTransasction(int size);
    }

    public class AbnormalTransactionDataAccess : IAbnormalTransactionDataAccess
    {
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
    }
}
