namespace Fanex.NAP.SPU.DCS.Features.Monitoring.Entities
{
    using System;

    public class SumSubscriberEntity
    {
        public DateTime TransDate { get; set; }

        public int SubscriberID { get; set; }

        public int ValidTotal { get; set; }

        public int ValidNoDevice { get; set; }

        public int ValidDeviceStatusNew { get; set; }

        public int ValidDeviceStatusOld { get; set; }

        public int ValidDeviceStatusRecover { get; set; }

        public int ValidBrowserless { get; set; }
    }
}
