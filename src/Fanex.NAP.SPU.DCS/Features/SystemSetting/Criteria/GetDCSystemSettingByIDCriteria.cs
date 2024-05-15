using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.NAP.SPU.DCS.Features.SystemSetting.Criteria
{
    public class GetDCSystemSettingByIDCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_ParameterID")]
        public int ParameterID { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DC_SystemSetting_GetParameterByID";
        }

        public override bool IsValid()
        {
            return true;
        }

    }
}
