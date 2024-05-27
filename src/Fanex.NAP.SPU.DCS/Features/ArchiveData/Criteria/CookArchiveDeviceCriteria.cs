using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.NAP.SPU.DCS.Features.ArchiveData.Criteria
{
    public class CookArchiveDeviceCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_BatchSize")]
        public int BatchSize { get; set; }

        [SpParam(SqlParamName = "op_ShouldContinue")]
        public int ShouldContinue { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DC_ArchiveData_Device_Cook";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}