namespace Fanex.NAP.SPU.DCS.Features.Monitoring.Entities
{
    using System;   

    public class SumDeviceTypeEntity
    {
        public long AccountID { get; set; }

        public DateTime TransDate { get; set; }

        public int DeviceMobile { get; set; }

        public int DeviceDesktop { get; set; }

        public int DeviceOthers { get; set; }

        public int TotalLogin { get; set; }
    }
}
