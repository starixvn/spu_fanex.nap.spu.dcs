using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.NAP.SPU.DCS.Features.AbnormalTransaction.Criteria
{
    internal class AbnormalTransasctionUpdateCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_TransJson")]
        public string TransJson { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DC_Abnormal_Transasction_Update";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
