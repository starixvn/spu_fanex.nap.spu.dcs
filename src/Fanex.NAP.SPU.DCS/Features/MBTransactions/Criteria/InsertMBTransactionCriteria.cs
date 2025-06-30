using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.NAP.SPU.DCS.Features.MBTransactions.Criteria
{
    public class InsertMBTransactionCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_FromTransID", AllowNull = true)]
        public ulong FromTransID { get; set; }

        [SpParam(SqlParamName = "ip_ToTransID", AllowNull = true)]
        public ulong ToTransID { get; set; }

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
