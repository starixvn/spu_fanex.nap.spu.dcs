using Fanex.NAP.SPU.DCS.Features.MBDevice;
using Fanex.NAP.SPU.DCS.Features.MBDevice.Entities;

namespace Fanex.NAP.SPU.DCS.JobService.Services
{
    public interface IMBDeviceJobService
    {
        Task<bool> Execute(int numberOfTrans, int numberOfBatch);
    }

    public class MBDeviceJobService : IMBDeviceJobService
    {
        private readonly ILogService logService;
        private readonly IMBDeviceServices deviceService;

        private const int DelayTimeMilliseconds = 100;

        public MBDeviceJobService(ILogService logService, IMBDeviceServices deviceService)
        {
            this.logService = logService ?? throw new ArgumentNullException(nameof(logService));
            this.deviceService = deviceService ?? throw new ArgumentNullException(nameof(deviceService));
        }

        public async Task<bool> Execute(int numberOfTrans, int numberOfBatch)
        {
            if (numberOfTrans <= 0 || numberOfBatch <= 0)
            {
                await logService.ErrorAsync(new ArgumentException("LimitTotalRecord and NumberOfBatch must be greater than zero."));
                return false;
            }

            try
            {
                await MBTransformDeviceAsync(numberOfTrans, numberOfBatch);

                var updateResult = deviceService.UpdateMaxMBTransIDTransformDevice();
                if (updateResult < 0)
                {
                    await logService.ErrorAsync(new Exception($"UpdateMaxMBTransIDTransformDevice return value: {updateResult}!"));

                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                await logService.ErrorAsync(ex);
                return false;
            }
        }

        private async Task MBTransformDeviceAsync(int numberOfTrans, int numberOfBatch)
        {
            var transactions = deviceService.GetMBTransaction(numberOfTrans, numberOfBatch);

            if (transactions == null || !transactions.Any())
            {
                return;
            }

            var groupedTransactions = transactions.GroupBy(x => x.BatchID);
            var tasks = groupedTransactions.Select(ProcessTransactionBatchAsync).ToList();

            await Task.WhenAll(tasks);
        }

        private async Task ProcessTransactionBatchAsync(IGrouping<int, MBTransactionEntity> transactionGroup)
        {
            if (transactionGroup == null || !transactionGroup.Any())
            {
                return;
            }

            try
            {
                var transactionInfoJson = JsonSerializer.Serialize(transactionGroup);
                deviceService.TransformDeviceInsert(transactionInfoJson);
                await Task.Delay(DelayTimeMilliseconds);
            }
            catch (Exception ex)
            {
                await logService.ErrorAsync(ex);
                throw;
            }
        }
    }
}
