using Fanex.NAP.SPU.DCS.Features.Monitoring;
using Sentry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fanex.NAP.SPU.DCS.JobService.Services
{
    public interface IMonitoringJobService
    {
        bool Execute();              

    }

    public class MonitoringJobService : IMonitoringJobService
    {
        private readonly ISentryClient sentryClient;
        private readonly IMonitoringServices monitoringServices;

        public MonitoringJobService(IMonitoringServices monitoringServices, ISentryClient sentryClient)
        {
            this.sentryClient = sentryClient;
            this.monitoringServices = monitoringServices;
        }

        public bool Execute()
        {
            try
            {
                var transDate = DateTime.Now.ToString("yyyy-MM-dd");

                this.monitoringServices.RawTransSumDailyInsert(DateTime.Now);

                return true;
            }
            catch(Exception ex)
            {
                sentryClient.CaptureException(ex);

                return false;
            }
        }
    }
}
