using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.NAP.SPU.DCS.Features.AbnormalTransaction.Criteria
{
    public class AbnormalTransactionUpdateCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_TransJson", AllowNull = true)]
        public string TransJson { get; set; }

        [SpParam(SqlParamName = "ip_MinID", AllowNull = true)]
        public ulong MinID { get; set; }

        [SpParam(SqlParamName = "ip_MaxID", AllowNull = true)]
        public ulong MaxID { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DC_AbnormalTransaction_Update";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
