using System;

namespace Fanex.NAP.SPU.DCS.Features.DataTrace.Model
{
    public class ReportBotDTransactionModel
    {
        public DateTime TransDate { get; set; }

        public int SubscriberID { get; set; }

        public string SubscriberName { get; set; }

        public string Flagged { get; set; }

        public int TotalTrans { get; set; }
    }
}

