using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.NAP.SPU.DCS.Features.Devices.Criteria
{
    public class InsertFPDeviceEvolutionCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_DeviceEvolutionJson")]
        public string DeviceEvolutionJson { get; set; }

        [SpParam(Ignored = true)]
        public bool IsReadyDbExtra { get; set; }

        public override string GetSettingKey()
        {
            return IsReadyDbExtra ? "" : "DCS_DC_Transform_FP_DeviceEvolutionInsert";
        }

        public override bool IsValid() => true;
    }
}
