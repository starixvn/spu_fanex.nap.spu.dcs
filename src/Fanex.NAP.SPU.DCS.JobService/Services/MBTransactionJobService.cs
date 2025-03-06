namespace Fanex.NAP.SPU.DCS.JobService.Services
{
    using Fanex.NAP.SPU.DCS.Features.MBTransactions;
    using Fanex.NAP.SPU.DCS.Features.MBTransactions.Entities;

    public interface IMBTransactionJobService
    {
        Task<bool> Execute(int numberOfTrans, int numberOfBatch);
    }

    public class MBTransactionJobService : IMBTransactionJobService
    {
        private readonly ILogService logService;
        private readonly IMBTransactionServices transactionService;

        private const int DelayTimeMilliseconds = 100;

        public MBTransactionJobService(ILogService logService, IMBTransactionServices transactionService)
        {
            this.logService = logService ?? throw new ArgumentNullException(nameof(logService));
            this.transactionService = transactionService ?? throw new ArgumentNullException(nameof(transactionService));
        }

        public async Task<bool> Execute(int numberOfTrans, int numberOfBatch)
        {
            if (numberOfTrans <= 0 || numberOfBatch <= 0)
            {
                await logService.ErrorAsync(new ArgumentException("NumberOfTrans and NumberOfBatch must be greater than zero."));
                return false;
            }

            try
            {
                await MBTransformTransactionAsync(numberOfTrans, numberOfBatch);
                return true;
            }
            catch (Exception ex)
            {
                await logService.ErrorAsync(ex);
                return false;
            }
        }

        private async Task MBTransformTransactionAsync(int numberOfTrans, int numberOfBatch)
        {
            var rawTransactions = transactionService.GetMBRawTransaction(numberOfTrans, numberOfBatch);

            if (rawTransactions == null || !rawTransactions.Any())
            {
                return;
            }

            var tasks = rawTransactions.Select(ProcessTransactionAsync).ToList();

            await Task.WhenAll(tasks);
        }

        private async Task ProcessTransactionAsync(MBRawTransactionEntity transaction)
        {
            try
            {
                transactionService.InsertMBTransaction(transaction.MinTransID, transaction.MaxTransID);
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
