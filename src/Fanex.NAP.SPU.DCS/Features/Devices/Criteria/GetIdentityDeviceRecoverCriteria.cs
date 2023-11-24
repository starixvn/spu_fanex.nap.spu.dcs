namespace Fanex.NAP.SPU.DCS.Features.Devices.Criteria
{
    using Fanex.Data.Repository;
    using Fanex.Data;

    public class GetIdentityDeviceRecoverCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_TransIDs")]
        public string TransIds { get; set; }

        public override string GetSettingKey() => "DCS_DC_Identity_Device_Recover";

        public override bool IsValid() => true;
    }
}
