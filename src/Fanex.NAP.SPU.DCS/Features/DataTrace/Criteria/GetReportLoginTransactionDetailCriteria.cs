using System;
using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.NAP.SPU.DCS.Features.DataTrace.Criteria
{
    public class GetReportLoginTransactionDetailCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_TransDate")]
        public DateTime TransDate { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DT_Report_LoginTransaction_GetDetailByDate";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
