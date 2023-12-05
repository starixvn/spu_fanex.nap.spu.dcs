namespace Fanex.NAP.SPU.DCS.Features.Monitoring.Criteria
{
    using System;
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetSumTransactionCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "op_MaxTransID")]
        public UInt64 MaxTransID { get; set; }

        [SpParam(SqlParamName = "op_DateScan")]
        public DateTime DateScan { get; set; }

        [SpParam(SqlParamName = "op_AccountID")]
        public UInt64 AccountID { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DC_SumTransaction_Get";
        }

        public override bool IsValid()
        {
            return true;
        }

    }
}
