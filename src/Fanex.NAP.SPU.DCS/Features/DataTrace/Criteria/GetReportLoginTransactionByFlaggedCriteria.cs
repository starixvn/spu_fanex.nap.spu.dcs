using Fanex.Data;
using Fanex.Data.Repository;
using System;

namespace Fanex.NAP.SPU.DCS.Features.DataTrace.Criteria
{
    internal class GetReportLoginTransactionByFlaggedCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_FromDate")]
        public DateTime FromDate { get; set; }

        [SpParam(SqlParamName = "ip_ToDate")]
        public DateTime ToDate { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DT_Report_LoginTransactionByFlagged_GetByDateRange";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
