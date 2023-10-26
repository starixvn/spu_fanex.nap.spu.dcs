namespace Fanex.NAP.SPU.DCS.Features.DataTrace.Criteria
{
    using Fanex.Data.Repository;
    using System;

    public class GetBetTransactionCriteria : CriteriaBase
    {
        public DateTime FromTransDate { get; set; }
        public DateTime ToTransDate { get; set; }

        public override string GetSettingKey()
        {
            return "Acc_Rpt_FPS_GetCustBettingInfo";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
