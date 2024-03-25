using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.NAP.SPU.DCS.Features.DataTrace.Criteria
{
    public class InitialDataLoginTransactionCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_BatchSize")]
        public int BatchSize { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DT_Summary_LoginTransaction_InitialData";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
