using Fanex.Data;
using Fanex.Data.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fanex.NAP.SPU.DCS.Features.UserAgent.Criteria
{
    public class GetUserAgentCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_size")]
        public int Size { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DC_TransformData_UserAgent_GetNullBrowserOSList";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
