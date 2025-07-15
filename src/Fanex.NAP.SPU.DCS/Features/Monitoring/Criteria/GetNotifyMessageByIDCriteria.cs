namespace Fanex.NAP.SPU.DCS.Features.Monitoring.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetNotifyMessageByIDCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_NotifyMessageID", AllowNull = true)]
        public uint NotifyMessageID { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DC_NotifyMessage_GetByID";
        }

        public override bool IsValid()
        {
            return true;
        }

    }
}
