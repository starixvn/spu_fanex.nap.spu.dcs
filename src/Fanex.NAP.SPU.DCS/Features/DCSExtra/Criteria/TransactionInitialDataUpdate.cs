using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.NAP.SPU.DCS.Features.DCSExtra.Criteria
{
    internal class TransactionInitialDataUpdate : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_TransJs")]
        public string TransJs { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_ET_Transaction_InitialData_Update";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
