using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.NAP.SPU.DCS.Features.DataTrace.Criteria
{
    internal class GetSystemSettingParameterByIDCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_ParameterID")]
        public int ParameterID { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DT_SystemSetting_GetParameterByID";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
