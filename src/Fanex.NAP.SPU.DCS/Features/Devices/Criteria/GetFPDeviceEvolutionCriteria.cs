using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.NAP.SPU.DCS.Features.Devices.Criteria
{
    public class GetFPDeviceEvolutionCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_BatchSize", AllowNull = true)]
        public int BatchSize { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DC_Transform_FP_DeviceEvolutionGet";
        }

        public override bool IsValid() => true;
    }
}
