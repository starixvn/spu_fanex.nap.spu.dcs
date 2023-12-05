namespace Fanex.NAP.SPU.DCS.Features.Devices.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class IdentityEvolutionInsertCriteria : CriteriaBase
    {

        [SpParam(SqlParamName = "ip_TransJson")]
        public string TransJson { get; set; }

        [SpParam(Ignored = true)]
        public bool IsReadyDbExtra { get; set; }

        public override string GetSettingKey()
        {
            return IsReadyDbExtra ? "DCS_ET_Identity_Evolution_Insert" : "DCS_DC_Identity_Evolution_Insert";
        }

        public override bool IsValid() => true;
    }
}
