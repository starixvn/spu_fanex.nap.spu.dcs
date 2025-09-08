namespace Fanex.NAP.SPU.DCS.JobService.Services
{
    using Fanex.NAP.SPU.DCS.Features.Transactions;

    public interface ITransactionJobService
    {
        Task<bool> Execute(int numberOfTrans, int numberOfBatch, bool isTransformExtra);

        bool ProcessRawFingerprintQueue(int batchSize);
    }

    public class TransactionJobService : ITransactionJobService
    {
        private readonly ILogService logService;
        private readonly ITransactionServices transactionService;

        private readonly IList<string> IgnoreMessages;
        private const int DelayTimeMilliseconds = 500;

        public TransactionJobService(ILogService logService, ITransactionServices transactionService)
        {
            this.logService = logService;
            this.transactionService = transactionService;
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
                    await JobRun(numberOfTrans, numberOfBatch, isTransformExtra);
                }

                return true;
            }
            catch (Exception ex)
            {
                if (!this.IgnoreMessages.Contains(ex.Message))
                {
                    await logService.ErrorAsync(ex);

                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public bool ProcessRawFingerprintQueue(int batchSize)
        {
            try
            {
                this.transactionService.ProcessRawFingerprintQueue(batchSize);
                return true;
            }
            catch (Exception ex)
            {
                logService.Error(ex);
                return false;
            }

            return true;
        }

        private async Task JobRun(int numberOfTrans, int numberOfBatch, bool isTransformExtra)
        {
            await TransformTransaction(numberOfTrans, numberOfBatch, isTransformExtra);
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
                               .UserAgentProcessing(trans.MinTransId, trans.MaxTransId, isReadyDbExtra)
                               .TransactionProcessing(trans.MinTransId, trans.MaxTransId, isReadyDbExtra);
                    });

                    await Task.Delay(DelayTimeMilliseconds);

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
