namespace Fanex.NAP.SPU.DCS.JobService.Services
{
    using Fanex.NAP.SPU.DCS.Features.FPDevices;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public interface IFPDeviceJobService
    {
        Task<bool> Execute(int size, int numberOfBatch);
    }

    public class FPDeviceJobService : IFPDeviceJobService
    {
        private readonly ISentryClient sentryClient;
        private readonly IFPDeviceServices deviceService;
        private const int delayTime = 100; // milliseconds

        public FPDeviceJobService(ISentryClient sentryClient, IFPDeviceServices deviceService)
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
