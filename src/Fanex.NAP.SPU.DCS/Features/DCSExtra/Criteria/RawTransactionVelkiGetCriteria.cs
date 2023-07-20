using Fanex.Data.Repository;
using System;

namespace Fanex.NAP.SPU.DCS.Features.DCSExtra.Criteria
{
    internal class RawTransactionVelkiGetCriteria : CriteriaBase
    {
        public string ip_SubscriberName { get; set; }

        public DateTime ip_LastTransTime { get; set; }

        public DateTime ip_ToTransTime { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_FPS_GetRawTransaction_Velki";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
