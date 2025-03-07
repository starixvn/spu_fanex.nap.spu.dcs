using Fanex.NAP.SPU.DCS.Features.ELK.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Fanex.NAP.SPU.DCS.Features.ELK
{
    public interface IELKServices
    {
        void PrepareStagingForParlayValidationStats(DateTime month, string sport, string siteType, string bettingMessageCount, int runningCounter);

        void PrepareStagingForBetMessage(DateTime month, string sport, string siteType, string bettingMessageCount, int runningCounter);

        IEnumerable<LeagueGroupEntity> GetLeagueGroupId(DateTime betDate);

        void PrepareStagingForBetMessageDaily(DateTime betDate, string sport, string siteType, int leagueGroupId, string bettingMessageCount, int runningCounter);
    }

    [ExcludeFromCodeCoverage(Justification = "Excluded from code coverage")]
    public class ELKServices : IELKServices
    {
        private readonly IELKDataAccess elkDataAccess;

        public ELKServices(IELKDataAccess elkDataAccess) { 
            this.elkDataAccess = elkDataAccess;
        }

        public IEnumerable<LeagueGroupEntity> GetLeagueGroupId(DateTime betDate)
        {
            return elkDataAccess.GetLeagueGroupId(betDate);
        }

        public void PrepareStagingForBetMessage(DateTime month, string sport, string siteType, string bettingMessageCount, int runningCounter)
        {
            elkDataAccess.PrepareStagingForBetMessage(month, sport, siteType, bettingMessageCount, runningCounter);
        }

        public void PrepareStagingForBetMessageDaily(DateTime betDate, string sport, string siteType, int leagueGroupId, string bettingMessageCount, int runningCounter)
        {
            elkDataAccess.PrepareStagingForBetMessageDaily(betDate, sport, siteType, leagueGroupId, bettingMessageCount, runningCounter);
        }

        public void PrepareStagingForParlayValidationStats(DateTime month, string sport, string siteType, string bettingMessageCount, int runningCounter)
        {
            elkDataAccess.PrepareStagingForParlayValidationStats(month, sport, siteType, bettingMessageCount, runningCounter);
        }
    }
}
