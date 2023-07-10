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
        private readonly IAppSettings appSettings;
        private const int delayTime = 100; // milliseconds

        public DeviceJobService(ISentryClient sentryClient, IDeviceServices deviceService, IAppSettings appSettings)
        {
            this.sentryClient = sentryClient;
            this.deviceService = deviceService;
            this.appSettings = appSettings;
        }

        public async Task<bool> Execute(int size, int numberOfBatch)
        {
            try
            {
                if (size > 0 && numberOfBatch > 0)
                {                    
                    await JobRun(size, numberOfBatch);    
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
            var tasks = new List<Task>();
            
            if (appSettings.IsReadyDbExtra)
            {
                tasks.Add(Task.Run(() =>
                {
                    TransformDevice(size, numberOfBatch, true).Wait();
                }));
            }
            tasks.Add(Task.Run(() =>
            {
                TransformDevice(size, numberOfBatch, false).Wait();
            }));

            await Task.WhenAll(tasks.ToArray());
        }

        private async Task TransformDevice(int size, int numberOfBatch, bool isReadyDbExtra)
        {
            var stagingTransIds = deviceService.GetStagingTransIdForDevice(size, numberOfBatch, isReadyDbExtra);

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
                            deviceService.UpdateDevices(trans, isReadyDbExtra);
                        }
                        catch (Exception ex)
                        {
                            sentryClient.CaptureException(ex);
                        }
                    });

                    await Task.Delay(delayTime);

                    tasks.Add(task);
                }

                await Task.WhenAll(tasks.ToArray());                
            }
        }

    }
}
