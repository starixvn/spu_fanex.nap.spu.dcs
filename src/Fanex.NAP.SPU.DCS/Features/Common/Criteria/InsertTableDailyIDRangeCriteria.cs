using Fanex.Data;
using Fanex.Data.Repository;
using System;

namespace Fanex.NAP.SPU.DCS.Features.Common.Criteria
{
    public class InsertTableDailyIDRangeCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_DatabaseName")]
        public string DatabaseName { get; set; }

        [SpParam(SqlParamName = "ip_TableName")]
        public string TableName { get; set; }
        
        [SpParam(SqlParamName = "ip_DateField")]
        public string DateField { get; set; }
        
        [SpParam(SqlParamName = "ip_IDField")]
        public string IDField { get; set; }

        [SpParam(SqlParamName = "ip_Date")]
        public DateTime Date { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DC_OpenSearch_TableDailyIDRange_Insert";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
