namespace Fanex.NAP.SPU.DCS.Features.DataTrace.Criteria
{
    using Fanex.Data.Repository;
    public class CheckDailyTaskReadyCriteria : CriteriaBase
    {
        public int TaskID { get; set; }

        public override string GetSettingKey()
        {
            return "Acc_CheckDailyTaskReadiness";
        }

        public override bool IsValid() => TaskID > 0;
    }
}
