using Fanex.Data.Repository;
using System;

namespace Fanex.NAP.SPU.DCS.Features.ELK.Criteria
{
    public class GetLeagueGroupCriteria : NonQueryCommand
    {
        public DateTime BetDate { get; set; }

        public override string GetSettingKey()
        {
            return "NAP_DW_ELK_LeagueGroup_Select";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
