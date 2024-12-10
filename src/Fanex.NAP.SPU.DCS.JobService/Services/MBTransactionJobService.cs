namespace Fanex.NAP.SPU.DCS.JobService.Services
{
    using Fanex.NAP.SPU.DCS.Features.MBTransactions;

    public interface IMBTransactionJobService
    {
        Task<bool> Execute(int numberOfTrans, int numberOfBatch);
    }

    public class MBTransactionJobService : IMBTransactionJobService
    {
        private readonly ILogService logService;
        private readonly IMBTransactionServices transactionService;

        private const int delayTime = 100; // milliseconds

        public MBTransactionJobService(ILogService logService, IMBTransactionServices transactionService)
        {
            this.logService = logService;
            this.transactionService = transactionService;
        }

        public async Task<bool> Execute(int numberOfTrans, int numberOfBatch)
        {
            try
            {
                if (numberOfTrans > 0 && numberOfBatch > 0)
                    await MBTransformTransaction(numberOfTrans, numberOfBatch);

                return true;
            }
            catch (Exception ex)
            {
                await logService.ErrorAsync(ex);

                return false;
            }
        }

        private async Task MBTransformTransaction(int numberOfTrans, int numberOfBatch)
        {
            var rawTrans = transactionService.GetMBRawTransaction(numberOfTrans, numberOfBatch);

            var tasks = new List<Task>();
            foreach (var trans in rawTrans)
            {
                var task = Task.Run(() =>
                {
                    transactionService.InsertMBTransaction(trans.MinTransID, trans.MaxTransID);
                });

                await Task.Delay(delayTime);

                tasks.Add(task);
            }

            await Task.WhenAll(tasks.ToArray());
        }
    }
}
