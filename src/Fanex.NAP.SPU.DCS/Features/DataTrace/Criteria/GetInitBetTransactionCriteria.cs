using System;
using Fanex.Data.Repository;

namespace Fanex.NAP.SPU.DCS.Features.DataTrace.Criteria
{
    public class GetInitBetTransactionCriteria : CriteriaBase
    {
        public DateTime FromTransDate { get; set; }
        public DateTime ToTransDate { get; set; }

        public override string GetSettingKey()
        {
            return "Ned_Acc_Rpt_FPS_GetCustBettingInfo";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
