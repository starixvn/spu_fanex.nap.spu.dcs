namespace Fanex.NAP.SPU.DCS.Features.Devices.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class IdentityEvolutionInsertCriteria : CriteriaBase
    {

        [SpParam(SqlParamName = "ip_TransJson")]
        public string transJson { get; set; }

        public override string GetSettingKey() => "DCS_DC_Identity_Evolution_Insert";

        public override bool IsValid() => true;
    }
}
