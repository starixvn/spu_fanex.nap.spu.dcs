using System;
using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.NAP.SPU.DCS.Features.Monitoring.Criteria
{
    public class InsertNotifyMonitorTransactionCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_RptDate")]
        public DateTime RptDate { get; set; }

        [SpParam(SqlParamName = "ip_NotifyType")]
        public string NotifyType { get; set; }

        [SpParam(SqlParamName = "ip_BatchSize")]
        public int BatchSize { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DC_Notify_MonitorTransaction_Insert";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
