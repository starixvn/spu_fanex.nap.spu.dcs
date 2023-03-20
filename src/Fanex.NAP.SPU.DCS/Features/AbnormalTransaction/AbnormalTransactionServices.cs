using Fanex.NAP.SPU.DCS.Features.AbnormalTransaction.Entities;
using System;
using System.Collections.Generic;

namespace Fanex.NAP.SPU.DCS.Features.AbnormalTransaction
{
    public interface IAbnormalTransactionJobServices
    {
        IEnumerable<AbnormalTransactionEntity> GetAbnormalTransasction(int size);
    }

    public class AbnormalTransactionJobServices : IAbnormalTransactionJobServices
    {
        private readonly IAbnormalTransactionDataAccess dataAcess;

        public AbnormalTransactionJobServices(IAbnormalTransactionDataAccess dataAcess)
        {
            this.dataAcess = dataAcess;
        }

        public IEnumerable<AbnormalTransactionEntity> GetAbnormalTransasction(int size)
        {
            if (size == 0)
            {
                return new List<AbnormalTransactionEntity>();
            }

            return this.dataAcess.GetAbnormalTransasction(size);
        }
    }
}
