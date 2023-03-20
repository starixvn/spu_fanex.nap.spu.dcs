using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.NAP.SPU.DCS.Features.AbnormalTransaction.Criteria
{
    internal class AbnormalTransasctionGetCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_Size")]
        public int Size { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DC_Abnormal_Transasction_Get";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
