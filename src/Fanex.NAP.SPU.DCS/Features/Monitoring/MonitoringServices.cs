using System;

namespace Fanex.NAP.SPU.DCS.Features.Monitoring
{
    public interface IMonitoringServices
    {
        void RawTransSumDailyInsert(DateTime transDate);
    }
    public class MonitoringServices : IMonitoringServices
    {
        private readonly IMonitoringDataAccess monitoringDataAccess;

        public MonitoringServices(IMonitoringDataAccess monitoringDataAccess)
        {
            this.monitoringDataAccess = monitoringDataAccess;
        }

        public void RawTransSumDailyInsert(DateTime transDate)
        {
            monitoringDataAccess.RawTransSumDailyInsert(transDate);
        }
    }
}
