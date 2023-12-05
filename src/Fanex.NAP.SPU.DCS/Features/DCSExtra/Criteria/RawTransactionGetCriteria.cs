using Fanex.Data.Repository;
using System;

namespace Fanex.NAP.SPU.DCS.Features.DCSExtra.Criteria
{
    public class RawTransactionGetCriteria : CriteriaBase
    {
        public string ip_SubscriberName { get; set; }

        public DateTime ip_LastTransTime { get; set; }

        public DateTime ip_ToTransTime { get; set; }

        public override string GetSettingKey()
        {
            return "";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
