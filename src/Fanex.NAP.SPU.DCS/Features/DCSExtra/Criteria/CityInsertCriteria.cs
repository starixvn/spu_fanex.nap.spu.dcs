using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.NAP.SPU.DCS.Features.DCSExtra.Criteria
{
    internal class CityInsertCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_InputJs")]
        public string InputJs { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_ET_City_Insert";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
