using System;
using Fanex.Data.Repository;

namespace Fanex.NAP.SPU.DCS.Features.DCSExtra.Criteria
{
  internal class RawTransactionCTMaxGetCriteria : CriteriaBase
  {
    public string ip_SubscriberName { get; set; }

    public DateTime ip_LastTransTime { get; set; }

    public DateTime ip_ToTransTime { get; set; }

    public override string GetSettingKey()
    {
      return "DCS_FPS_GetRawTransaction_CTMax";
    }

    public override bool IsValid()
    {
      return true;
    }
  }
}
