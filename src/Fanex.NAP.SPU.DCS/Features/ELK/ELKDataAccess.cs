using Fanex.Data.Repository;
using Fanex.NAP.SPU.DCS.Features.ELK.Criteria;
using System;

namespace Fanex.NAP.SPU.DCS.Features.ELK
{
    public interface IELKDataAccess
    {
        void PrepareStagingForParlayValidationStats(DateTime month, string sport, string siteType, string bettingMessageCount, int runningCounter);

        void PrepareStagingForBetMessage(DateTime month, string sport, string siteType, string bettingMessageCount, int runningCounter);
    }

    public class ELKDataAccess : IELKDataAccess
    {
        private readonly IDynamicRepository repository;

        public ELKDataAccess(IDynamicRepository repository)
        {
            this.repository = repository ?? new DynamicRepository();
        }

        public void PrepareStagingForParlayValidationStats(DateTime month, string sport, string siteType, string bettingMessageCount, int runningCounter)
        {
            if(runningCounter == 1)
            {
                var criteria = new PrepareStaging_ParlayValidationStatsCriteria1
                {
                    Month = month,
                    Sport = sport,
                    SiteType = siteType,
                    BettingMessageCount = bettingMessageCount                    
                };

                repository.Execute(criteria);
            }
            else
            {
                var criteria = new PrepareStaging_ParlayValidationStatsCriteria2
                {
                    Month = month,
                    Sport = sport,
                    SiteType = siteType,
                    BettingMessageCount = bettingMessageCount
                };

                repository.Execute(criteria);
            }            
        }

        public void PrepareStagingForBetMessage(DateTime month, string sport, string siteType, string bettingMessageCount, int runningCounter)
        {
            if (runningCounter == 1)
            {
                var criteria = new PrepareStaging_BetMessageCriteria1
                {
                    Month = month,
                    Sport = sport,
                    SiteType = siteType,
                    BettingMessageCount = bettingMessageCount
                };

                repository.Execute(criteria);
            }
            else
            {
                var criteria = new PrepareStaging_BetMessageCriteria2
                {
                    Month = month,
                    Sport = sport,
                    SiteType = siteType,
                    BettingMessageCount = bettingMessageCount
                };

                repository.Execute(criteria);
            }
        }
    }
}
