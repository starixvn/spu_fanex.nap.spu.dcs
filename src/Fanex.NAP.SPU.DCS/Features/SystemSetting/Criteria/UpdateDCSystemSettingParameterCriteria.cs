using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.NAP.SPU.DCS.Features.SystemSetting.Criteria
{
    public class UpdateDCSystemSettingParameterCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_ParameterID")]
        public int ParameterID { get; set; }

        [SpParam(SqlParamName = "ip_ParameterValue")]
        public string ParameterValue { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DC_SystemSetting_UpdateParameterByID";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}