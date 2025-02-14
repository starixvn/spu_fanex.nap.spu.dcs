using System;

namespace Fanex.NAP.SPU.DCS.Features.DataTrace.Model
{
    public class ReportLoginTransactionByAccountModel
    {
        public DateTime TransDate { get; set; }

        public int SubscriberID { get; set; }

        public string SubscriberName { get; set; }

        public ulong AccountID { get; set; }

        public ulong CustID { get; set; }

        public string LoginName { get; set; }

        public string Site { get; set; }

        public string PaymentType { get; set; }

        public string LicenseeType { get; set; }

        public int TotalTrans { get; set; }

        public int TotalInvalidBrowers { get; set; }

        public int TotalBotTrans { get; set; }

        public int TotalNoDeviceTrans { get; set; }
    }
}