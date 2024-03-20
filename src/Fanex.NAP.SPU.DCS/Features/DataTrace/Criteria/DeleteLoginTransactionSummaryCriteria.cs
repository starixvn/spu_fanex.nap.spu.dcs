using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.NAP.SPU.DCS.Features.DataTrace.Criteria
{
    public class DeleteLoginTransactionSummaryCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_RetentionDays")]
        public int RetentionDays { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DT_ArchiveData_LoginTransactionSummary_Delete";
        }

        public override bool IsValid() => true;
    }
}
