using System;
using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.NAP.SPU.DCS.Features.Monitoring.Criteria
{
    internal class GetDCSCustByDateCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_TransDate")]
        public DateTime TransDate { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DC_Monitor_GetCustByDate";
        }

        public override bool IsValid()
        {
            return true;
        }

    }
}