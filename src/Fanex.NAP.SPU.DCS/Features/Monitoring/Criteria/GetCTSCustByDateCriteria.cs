using System;
using Fanex.Data.Repository;

namespace Fanex.NAP.SPU.DCS.Features.Monitoring.Criteria
{
    internal class GetCTSCustByDateCriteria : CriteriaBase
    {
        public DateTime RptDate { get; set; }

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
