namespace Fanex.NAP.SPU.DCS.Features.Monitoring.Entities
{
    using System;

    public class SumDeviceTypeEntity
    {
        public UInt64 AccountID { get; set; }

        public DateTime TransDate { get; set; }

        public int DeviceMobile { get; set; }

        public int DeviceDesktop { get; set; }

        public int DeviceOthers { get; set; }

        public int TotalLogin { get; set; }

        public UInt64 MaxTransID { get; set; }
    }
}
