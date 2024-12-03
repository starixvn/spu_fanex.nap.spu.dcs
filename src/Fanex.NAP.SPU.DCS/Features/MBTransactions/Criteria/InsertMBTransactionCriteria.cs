using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.NAP.SPU.DCS.Features.MBTransactions.Criteria
{
    public class InsertMBTransactionCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_FromTransID")]
        public long FromStagingTransId { get; set; }

        [SpParam(SqlParamName = "ip_ToTransID")]
        public long ToStagingTransId { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DC_Transform_MBTransaction_Insert";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
