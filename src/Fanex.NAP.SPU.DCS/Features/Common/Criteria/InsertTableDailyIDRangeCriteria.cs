using Fanex.Data;
using Fanex.Data.Repository;
using System;

namespace Fanex.NAP.SPU.DCS.Features.Common.Criteria
{
    public class InsertTableDailyIDRangeCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_DatabaseName", AllowNull = true)]
        public string DatabaseName { get; set; }

        [SpParam(SqlParamName = "ip_TableName", AllowNull = true)]
        public string TableName { get; set; }
        
        [SpParam(SqlParamName = "ip_DateField", AllowNull = true)]
        public string DateField { get; set; }
        
        [SpParam(SqlParamName = "ip_IDField", AllowNull = true)]
        public string IDField { get; set; }

        [SpParam(SqlParamName = "ip_Date", AllowNull = true)]
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
