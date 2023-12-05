using Fanex.Data.Repository;

namespace Fanex.NAP.SPU.DCS.Features.DCSExtra.Criteria
{
    public class LastDeviceFingerprintGetCriteria : CriteriaBase
    {
        public string ip_DeviceNoList { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_FPS_GetLastDeviceFingerprint";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
