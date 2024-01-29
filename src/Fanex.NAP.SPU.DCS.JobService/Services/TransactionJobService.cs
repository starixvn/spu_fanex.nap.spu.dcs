namespace Fanex.NAP.SPU.DCS.JobService.Services
{
    using Fanex.NAP.SPU.DCS.Features.Transactions;

    public interface ITransactionJobService
    {
        Task<bool> Execute(int numberOfTrans, int numberOfBatch, bool isTransformExtra);
    }

    public class TransactionJobService : ITransactionJobService
    {
        private readonly ILogService logService;
        private readonly ITransactionServices transactionService;
        private readonly IAppSettings appSettings;

        private readonly IList<string> IgnoreMessages;
        private const int delayTime = 500; // milliseconds

        public TransactionJobService(ILogService logService, ITransactionServices transactionService, IAppSettings appSettings)
        {
            this.logService = logService;
            this.transactionService = transactionService;
            this.appSettings = appSettings;
            IgnoreMessages = new List<string> {
                "Deadlock found when trying to get lock; try restarting transaction"
            };
        }

        public async Task<bool> Execute(int numberOfTrans, int numberOfBatch, bool isTransformExtra)
        {
            try
            {
                if (numberOfTrans > 0 && numberOfBatch > 0)
                {
                    JobRun(numberOfTrans, numberOfBatch, isTransformExtra);
                }

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

        private void JobRun(int numberOfTrans, int numberOfBatch, bool isTransformExtra)
        {
            TransformTransaction(numberOfTrans, numberOfBatch, isTransformExtra).Wait();
        }

        private async Task TransformTransaction(int numberOfTrans, int numberOfBatch, bool isReadyDbExtra)
        {
            var rawTrans = this.transactionService.GetRawTransaction(0, numberOfTrans, numberOfBatch, isReadyDbExtra);

            var tasks = new List<Task>();
            foreach (var trans in rawTrans)
            {
                try
                {
                    var task = Task.Run(() =>
                    {
                        this.transactionService
                               .UnhandleSubscriberAccountProcessing(trans.MinTransId, trans.MaxTransId, isReadyDbExtra)
                               .UserAgentProcessing(trans.MinTransId, trans.MaxTransId, isReadyDbExtra)
                               .TransactionProcessing(trans.MinTransId, trans.MaxTransId, isReadyDbExtra);
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
