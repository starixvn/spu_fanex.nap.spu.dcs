using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.NAP.SPU.DCS.Features.Devices.Criteria
{
    public class TransformDeviceInsertCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_TransIDs")]
        public string TransIDs { get; set; }

        [SpParam(Ignored = true)]
        public bool IsReadyDbExtra { get; set; }

        public override string GetSettingKey()
        {
            return IsReadyDbExtra ? "DCS_ET_Transform_Device_Insert" : "DCS_DC_Transform_Device_Insert";
        }

        public override bool IsValid() => true;
    }
}
