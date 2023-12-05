using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.NAP.SPU.DCS.Features.DCSExtra.Criteria
{
    public class InsertTransformRawTransCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_RawTransJson")]
        public string RawTransJson { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_ET_Transform_RawTrans_Insert";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
