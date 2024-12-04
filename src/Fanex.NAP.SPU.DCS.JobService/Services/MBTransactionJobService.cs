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

        private readonly IList<string> IgnoreMessages;
        private const int delayTime = 500; // milliseconds

        public MBTransactionJobService(ILogService logService, IMBTransactionServices transactionService)
        {
            this.logService = logService;
            this.transactionService = transactionService;

            IgnoreMessages = new List<string> {
                "Deadlock found when trying to get lock; try restarting transaction"
            };
        }

        public async Task<bool> Execute(int numberOfTrans, int numberOfBatch)
        {
            try
            {
                if (numberOfTrans > 0 && numberOfBatch > 0)
                    await TransformTransaction(numberOfTrans, numberOfBatch);

                return await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                if (!this.IgnoreMessages.Contains(ex.Message))
                {
                    await logService.ErrorAsync(ex);

                    return await Task.FromResult(false);
                }
                else
                {
                    return await Task.FromResult(true);
                }
            }
        }

        private async Task TransformTransaction(int numberOfTrans, int numberOfBatch)
        {
            var rawTrans = this.transactionService.GetRawTransaction(numberOfTrans, numberOfBatch);

            var tasks = new List<Task>();
            foreach (var trans in rawTrans)
            {
                try
                {
                    var task = Task.Run(() =>
                    {
                        this.transactionService
                               .TransactionProcessing(trans.MinTransId, trans.MaxTransId);
                    });

                    await Task.Delay(delayTime);

                    tasks.Add(task);
                }
                catch (Exception ex)
                {
                    await logService.ErrorAsync(ex);

                    throw;
                }
            }

            await Task.WhenAll(tasks.ToArray());
        }
    }
}
