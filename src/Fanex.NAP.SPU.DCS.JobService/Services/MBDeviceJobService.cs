using Fanex.NAP.SPU.DCS.Features.MBDevice;

namespace Fanex.NAP.SPU.DCS.JobService.Services
{
    public interface IMBDeviceJobService
    {
        Task<bool> Execute(int limitTotalRecord, int numberOfBatch);
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

        public async Task<bool> Execute(int limitTotalRecord, int numberOfBatch)
        {
            try
            {
                if (limitTotalRecord > 0 && numberOfBatch > 0)
                {
                    await MBTransformDevice(limitTotalRecord, numberOfBatch);
                }
                else
                {
                    await logService.ErrorAsync(new Exception("MBTransformDevice BatchSize = 0!"));

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

        private async Task MBTransformDevice(int limitTotalRecord, int numberOfBatch)
        {
            var transList = deviceService.GetMBTransaction(limitTotalRecord, numberOfBatch);

            if (transList != null && transList.Any())
            {
                var groupByBatchTrans = transList.GroupBy(x => x.BatchID);

                var tasks = new List<Task>();

                foreach (var transInfo in groupByBatchTrans)
                {
                    var task = Task.Run(async () =>
                    {
                        try
                        {
                            if (transInfo != null && transInfo.Any())
                            {
                                var transactionInfoJson = JsonSerializer.Serialize(transInfo);
                                deviceService.TransformDeviceInsert(transactionInfoJson);
                            }    
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
