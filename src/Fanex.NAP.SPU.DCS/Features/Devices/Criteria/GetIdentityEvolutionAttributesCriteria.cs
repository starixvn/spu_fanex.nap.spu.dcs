namespace Fanex.NAP.SPU.DCS.Features.Devices.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetIdentityEvolutionAttributesCriteria : CriteriaBase
    {

        [SpParam(SqlParamName = "ip_RawFingerPrintIDs")]
        public string RawFingerPrintIDs { get; set; }

        [SpParam(SqlParamName = "ip_BrowserFamilyCode")]
        public string BrowserFamilyCode { get; set; }

        public override string GetSettingKey() => "DCS_DC_Identity_Evolution_GetAttributes";

        public override bool IsValid() => true;
    }
}
