using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.NAP.SPU.DCS.Features.MBTransactions.Criteria
{
    public class GetMBRawTransactionCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_NoOfTickets", AllowNull = true)]
        public int NumberOfTickets { get; set; }

        [SpParam(SqlParamName = "ip_NoOfBatch", AllowNull = true)]
        public int NumberOfBatch { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DC_Transform_MBRawTransaction_GetPackage";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
