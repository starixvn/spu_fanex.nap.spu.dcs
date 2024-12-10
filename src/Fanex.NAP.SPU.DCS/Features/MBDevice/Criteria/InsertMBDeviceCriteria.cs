using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.NAP.SPU.DCS.Features.MBDevice.Criteria
{
    public class InsertMBDeviceCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_MBTransactionInfoJson")]
        public string MBTransactionInfoJson { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DC_Transform_MBDevice_Insert";
        }

        public override bool IsValid() => true;
    }
}
