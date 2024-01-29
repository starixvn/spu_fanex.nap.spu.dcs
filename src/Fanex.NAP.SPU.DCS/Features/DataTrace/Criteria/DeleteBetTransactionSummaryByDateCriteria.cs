using System;
using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.NAP.SPU.DCS.Features.DataTrace.Criteria
{
    public class DeleteBetTransactionSummaryByDateCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_TransDate")]
        public DateTime TransDate { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DT_Summary_BetTransaction_Delete";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
