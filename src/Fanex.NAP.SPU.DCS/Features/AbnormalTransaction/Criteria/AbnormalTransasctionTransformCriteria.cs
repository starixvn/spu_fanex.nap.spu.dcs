using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.NAP.SPU.DCS.Features.AbnormalTransaction.Criteria
{
    internal class AbnormalTransasctionTransformCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_Size")]
        public int Size { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DC_Abnormal_Transasction_Transform";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
