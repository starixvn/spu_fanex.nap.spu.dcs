namespace Fanex.NAP.SPU.DCS.Features.DataTrace.Criteria
{
    using Fanex.Data.Repository;
    using Fanex.Data;
    using System;

    public class GetMissingLoginTransactionPackageCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_TransDate")]
        public DateTime TransDate { get; set; }

        [SpParam(SqlParamName = "ip_BatchSize")]
        public int BatchSize { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DT_Summary_MissingLoginTransaction_GetPackage";
        }

        public override bool IsValid() => BatchSize > 0;
    }
}
