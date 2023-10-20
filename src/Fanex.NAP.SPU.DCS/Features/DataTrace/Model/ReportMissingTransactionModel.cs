using System;

namespace Fanex.NAP.SPU.DCS.Features.DataTrace.Model
{
    public class ReportMissingTransactionModel
    {
        public DateTime TransDate { get; set; }

        public int SubscriberID { get; set; }

        public string SubscriberName { get; set; }

        public int TotalTrans { get; set; }

        public int CountCustWithoutTrans { get; set; }

        public int CountCustMissingTrans { get; set; }
    }
}

