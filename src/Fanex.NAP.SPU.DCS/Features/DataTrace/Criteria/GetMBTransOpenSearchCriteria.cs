using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.NAP.SPU.DCS.Features.DataTrace.Criteria
{
    internal class GetMBTransOpenSearchCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_BatchSize", AllowNull = true)]
        internal int BatchSize { get; set; }

        internal override string GetSettingKey()
        {
            return "DCS_DC_OpenSearch_MBTransaction_Get";
        }

        internal override bool IsValid()
        {
            return true;
        }
    }
}
