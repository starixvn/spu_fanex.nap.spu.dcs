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
        Task<bool> Execute(int size, int numberOfBatch, bool isTransformExtra);
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

        public async Task<bool> Execute(int size, int numberOfBatch, bool isTransformExtra)
        {
            try
            {
                if (size > 0 && numberOfBatch > 0)
                {                    
                    JobRun(size, numberOfBatch, isTransformExtra);    
                }              
            }
            catch (Exception ex)
            {
                sentryClient.CaptureException(ex);

                return await Task.FromResult(false);
            }

            return await Task.FromResult(true);
        }

        private void JobRun(int size, int numberOfBatch, bool isTransformExtra)
        {
            if (isTransformExtra)
            {
                if(appSettings.IsReadyDbExtra)
                    TransformDevice(size, numberOfBatch, true).Wait();
            }
            else
            {
                TransformDevice(size, numberOfBatch, false).Wait();
            }          
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
