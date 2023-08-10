using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.NAP.SPU.DCS.Features.DCSExtra.Criteria
{
    internal class RegionInsertCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_InputJs")]
        public string InputJs { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_ET_Region_Insert";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
