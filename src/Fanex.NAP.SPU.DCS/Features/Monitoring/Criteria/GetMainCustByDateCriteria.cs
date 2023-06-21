using System;
using Fanex.Data.Repository;

namespace Fanex.NAP.SPU.DCS.Features.Monitoring.Criteria
{
    internal class GetMainCustByDateCriteria : CriteriaBase
    {
        public DateTime RptDate { get; set; }
        public int MinCustID { get; set; }
        public int BatchSize { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_Monitor_MissingFPSTrans_GetCustByDate";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
