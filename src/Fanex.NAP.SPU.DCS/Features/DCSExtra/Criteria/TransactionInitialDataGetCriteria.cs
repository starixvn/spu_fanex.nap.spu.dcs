using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.NAP.SPU.DCS.Features.DCSExtra.Criteria
{
    internal class TransactionInitialDataGetCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_BatchSize")]
        public int BatchSize { get; set; }

        [SpParam(SqlParamName = "ip_LastTransID_ParameterID")]
        public int LastTransID_ParameterID { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_ET_Transaction_InitialData_Get";
        }

        public override bool IsValid()
        {
            return true;
        }

    }
}
