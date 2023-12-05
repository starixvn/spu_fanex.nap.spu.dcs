using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.NAP.SPU.DCS.Features.ArchiveData.Criteria
{
    public class UpdateModifiedDateCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_BatchSize")]
        public int BatchSize { get; set; }

        [SpParam(Ignored = true)]
        public bool IsReadyDbExtra { get; set; }

        public override string GetSettingKey()
        {
            return IsReadyDbExtra ? "DCS_ET_ArchiveData_ModifiedDate_Update" : "DCS_DC_ArchiveData_ModifiedDate_Update";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
