namespace Fanex.NAP.SPU.DCS.Features.Monitoring.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class UpdateNotifyMessageCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_IDs")]
        public string IDs { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DC_Notify_NotifyMessage_Update";
        }

        public override bool IsValid()
        {
            return true;
        }

    }
}
