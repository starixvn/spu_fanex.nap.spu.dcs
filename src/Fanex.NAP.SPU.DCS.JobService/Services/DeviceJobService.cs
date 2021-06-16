using Fanex.NAP.SPU.DCS.Features.Devices;
using Sentry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fanex.NAP.SPU.DCS.JobService.Services
{
    public interface IDeviceJobService
    {
        Task<bool> Execute(int size, int numberOfBatch);
    }

    public class DeviceJobService : IDeviceJobService
    {
        private readonly ISentryClient sentryClient;
        private readonly IDeviceServices deviceService;
        private const int delayTime = 100; // milliseconds

        public DeviceJobService(ISentryClient sentryClient, IDeviceServices deviceService)
        {
            this.sentryClient = sentryClient;
            this.deviceService = deviceService;
        }

        public Task<bool> Execute(int size, int numberOfBatch)
        {
            try
            {
                if(size > 0 && numberOfBatch > 0)
                {
                    JobRun(size, numberOfBatch);
                }                               
            }
            catch (Exception ex)
            {
                sentryClient.CaptureException(ex);

                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        private void JobRun(int size, int numberOfBatch)
        {
            var stagingTransIds = deviceService.GetStagingTransIdForDevice(size, numberOfBatch);

            if (stagingTransIds != null && stagingTransIds.Any())
            {
                var groupTrans = stagingTransIds.GroupBy(x => x.BatchId);
                var tasks = new List<Task>();

                groupTrans.ToList().ForEach(x =>
                {
                    var task = Task.Run(() =>
                    {
                        try
                        {
                            deviceService.UpdateDevices(x);
                        }
                        catch (Exception ex)
                        {
                            sentryClient.CaptureException(ex);
                        }
                    });

                    tasks.Add(task);

                    Task.Delay(delayTime);
                });

                Task.WhenAll(tasks.ToArray());
            }
        }
    }
}
