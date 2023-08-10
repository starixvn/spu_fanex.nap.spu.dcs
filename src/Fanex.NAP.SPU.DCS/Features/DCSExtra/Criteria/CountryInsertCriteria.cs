using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.NAP.SPU.DCS.Features.DCSExtra.Criteria
{
    internal class CountryInsertCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_InputJs")]
        public string InputJs { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_ET_Country_Insert";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
