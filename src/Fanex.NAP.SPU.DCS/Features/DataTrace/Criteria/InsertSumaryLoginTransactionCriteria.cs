namespace Fanex.NAP.SPU.DCS.Features.DataTrace.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    public class InsertSumaryLoginTransactionCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_Size")]
        public int BatchSize { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DT_Summary_LoginTransaction_Insert";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
