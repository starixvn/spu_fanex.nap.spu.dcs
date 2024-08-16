using Fanex.NAP.SPU.DCS.Features.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.SPU.DCS.Features.ELK
{
    public interface IELKServices
    {
        void PrepareStagingForParlayValidationStats(DateTime month, string sport, string siteType, string bettingMessageCount, int runningCounter);

        void PrepareStagingForBetMessage(DateTime month, string sport, string siteType, string bettingMessageCount, int runningCounter);

    }

    public class ELKServices : IELKServices
    {
        private readonly IELKDataAccess elkDataAccess;

        public ELKServices(IELKDataAccess elkDataAccess) { 
            this.elkDataAccess = elkDataAccess;
        }

        public void PrepareStagingForBetMessage(DateTime month, string sport, string siteType, string bettingMessageCount, int runningCounter)
        {
            elkDataAccess.PrepareStagingForBetMessage(month, sport, siteType, bettingMessageCount, runningCounter);
        }

        public void PrepareStagingForParlayValidationStats(DateTime month, string sport, string siteType, string bettingMessageCount, int runningCounter)
        {
            elkDataAccess.PrepareStagingForParlayValidationStats(month, sport, siteType, bettingMessageCount, runningCounter);
        }
    }
}
