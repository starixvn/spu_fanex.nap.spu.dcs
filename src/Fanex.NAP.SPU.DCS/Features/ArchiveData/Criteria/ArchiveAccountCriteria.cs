using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.NAP.SPU.DCS.Features.ArchiveData.Criteria
{
    public class ArchiveAccountCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_BatchSize")]
        public int BatchSize { get; set; }

        [SpParam(SqlParamName = "op_RowCount")]
        public int RowCount { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DC_ArchiveData_Account_Archive";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}