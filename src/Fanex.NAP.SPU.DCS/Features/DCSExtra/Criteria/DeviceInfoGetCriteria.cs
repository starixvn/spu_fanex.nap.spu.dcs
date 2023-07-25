using Fanex.Data.Repository;

namespace Fanex.NAP.SPU.DCS.Features.DCSExtra.Criteria
{
    internal class DeviceInfoGetCriteria : CriteriaBase
    {
        public string ip_DeviceIDList { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_FPS_GetDeviceInfo";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
