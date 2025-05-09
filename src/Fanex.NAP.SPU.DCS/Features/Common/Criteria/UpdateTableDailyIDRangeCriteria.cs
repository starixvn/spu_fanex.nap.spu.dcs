using Fanex.Data;
using Fanex.Data.Repository;
using System;

namespace Fanex.NAP.SPU.DCS.Features.Common.Criteria
{
    public class UpdateTableDailyIDRangeCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_DatabaseName")]
        public string DatabaseName { get; set; }

        [SpParam(SqlParamName = "ip_TableName")]
        public string TableName { get; set; }

        [SpParam(SqlParamName = "ip_Date")]
        public DateTime Date { get; set; }

        [SpParam(SqlParamName = "ip_MinID")]
        public ulong MinID { get; set; }

        [SpParam(SqlParamName = "ip_MaxID")]
        public ulong MaxID { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DC_OpenSearch_TableDailyIDRange_Update";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
