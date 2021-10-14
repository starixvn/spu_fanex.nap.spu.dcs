namespace Fanex.NAP.SPU.DCS.Features.Monitoring.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class UpdateSumTransactionCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_MaxTransID")]
        public long MaxTransID { get; set; }

        [SpParam(SqlParamName = "ip_SumJson")]
        public string SumTransJson { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DC_SumTransaction_Update";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
