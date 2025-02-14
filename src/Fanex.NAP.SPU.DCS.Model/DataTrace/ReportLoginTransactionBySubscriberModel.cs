using System;

namespace Fanex.NAP.SPU.DCS.Features.DataTrace.Model
{
    public class ReportLoginTransactionBySubscriberModel
    {
        public DateTime TransDate { get; set; }

        public int SubscriberID { get; set; }

        public string SubscriberName { get; set; }

        public int TotalTrans { get; set; }

        public int TotalInvalidBrowers { get; set; }

        public int TotalBotTrans { get; set; }

        public int TotalNoDeviceTrans { get; set; }
    }
}