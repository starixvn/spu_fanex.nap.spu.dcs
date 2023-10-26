using Fanex.Data;
using Fanex.Data.Repository;
using System;

namespace Fanex.NAP.SPU.DCS.Features.DataTrace.Criteria
{
    internal class DeleteBetTransactionSummaryByDateCriteria : NonQueryCommand
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
