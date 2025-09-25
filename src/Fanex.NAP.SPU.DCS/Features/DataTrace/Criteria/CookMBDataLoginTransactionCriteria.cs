using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.NAP.SPU.DCS.Features.DataTrace.Criteria
{
    public class CookMBDataLoginTransactionCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_BatchSize", AllowNull = true)]
        public int BatchSize { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DT_Summary_MBLoginTransaction_CookData";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
