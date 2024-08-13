using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.NAP.SPU.DCS.Features.Devices.Criteria
{
    public class GetAttributesFPDeviceEvolutionCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_FPFingerprintIDs")]
        public string FPFingerprintIDs { get; set; }

        [SpParam(Ignored = true)]
        public bool IsReadyDbExtra { get; set; }

        public override string GetSettingKey()
        {
            return IsReadyDbExtra ? "" : "DCS_DC_Transform_FP_DeviceEvolution_GetAttributes";
        }

        public override bool IsValid() => true;
    }
}
