namespace Fanex.NAP.SPU.DCS.Features.Monitoring.Model
{
    using Fanex.NAP.SPU.DCS.Features.Monitoring.Entities;    
    using System.Collections.Generic;

    public class SumTransactionModel
    {
        public SumTransactionModel()
        {
            SumTransactionList = new List<SumTransactionEntity>();
        }

        public long MaxTransID { get; set; }

        public IEnumerable<SumTransactionEntity> SumTransactionList { get; set; }
    }
}
