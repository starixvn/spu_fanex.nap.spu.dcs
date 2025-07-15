namespace Fanex.NAP.SPU.DCS.Features.Monitoring.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetNotifyMessageByNotifyTypeCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_NotifyType", AllowNull = true)]
        public string NotifyType { get; set; }

        [SpParam(SqlParamName = "ip_BatchSize", AllowNull = true)]
        public int BatchSize { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DC_NotifyMessage_GetByNotifyType";
        }

        public override bool IsValid()
        {
            return true;
        }

    }
}
