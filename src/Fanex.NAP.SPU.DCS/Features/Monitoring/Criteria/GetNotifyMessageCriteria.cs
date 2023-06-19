namespace Fanex.NAP.SPU.DCS.Features.Monitoring.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetNotifyMessageCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_NotifyType")]
        public string NotifyType { get; set; }

        [SpParam(SqlParamName = "ip_BatchSize")]
        public int BatchSize { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DC_Notify_NotifyMessage_Get";
        }

        public override bool IsValid()
        {
            return true;
        }

    }
}
