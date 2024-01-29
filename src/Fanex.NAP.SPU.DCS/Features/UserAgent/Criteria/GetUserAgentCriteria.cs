namespace Fanex.NAP.SPU.DCS.Features.UserAgent.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetUserAgentCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_size")]
        public int Size { get; set; }

        [SpParam(Ignored = true)]
        public bool IsReadyDbExtra { get; set; }

        public override string GetSettingKey()
        {
            return IsReadyDbExtra ? "DCS_ET_Transform_UserAgent_GetNullBrowserOSList" : "DCS_DC_Transform_UserAgent_GetNullBrowserOSList";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
