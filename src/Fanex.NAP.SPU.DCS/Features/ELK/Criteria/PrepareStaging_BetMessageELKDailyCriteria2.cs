using Fanex.Data.Repository;
using System;

namespace Fanex.NAP.SPU.DCS.Features.ELK.Criteria
{
    public class PrepareStaging_BetMessageELKDailyCriteria2 : NonQueryCommand
    {
        public DateTime BetDate { get; set; }

        public string Sport { get; set; }

        public string SiteType { get; set; }

        public int LeagueGroupID { get; set; }

        public string BettingMessageCount { get; set; }

        public override string GetSettingKey()
        {
            return "Task_PrepareStaging_BetMessageELKDaily2";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
