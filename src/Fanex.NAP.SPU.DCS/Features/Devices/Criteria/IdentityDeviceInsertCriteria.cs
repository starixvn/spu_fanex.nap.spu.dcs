namespace Fanex.NAP.SPU.DCS.Features.Devices.Criteria
{
    using Fanex.Data.Repository;
    using Fanex.Data;
    public class IdentityDeviceInsertCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_TransIDs")]
        public string transIdJson { get; set; }
        
        public override string GetSettingKey() => "DCS_DC_Identity_Device_Insert";

        public override bool IsValid() => true;
    }
}
