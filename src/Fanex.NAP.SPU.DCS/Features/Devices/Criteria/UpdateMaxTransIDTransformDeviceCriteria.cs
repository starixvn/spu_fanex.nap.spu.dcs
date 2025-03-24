using Fanex.Data.Repository;

namespace Fanex.NAP.SPU.DCS.Features.Devices.Criteria
{
    public class UpdateMaxTransIDTransformDeviceCriteria : NonQueryCommand
    {
        public override string GetSettingKey()
        {
            return "DCS_DC_Transform_Device_UpdateMaxTransID";
        }

        public override bool IsValid() => true;
    }
}
