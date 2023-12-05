using Fanex.Data.Repository;

namespace Fanex.NAP.SPU.DCS.Features.DCSExtra.Criteria
{
    public class LocationByIPListGet : CriteriaBase
    {
        public string IPs { get; set; }

        public override string GetSettingKey()
        {
            return "GetLocationByIPList";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
