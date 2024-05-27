namespace Fanex.NAP.SPU.DCS.JobService.Services
{
    using Fanex.NAP.SPU.DCS.Features.ArchiveData;
    using System.Net.WebSockets;

    public interface IArchiveJobServices
    {
        Task<bool> Execute(int batchSize, bool isTransformExtra);

        Task<bool> DeleteBotComponent(int retentionDays, bool isTransformExtra);

        Task<bool> CookArchiveAccountAsync(int batchSize, int numberOfBatch);

        Task<bool> ArchiveAccountAsync(int batchSize);

        Task<bool> CookArchiveDeviceAsync(int batchSize, int numberOfBatch);

        Task<bool> ArchiveDeviceAsync(int batchSize);
    }

    public class ArchiveJobServices : IArchiveJobServices
    {
        private readonly IArchiveDataService archiveDataService;
        private readonly ILogService logService;

        public ArchiveJobServices(IArchiveDataService archiveDataService, ILogService logService)
        {
            this.archiveDataService = archiveDataService;
            this.logService = logService;
        }

        public Task<bool> Execute(int batchSize, bool isTransformExtra)
        {
            try
            {
                if (batchSize > 0)
                {
                    archiveDataService.UpdateModifiedDate(batchSize, isTransformExtra);
                }
                else
                {
                    logService.ErrorAsync(new Exception("ArchiveData invalid Batch size"));
                    return Task.FromResult(false);
                }
            }
            catch (Exception ex)
            {
                logService.ErrorAsync(ex);
                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        public Task<bool> DeleteBotComponent(int retentionDays, bool isTransformExtra)
        {
            try
            {
                if (retentionDays >= 0)
                {
                    archiveDataService.DeleteBotComponent(retentionDays, isTransformExtra);
                }
                else
                {
                    logService.ErrorAsync(new Exception("DeleteBotComponent invalid Retention days"));
                    return Task.FromResult(false);
                }
            }
            catch (Exception ex)
            {
                logService.ErrorAsync(ex);
                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        public async Task<bool> CookArchiveAccountAsync(int batchSize, int numberOfBatch)
        {
            try
            {
                var i = 0;
                var shouldContinue = 1;
                while (i < numberOfBatch && shouldContinue == 1)
                {
                    shouldContinue = await archiveDataService.CookArchiveAccountAsync(batchSize);
                    await Task.Delay(100);
                    i++;
                }
            }
            catch (Exception ex)
            {
                await logService.ErrorAsync(ex);
                return false;
            }

            return true;
        }

        public async Task<bool> ArchiveAccountAsync(int batchSize)
        {
            try
            {
                await archiveDataService.CheckReActiveArchiveAccountAsync();

                var shouldContinue = 1;
                while (shouldContinue > 0)
                {
                    shouldContinue = await archiveDataService.ArchiveAccountAsync(batchSize);
                }
            }
            catch (Exception ex)
            {
                await logService.ErrorAsync(ex);
                return false;
            }

            return true;
        }

        public async Task<bool> CookArchiveDeviceAsync(int batchSize, int numberOfBatch)
        {
            try
            {
                var i = 0;
                var shouldContinue = 1;
                while (i < numberOfBatch && shouldContinue == 1)
                {
                    shouldContinue = await archiveDataService.CookArchiveDeviceAsync(batchSize);
                    await Task.Delay(100);
                    i++;
                }
            }
            catch (Exception ex)
            {
                await logService.ErrorAsync(ex);
                return false;
            }

            return true;
        }

        public async Task<bool> ArchiveDeviceAsync(int batchSize)
        {
            try
            {
                await archiveDataService.CheckReActiveArchiveDeviceAsync();

                var shouldContinue = 1;
                while (shouldContinue > 0)
                {
                    shouldContinue = await archiveDataService.ArchiveDeviceAsync(batchSize);
                }
            }
            catch (Exception ex)
            {
                await logService.ErrorAsync(ex);
                return false;
            }

            return true;
        }
    }
}
