
namespace Fanex.NAP.SPU.DCS.Features.DataTrace.Criteria
{
    using Fanex.Data.Repository;
    using Fanex.Data;
    public class GetTranPackageCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_BatchSize")]
        public int BatchSize { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DC_OpenSearch_Transaction_Get";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
