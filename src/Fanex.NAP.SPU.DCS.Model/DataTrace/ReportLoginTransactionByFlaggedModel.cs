using System;

namespace Fanex.NAP.SPU.DCS.Features.DataTrace.Model
{
    public class ReportLoginTransactionByFlaggedModel
    {
        public DateTime TransDate { get; set; }

        public int Flagged { get; set; }

        public string FlaggedName { get; set; }

        public int TotalTrans { get; set; }
    }
}