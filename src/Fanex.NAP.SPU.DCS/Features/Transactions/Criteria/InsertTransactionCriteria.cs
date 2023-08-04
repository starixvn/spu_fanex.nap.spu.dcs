using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.NAP.SPU.DCS.Features.Transactions.Criteria
{
    public class InsertTransactionCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_FromTransID")]
        public long FromStagingTransId { get; set; }

        [SpParam(SqlParamName = "ip_ToTransID")]
        public long ToStagingTransId { get; set; }

        [SpParam(Ignored = true)]
        public bool IsReadyDbExtra { get; set; }

        public override string GetSettingKey()
        {
            if (IsReadyDbExtra)
            {
                return "DCS_ET_Transform_Transaction_Insert";
            }
            else
            {
                return "DCS_DC_Transform_Transaction_Insert";
            }
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
