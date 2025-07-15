using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.NAP.SPU.DCS.Features.Transactions.Criteria
{
    public class InsertTransactionCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_FromTransID", AllowNull = true)]
        public long FromStagingTransId { get; set; }

        [SpParam(SqlParamName = "ip_ToTransID", AllowNull = true)]
        public long ToStagingTransId { get; set; }

        [SpParam(Ignored = true)]
        public bool IsReadyDbExtra { get; set; }

        public override string GetSettingKey()
        {
            return IsReadyDbExtra ? "DCS_ET_Transform_Transaction_Insert" : "DCS_DC_Transform_Transaction_Insert";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
