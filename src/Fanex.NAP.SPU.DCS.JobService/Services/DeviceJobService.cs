namespace Fanex.NAP.SPU.DCS.JobService.Services
{
    using Fanex.NAP.SPU.DCS.Features.Devices;
    using System.Net.Http;
    using System.Threading.Tasks;

    public interface IDeviceJobService
    {
        Task<bool> Execute(int size, int numberOfBatch);
    }

    public class DeviceJobService : IDeviceJobService
    {
        private readonly ILogService logService;
        private readonly IDeviceServices deviceService;
        private const int delayTime = 100; // milliseconds

        public DeviceJobService(ILogService logService, IDeviceServices deviceService, HttpClient httpClient)
        {
            this.logService = logService;
            this.deviceService = deviceService;

            httpClient.Timeout = TimeSpan.FromSeconds(10);
        }

        public async Task<bool> Execute(int size, int numberOfBatch)
        {
            try
            {
                if (size <= 0 || numberOfBatch <= 0)
                {
                    await logService.ErrorAsync(new Exception("TransformDevice BatchSize = 0!"));
                    return false;
                }

                var transformResult = await TransformDevice(size, numberOfBatch);

                if (transformResult)
                {
                    var updateResult = deviceService.UpdateMaxTransIDTransformDevice();
                    if (updateResult < 0)
                    {
                        await logService.ErrorAsync(new Exception($"UpdateMaxTransIDTransformDevice return value: {updateResult}!"));

                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                await logService.ErrorAsync(ex);

                return false;
            }

            return true;
        }

        private async Task<bool> TransformDevice(int size, int numberOfBatch)
        {
            var stagingTransIds = await deviceService.GetStagingTransIdForDeviceAsync(size, numberOfBatch);
            if (stagingTransIds == null || !stagingTransIds.Any())
            {
                return false;
            }

            var groupTrans = stagingTransIds.GroupBy(x => x.BatchId);
            var tasks = new List<Task>();

            foreach (var trans in groupTrans)
            {
                var task = Task.Run(async () =>
                {
                    try
                    {
                        await deviceService.TransformDeviceInsertAsync(trans);
                    }
                    catch (Exception ex)
                    {
                        await logService.ErrorAsync(ex);
                        throw;
                    }
                });

                await Task.Delay(delayTime);

                tasks.Add(task);
            }

            await Task.WhenAll(tasks.ToArray());

            return true;
        }
    }
}
