namespace Fanex.NAP.SPU.DCS.Features.DataTrace.Criteria
{
    using Fanex.Data.Repository;
    using Fanex.Data;
    internal class InsertDTSummaryMissingLoginTransBetTicketCriteria : NonQueryCommand
    {

        public override string GetSettingKey()
        {
            return "DCS_DT_Summary_MissingLoginTrans_BetTicket_Insert";
        }

        public override bool IsValid() => true;
    }
}
