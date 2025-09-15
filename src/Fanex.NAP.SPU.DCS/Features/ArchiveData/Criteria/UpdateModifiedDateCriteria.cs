using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.NAP.SPU.DCS.Features.ArchiveData.Criteria
{
    public class UpdateModifiedDateCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_BatchSize", AllowNull = true)]
        public int BatchSize { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DC_ArchiveData_ModifiedDate_Update";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
