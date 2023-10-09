namespace Fanex.NAP.SPU.DCS.Features.DataTrace.Criteria
{
    using Fanex.Data.Repository;
    using Fanex.Data;
    public class InsertSummaryMissingLoginTransactionCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_FromID")]
        public int MinID { get; set; }

        [SpParam(SqlParamName = "ip_ToID")]
        public int MaxID { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DT_Summary_MissingLoginTransaction_Insert";
        }

        public override bool IsValid() => MaxID >= MinID;
    }
}
