using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.NAP.SPU.DCS.Features.AbnormalTransaction.Criteria
{
    public class AbnormalTransactionGetCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_Size", AllowNull = true)]
        public int Size { get; set; }

        [SpParam(SqlParamName = "op_MinID", AllowNull = true)]
        public ulong MinID { get; set; }

        [SpParam(SqlParamName = "op_MaxID", AllowNull = true)]
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
