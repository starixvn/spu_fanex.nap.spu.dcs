namespace Fanex.NAP.SPU.DCS.Features.UserAgent.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetUserAgentCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_size", AllowNull = true)]
        public int Size { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DC_Transform_UserAgent_GetNullBrowserOSList";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
