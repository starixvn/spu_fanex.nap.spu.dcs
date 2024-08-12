namespace Fanex.NAP.SPU.DCS.JobService.Services
{
    using Fanex.NAP.SPU.DCS.Features.Devices;
    using System.Net.Http;
    using System.Threading.Tasks;

    public interface IDeviceJobService
    {
        Task<bool> Execute(int size, int numberOfBatch, bool isReadyDbExtra);
    }

    public class DeviceJobService : IDeviceJobService
    {
        private readonly ILogService logService;
        private readonly IDeviceServices deviceService;
        private readonly IAppSettings appSettings;
        private readonly HttpClient httpClient;
        private readonly bool isExtra = false;

        private const int delayTime = 100; // milliseconds

        private readonly int maxChangesRule5 = 2;
        private readonly int maxChangesRule6 = 1;

        public DeviceJobService(ILogService logService, IDeviceServices deviceService, IAppSettings appSettings, HttpClient httpClient)
        {
            this.logService = logService;
            this.deviceService = deviceService;
            this.appSettings = appSettings;
            this.httpClient = httpClient;

            httpClient.Timeout = TimeSpan.FromSeconds(10);
        }

        public async Task<bool> Execute(int size, int numberOfBatch, bool isReadyDbExtra)
        {
            try
            {
                if (size > 0 && numberOfBatch > 0)
                {
                    await TransformDevice(size, numberOfBatch, isReadyDbExtra);
                }
            }
            catch (Exception ex)
            {
                await logService.ErrorAsync(ex);

                return await Task.FromResult(false);
            }

            return await Task.FromResult(true);
        }

        private async Task TransformDevice(int size, int numberOfBatch, bool isReadyDbExtra)
        {
            var stagingTransIds = await deviceService.GetStagingTransIdForDeviceAsync(size, numberOfBatch, isReadyDbExtra);

            if (stagingTransIds != null && stagingTransIds.Any())
            {
                var groupTrans = stagingTransIds.GroupBy(x => x.BatchId);

                var tasks = new List<Task>();

                foreach (var trans in groupTrans)
                {
                    var task = Task.Run(async () =>
                    {
                        try
                        {
                            await deviceService.TransformDeviceInsertAsync(trans, isReadyDbExtra);
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
            }
        }
    }
}
