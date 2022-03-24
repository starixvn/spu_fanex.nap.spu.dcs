namespace Fanex.NAP.SPU.DCS.Features.Monitoring.Entities
{
    using System;
  
    public class AccountIPEntity
    {
        public UInt64 AccountID { get; set; }

        public DateTime TransDate { get; set; }

        public string IP { get; set; }

        public UInt64 MaxTransID { get; set; }
    }
}
