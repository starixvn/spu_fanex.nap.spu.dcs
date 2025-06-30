using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.NAP.SPU.DCS.Features.UserAgent.Criteria
{
    public class UpdateUserAgentCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_UserAgentJson", AllowNull = true)]
        public string UserAgentJson { get; set; }

        [SpParam(Ignored = true)]
        public bool IsReadyDbExtra { get; set; }

        public override string GetSettingKey()
        {
            return IsReadyDbExtra ? "DCS_ET_Transform_UserAgent_UpdateOSBrowser" : "DCS_DC_Transform_UserAgent_UpdateOSBrowser";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
