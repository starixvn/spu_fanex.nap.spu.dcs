using Fanex.Data.Repository;
using System;

namespace Fanex.NAP.SPU.DCS.Features.ELK.Criteria
{
    public class PrepareStaging_BetMessageCriteria2 : NonQueryCommand
    {
        public DateTime Month { get; set; }

        public string Sport { get; set; }

        public string SiteType { get; set; }

        public string BettingMessageCount { get; set; }

        public override string GetSettingKey()
        {
            return "Task_PrepareStaging_BetMessageELK2";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
