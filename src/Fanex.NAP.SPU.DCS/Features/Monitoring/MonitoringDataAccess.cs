using Fanex.Data.Repository;
using Fanex.NAP.SPU.DCS.Features.Monitoring.Criteria;
using System;

namespace Fanex.NAP.SPU.DCS.Features.Monitoring
{
    public interface IMonitoringDataAccess
    {
        void RawTransSumDailyInsert(DateTime transDate);
    }
    public class MonitoringDataAccess : IMonitoringDataAccess
    {
        private readonly IDynamicRepository repository;

        public MonitoringDataAccess()
        {
            repository = new DynamicRepository();
        }

        public void RawTransSumDailyInsert(DateTime transDate)
        {
            var criteria = new RawTransSumDailyInsertCriteria
            {
               TransDate = transDate
            };

            repository.Execute(criteria);
        }
    }
}
