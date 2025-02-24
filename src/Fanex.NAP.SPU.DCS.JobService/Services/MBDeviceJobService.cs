using Fanex.NAP.SPU.DCS.Features.MBDevice;
using Fanex.NAP.SPU.DCS.Features.MBDevice.Entities;

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

            if (transList == null || !transList.Any())
            {
                return;
            }

            var groupByBatchTrans = transList.GroupBy(x => x.BatchID);
            var tasks = new List<Task>();

            foreach (var transInfo in groupByBatchTrans)
            {
                tasks.Add(ProcessTransactionBatch(transInfo));
                await Task.Delay(delayTime);
            }

            await Task.WhenAll(tasks.ToArray());
        }

        private async Task ProcessTransactionBatch(IGrouping<int, MBTransactionEntity> transInfo)
        {
            try
            {
                if (transInfo == null || !transInfo.Any())
                {
                    return;
                }

                var transactionInfoJson = JsonSerializer.Serialize(transInfo);
                deviceService.TransformDeviceInsert(transactionInfoJson);
            }
            catch (Exception ex)
            {
                await logService.ErrorAsync(ex);
                throw;
            }
        }
    }
}
