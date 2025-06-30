using System;
using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.NAP.SPU.DCS.Features.DataTrace.Criteria
{
    public class GetMissingBetTransactionPackageCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_TransDate", AllowNull = true)]
        public DateTime TransDate { get; set; }

        [SpParam(SqlParamName = "ip_BatchSize", AllowNull = true)]
        public int BatchSize { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DT_Summary_MissingBetTransaction_GetPackage";
        }

        public override bool IsValid() => BatchSize > 0;
    }
}
