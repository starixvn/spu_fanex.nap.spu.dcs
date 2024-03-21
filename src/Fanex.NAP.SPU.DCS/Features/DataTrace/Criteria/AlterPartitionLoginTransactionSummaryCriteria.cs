using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.NAP.SPU.DCS.Features.DataTrace.Criteria
{
    public class AlterPartitionLoginTransactionSummaryCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_RetentionPartitions")]
        public int RetentionPartitions { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DT_ArchiveData_LoginTransactionSummary_AlterPartition";
        }

        public override bool IsValid() => true;
    }
}
