namespace Fanex.NAP.SPU.DCS.Features.DataTrace.Model
{
    using System;
    public class BetTransactionInfo
    {
        public DateTime TransDate { get; set; }

        public int CustID { get; set; }

        public int BetCount { get; set; }

        public DateTime FirstBetTicketTime { get; set; }

        public DateTime LastBetTicketTime { get; set; }
    }
}
