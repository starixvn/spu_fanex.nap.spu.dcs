using Fanex.Data.Repository;

namespace Fanex.NAP.SPU.DCS.Features.Devices.Criteria
{
    public class UpdateMaxMBTransIDTransformDeviceCriteria : NonQueryCommand
    {
        public override string GetSettingKey()
        {
            return "DCS_DC_Transform_MBDevice_UpdateMaxMBTransID";
        }

        public override bool IsValid() => true;
    }
}
