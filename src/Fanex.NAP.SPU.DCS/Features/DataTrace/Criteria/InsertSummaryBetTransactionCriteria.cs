namespace Fanex.NAP.SPU.DCS.Features.DataTrace.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    public class InsertSummaryBetTransactionCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_BetTransactionJson", AllowNull = true)]
        public string RawTransJson { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DT_Summary_BetTransaction_Insert";
        }

        public override bool IsValid() => !string.IsNullOrEmpty(RawTransJson);
    }
}
