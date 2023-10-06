namespace Fanex.NAP.SPU.DCS.Features.DataTrace.Criteria
{
    using System;
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetFPSTranBetTicketByRangeDateCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "FromTransDate")]
        public DateTime FromDate { get; set; }
        [SpParam(SqlParamName = "ToTransDate")]
        public DateTime ToDate { get; set; }

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
