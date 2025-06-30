namespace Fanex.NAP.SPU.DCS.Features.Monitoring.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class UpdateNotifyMessagesToNotifiedCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_IDs", AllowNull = true)]
        public string IDs { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DC_NotifyMessage_UpdateNotified";
        }

        public override bool IsValid()
        {
            return true;
        }

    }
}
