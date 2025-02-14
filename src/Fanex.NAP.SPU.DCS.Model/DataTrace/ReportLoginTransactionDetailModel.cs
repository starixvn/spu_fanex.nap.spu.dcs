using System;

namespace Fanex.NAP.SPU.DCS.Features.DataTrace.Model
{
    public class ReportLoginTransactionDetailModel
    {
        public DateTime TransDate { get; set; }

        public int SubscriberID { get; set; }

        public string SubscriberName { get; set; }

        public ulong AccountID { get; set; }

        public uint CustID { get; set; }

        public string Site { get; set; }

        public string Currency { get; set; }

        public string CustomerClass { get; set; }

        public string CustomerClassName { get; set; }

        public string Flagged { get; set; }

        public int TotalTrans { get; set; }
    }
}

