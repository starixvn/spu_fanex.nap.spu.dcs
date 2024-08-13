using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.NAP.SPU.DCS.Features.ArchiveData.Criteria
{
    public class InsertFPDeviceMappingCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_BatchSize")]
        public int BatchSize { get; set; }

        [SpParam(Ignored = true)]
        public bool IsReadyDbExtra { get; set; }

        public override string GetSettingKey()
        {
            return IsReadyDbExtra ? "" : "DCS_DC_Transform_FP_DeviceMapping_Insert";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
