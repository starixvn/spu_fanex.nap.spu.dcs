using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.NAP.SPU.DCS.Features.Devices
{
    public class GetDeviceStagingTransactionPackageCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_NoOfTickets")]
        public int Size { get; set; }

        [SpParam(SqlParamName = "ip_NoOfBatch")]
        public int NoOfBatch { get; set; }

        [SpParam(Ignored = true)]
        public bool IsReadyDbExtra { get; set; }

        public override string GetSettingKey()
        {
            return IsReadyDbExtra ? "DCS_ET_Transform_Device_GetPackage" : "DCS_DC_Transform_Device_GetPackage";
        }

        public override bool IsValid() => true;
    }
}
