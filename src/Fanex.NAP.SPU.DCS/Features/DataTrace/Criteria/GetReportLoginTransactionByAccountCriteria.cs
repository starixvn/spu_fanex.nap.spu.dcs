using Fanex.Data;
using Fanex.Data.Repository;
using System;

namespace Fanex.NAP.SPU.DCS.Features.DataTrace.Criteria
{
    public class GetReportLoginTransactionByAccountCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_FromDate", AllowNull = true)]
        public DateTime FromDate { get; set; }

        [SpParam(SqlParamName = "ip_ToDate", AllowNull = true)]
        public DateTime ToDate { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DT_Report_LoginTransactionByAccount_GetByDateRange";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
