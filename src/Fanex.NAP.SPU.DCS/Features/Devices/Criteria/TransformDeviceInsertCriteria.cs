using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.NAP.SPU.DCS.Features.Devices.Criteria
{
    public class TransformDeviceInsertCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_TransIDs", AllowNull = true)]
        public string TransIDs { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DC_Transform_Device_Insert";
        }

        public override bool IsValid() => true;
    }
}
