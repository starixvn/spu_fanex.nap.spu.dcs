using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.NAP.SPU.DCS.Features.Transactions.Criteria
{
    public class UnhandleSubscriberActionCriteria: NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_FromTransID")]
        public long FromTransId { get; set; }

        [SpParam(SqlParamName = "ip_ToTransID")]
        public long ToTransId { get; set; }

        [SpParam(Ignored = true)]
        public bool IsReadyDbExtra { get; set; }

        public override string GetSettingKey()
        {
            if (IsReadyDbExtra)
            {
                return "DCS_ET_Transform_UnhandleSubscriberAction";
            }
            else
            {
                return "DCS_DC_Transform_UnhandleSubscriberAction";
            }            
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
