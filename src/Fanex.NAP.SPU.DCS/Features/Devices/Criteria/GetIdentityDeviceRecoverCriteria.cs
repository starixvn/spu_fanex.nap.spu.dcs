namespace Fanex.NAP.SPU.DCS.Features.Devices.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetIdentityDeviceRecoverCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_TransIDs")]
        public string TransIds { get; set; }

        [SpParam(Ignored = true)]
        public bool IsReadyDbExtra { get; set; }

        public override string GetSettingKey()
        {
            return IsReadyDbExtra ? "DCS_ET_Identity_Device_Recover" : "DCS_DC_Identity_Device_Recover";
        }

        public override bool IsValid() => true;
    }
}
