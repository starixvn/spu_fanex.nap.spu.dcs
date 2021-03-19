using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.NAP.SPU.DCS.Features.Devices.Criteria
{
    public class UpdateDeviceIdCriteria: NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_TransJson")]
        public string TransIdJson { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DC_TransformData_Device_Insert";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
