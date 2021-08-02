using Fanex.NAP.SPU.DCS.Features.Devices;
using Newtonsoft.Json;
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

        public async Task<bool> Execute(int size, int numberOfBatch)
        {
            try
            {
                if (size > 0 && numberOfBatch > 0)
                {
                    var task = JobRun(size, numberOfBatch);

                    await task;
                }
            }
            catch (Exception ex)
            {
                sentryClient.CaptureException(ex);

                return false;
            }

            return true;
        }

        private async Task JobRun(int size, int numberOfBatch)
        {
            var stagingTransIds = deviceService.GetStagingTransIdForDevice(size, numberOfBatch);

            if (stagingTransIds != null && stagingTransIds.Any())
            {
                var groupTrans = stagingTransIds.GroupBy(x => x.BatchId);
                var tasks = new List<Task>();

                foreach (var trans in groupTrans)
                {
                    var task = Task.Run(() =>
                    {
                        try
                        {
                            deviceService.UpdateDevices(trans);                                                        
                        }
                        catch (Exception ex)
                        {
                            sentryClient.CaptureException(ex);
                        }
                    });

                    await Task.Delay(delayTime);

                    tasks.Add(task);                                        
                }

                var taskCompleted = Task.WhenAll(tasks.ToArray());

                await taskCompleted;
            }
        }
    }
}
