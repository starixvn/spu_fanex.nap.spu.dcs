using System;

namespace Fanex.NAP.SPU.DCS.Features.DCSExtra.Model
{
    public class TransactionInitialDataModel
    {
        public ulong TransID { get; set; }

        public decimal? IPID { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
