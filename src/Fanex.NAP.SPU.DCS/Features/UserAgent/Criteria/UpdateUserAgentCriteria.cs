using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.NAP.SPU.DCS.Features.UserAgent.Criteria
{
    public class UpdateUserAgentCriteria: NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_UserAgentJson")]
        public string UserAgentJson { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DC_TransformData_UserAgent_UpdateOSBrowser";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
