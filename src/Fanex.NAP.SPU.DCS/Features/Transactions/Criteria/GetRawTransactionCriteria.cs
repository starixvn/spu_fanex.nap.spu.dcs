using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.NAP.SPU.DCS.Features.Transactions.Criteria
{
    public class GetRawTransactionCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_IsProcessed")]
        public int IsProcessed { get; set; }

        [SpParam(SqlParamName = "ip_NoOfTickets")]
        public int NumberOfTickets { get; set; }

        [SpParam(SqlParamName = "ip_NoOfBatch")]
        public int NumberOfBatch { get; set; }

        [SpParam(Ignored = true)]
        public bool IsReadyDbExtra { get; set; }

        public override string GetSettingKey()
        {
            return IsReadyDbExtra ? "DCS_ET_Transform_RawTrans_GetPackage" : "DCS_DC_Transform_RawTrans_GetPackage";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
