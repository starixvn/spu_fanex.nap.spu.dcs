namespace Fanex.NAP.SPU.DCS.Features.DataTrace.Criteria
{
    using Fanex.Data.Repository;
    using Fanex.Data;
    public class InsertSummaryBetTicketCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_BetTicketJson")]
        public string RawTransJson { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DT_Summary_BetTicket_Insert";
        }

        public override bool IsValid() => !string.IsNullOrEmpty(RawTransJson);
    }
}
