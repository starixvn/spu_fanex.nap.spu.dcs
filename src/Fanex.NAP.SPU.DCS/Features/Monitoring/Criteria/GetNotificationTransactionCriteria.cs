namespace Fanex.NAP.SPU.DCS.Features.Monitoring.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class GetNotificationTransactionCriteria : NonQueryCommand
    {       
        public override string GetSettingKey()
        {
            return "DCS_DC_Notification_Transaction";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
