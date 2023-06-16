using System;
using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.NAP.SPU.DCS.Features.Monitoring.Criteria
{
    public class InsertMonitorTransactionCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_RptDate")]
        public DateTime RptDate { get; set; }

        [SpParam(SqlParamName = "ip_TransJson")]
        public string TransJson { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DC_Monitor_Transaction_Insert";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
