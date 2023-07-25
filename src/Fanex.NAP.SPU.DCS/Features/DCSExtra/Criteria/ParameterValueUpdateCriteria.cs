using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.NAP.SPU.DCS.Features.DCSExtra.Criteria
{
    public class ParameterValueUpdateCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_ParameterID")]
        public int ParameterID { get; set; }

        [SpParam(SqlParamName = "ip_ParameterValue")]
        public string ParameterValue { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_ET_SystemSetting_UpdateParameterValue";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
