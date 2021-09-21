using Fanex.Data;
using Fanex.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.SPU.DCS.Features.Monitoring.Criteria
{
    public class RawTransSumDailyInsertCriteria : NonQueryCommand
    {

        [SpParam(SqlParamName = "ip_TransDate")]
        public DateTime TransDate { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DC_Transform_RawTrans_Monitoring_Daily_Insert";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
