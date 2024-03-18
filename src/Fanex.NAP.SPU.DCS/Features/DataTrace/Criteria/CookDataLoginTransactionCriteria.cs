using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.NAP.SPU.DCS.Features.DataTrace.Criteria
{
    public class CookDataLoginTransactionCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_Size")]
        public int Size { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DT_Summary_LoginTransaction_CookData";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
