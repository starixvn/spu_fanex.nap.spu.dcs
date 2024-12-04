using Fanex.NAP.SPU.DCS.Features.MBDevice;

namespace Fanex.NAP.SPU.DCS.JobService.Services
{
    public interface IMBDeviceJobService
    {
        Task<bool> Execute(int size, int numberOfBatch);
    }

    public class MBDeviceJobService : IMBDeviceJobService
    {
        private readonly ILogService logService;
        private readonly IMBDeviceServices deviceService;

        private const int delayTime = 100; // milliseconds
        public MBDeviceJobService(ILogService logService, IMBDeviceServices deviceService)
        {
            this.logService = logService;
            this.deviceService = deviceService;
        }

        public async Task<bool> Execute(int size, int numberOfBatch)
        {
            try
            {
                if (size > 0 && numberOfBatch > 0)
                {
                    await TransformDevice(size, numberOfBatch);
                }
                else
                {
                    await logService.ErrorAsync(new Exception("TransformDevice BatchSize = 0!"));

                    return await Task.FromResult(false);
                }
            }
            catch (Exception ex)
            {
                await logService.ErrorAsync(ex);

                return await Task.FromResult(false);
            }

            return await Task.FromResult(true);
        }

        private async Task TransformDevice(int size, int numberOfBatch)
        {
            var transIds = deviceService.GetTransaction(size, numberOfBatch);

            if (transIds != null && transIds.Any())
            {
                var groupTrans = transIds.GroupBy(x => x.BatchId);

                var tasks = new List<Task>();

                foreach (var trans in groupTrans)
                {
                    var task = Task.Run(async () =>
                    {
                        try
                        {
                            deviceService.TransformDeviceInsert(trans);
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
