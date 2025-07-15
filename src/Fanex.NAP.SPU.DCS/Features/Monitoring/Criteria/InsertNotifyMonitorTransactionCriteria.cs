using System;
using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.NAP.SPU.DCS.Features.Monitoring.Criteria
{
    public class InsertNotifyMonitorTransactionCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_RptDate", AllowNull = true)]
        public DateTime RptDate { get; set; }

        [SpParam(SqlParamName = "ip_NotifyType", AllowNull = true)]
        public string NotifyType { get; set; }

        [SpParam(SqlParamName = "ip_GroupId", AllowNull = true)]
        public string GroupId { get; set; }

        [SpParam(SqlParamName = "ip_LimitShowCust", AllowNull = true)]
        public int LimitShowCust { get; set; }


        [SpParam(SqlParamName = "op_NotifyMessageID", AllowNull = true)]
        public uint NotifyMessageID { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DC_NotifyMessage_InsertMonitorTransaction";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
