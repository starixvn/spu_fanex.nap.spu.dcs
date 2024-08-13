namespace Fanex.NAP.SPU.DCS.JobService.Services
{
    using Fanex.NAP.SPU.DCS.Features.Devices;
    using System.Threading.Tasks;

    public interface IDeviceJobService
    {
        Task<bool> Execute(int size, int numberOfBatch, bool isReadyDbExtra);

        Task<bool> DeviceEvolution(int batchSize, bool isReadyDbExtra);
    }

    public class DeviceJobService : IDeviceJobService
    {
        private readonly ILogService logService;
        private readonly IDeviceServices deviceService;

        private const int delayTime = 100; // milliseconds

        public DeviceJobService(ILogService logService, IDeviceServices deviceService)
        {
            this.logService = logService;
            this.deviceService = deviceService;
        }

        public async Task<bool> Execute(int size, int numberOfBatch, bool isReadyDbExtra)
        {
            try
            {
                if (size > 0 && numberOfBatch > 0)
                {
                    await deviceService.InsertFPDeviceMappingAsync(size * numberOfBatch, isReadyDbExtra);
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

        public async Task<bool> DeviceEvolution(int batchSize, bool isReadyDbExtra)
        {
            try
            {
                if (batchSize > 0)
                {
                    var resultDeviceEvolution = await deviceService.GetFPDeviceEvolutionAsync(batchSize, isReadyDbExtra);

                    var dataEvolution = resultDeviceEvolution.Item1;
                    var dataEvolutionAttribute = resultDeviceEvolution.Item2;

                    if (dataEvolution != null && dataEvolution.Any())
                    {
                        foreach (var itemEvolution in dataEvolution)
                        {
                            if (!string.IsNullOrEmpty(itemEvolution.FPDeviceFingerprintAccountIDRef))
                            {
                                var refIds = itemEvolution.FPDeviceFingerprintAccountIDRef
                                                      .Split(',')
                                                      .Select(ulong.Parse)
                                                      .ToList();

                                var matchingRecords = dataEvolutionAttribute
                                    .Where(attrEntity => refIds.Contains(attrEntity.FPDeviceFingerprintAccountID))
                                    .ToList();

                            }
                        }
                    }
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
