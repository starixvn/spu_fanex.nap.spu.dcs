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
            if (IsReadyDbExtra)
            {
                return "DCS_ET_ArchiveData_ModifiedDate_Update";
            }
            else
            {
                return "DCS_DC_ArchiveData_ModifiedDate_Update";
            }
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
