using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.NAP.SPU.DCS.Features.DataTrace.Criteria
{
    internal class GetMBTransOpenSearchCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_BatchSize", AllowNull = true)]
        public int BatchSize { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DC_OpenSearch_MBTransaction_Get";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
