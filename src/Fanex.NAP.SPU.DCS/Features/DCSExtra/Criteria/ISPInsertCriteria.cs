using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.NAP.SPU.DCS.Features.DCSExtra.Criteria
{
    internal class ISPInsertCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_InputJs")]
        public string InputJs { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_ET_ISP_Insert";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
