namespace Fanex.NAP.SPU.DCS.Features.Monitoring.Entities
{
    using System;

    public class AccountDeviceEntity
    {
        public UInt64 AccountID { get; set; }

        public DateTime TransDate { get; set; }

        public long DeviceID { get; set; }

        public UInt64 MaxTransID { get; set; }
    }
}
