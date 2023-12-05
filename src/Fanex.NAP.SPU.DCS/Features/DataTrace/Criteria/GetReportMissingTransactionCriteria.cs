using System;
using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.NAP.SPU.DCS.Features.DataTrace.Criteria
{
    public class GetReportMissingTransactionCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_FromDate")]
        public DateTime FromDate { get; set; }

        [SpParam(SqlParamName = "ip_ToDate")]
        public DateTime ToDate { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DT_Report_MissingTransaction_Get";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
