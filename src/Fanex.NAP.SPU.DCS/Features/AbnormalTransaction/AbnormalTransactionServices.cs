using Fanex.NAP.SPU.DCS.Features.AbnormalTransaction.Entities;
using System.Collections.Generic;

namespace Fanex.NAP.SPU.DCS.Features.AbnormalTransaction
{
    public interface IAbnormalTransactionJobServices
    {
        void Update(string transJson);

        IEnumerable<AbnormalTransactionEntity> GetAbnormalTransasction(int size);

        void TransformAbnormalTransasction(int size);
    }

    public class AbnormalTransactionJobServices : IAbnormalTransactionJobServices
    {
        private readonly IAbnormalTransactionDataAccess dataAcess;

        public AbnormalTransactionJobServices(IAbnormalTransactionDataAccess dataAcess)
        {
            this.dataAcess = dataAcess;
        }

        public void Update(string transJson)
        {
            if (!string.IsNullOrEmpty(transJson))
            {
                dataAcess.UpdateAbnormalTransasction(transJson);
            }
        }

        public IEnumerable<AbnormalTransactionEntity> GetAbnormalTransasction(int size)
        {
            if (size == 0)
            {
                return new List<AbnormalTransactionEntity>();
            }

            return dataAcess.GetAbnormalTransasction(size);
        }

        public void TransformAbnormalTransasction(int size)
        {
            if (size > 0)
            {
                dataAcess.TransformAbnormalTransasction(size);
            }
        }
    }
}
