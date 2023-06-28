using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.NAP.SPU.DCS.Features.AbnormalTransaction.Criteria
{
    internal class AbnormalTransactionGetCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_Size")]
        public int Size { get; set; }

        [SpParam(SqlParamName = "op_MinID")]
        public ulong MinID { get; set; }

        [SpParam(SqlParamName = "op_MaxID")]
        public ulong MaxID { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DC_AbnormalTransaction_Get";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
