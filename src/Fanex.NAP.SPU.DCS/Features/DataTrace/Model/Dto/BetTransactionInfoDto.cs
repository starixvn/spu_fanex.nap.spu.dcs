using System;

namespace Fanex.NAP.SPU.DCS.Features.DataTrace.Model.Dto
{
    internal class BetTransactionInfoDto
    {
        public DateTime TransDate { get; set; }

        public int CustID { get; set; }

        public int BetCount { get; set; }

        public DateTime FirstBetTransactionTime { get; set; }

        public DateTime LastBetTransactionTime { get; set; }
    }
}
