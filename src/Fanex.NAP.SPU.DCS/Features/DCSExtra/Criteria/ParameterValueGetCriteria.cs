using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.NAP.SPU.DCS.Features.DCSExtra.Criteria
{
    internal class ParameterValueGetCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_ParameterID")]
        public int ParameterID { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_ET_SystemSetting_GetParameterValue";
        }

        public override bool IsValid()
        {
            return true;
        }

    }
}
