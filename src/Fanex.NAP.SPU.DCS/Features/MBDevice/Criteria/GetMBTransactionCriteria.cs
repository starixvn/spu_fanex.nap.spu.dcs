using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.NAP.SPU.DCS.Features.MBDevice.Criteria
{
    public class GetMBTransactionCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_LimitTotalRecord", AllowNull = true)]
        public int LimitTotalRecord { get; set; }

        [SpParam(SqlParamName = "ip_NoOfBatch", AllowNull = true)]
        public int NumberOfBatch { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DC_Transform_MBTransaction_GetPackage";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
