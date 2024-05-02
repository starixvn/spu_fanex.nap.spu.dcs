using System.Threading.Tasks;

namespace Fanex.NAP.SPU.DCS.Features.ArchiveData
{
    public interface IArchiveDataService
    {
        void UpdateModifiedDate(int batchSize, bool isTransformExtra);

        void DeleteBotComponent(int retentionDays, bool isTransformExtra);

        Task<int> CookArchiveAccountAsync(int batchSize);

        Task RecheckArchiveAccountAsync();

        Task<int> ArchiveAccountAsync(int batchSize);

        Task<int> CookArchiveDeviceAsync(int batchSize);

        Task<int> ArchiveDeviceAsync(int batchSize);
    }

    public class ArchiveDataService : IArchiveDataService
    {
        private readonly IArchiveDataDataAccess archiveDataAccess;

        public ArchiveDataService(IArchiveDataDataAccess archiveDataAccess)
        {
            this.archiveDataAccess = archiveDataAccess;
        }

        public void UpdateModifiedDate(int batchSize, bool isTransformExtra)
        {
            archiveDataAccess.UpdateModifiedDate(batchSize, isTransformExtra);
        }

        public void DeleteBotComponent(int retentionDays, bool isTransformExtra)
        {
            archiveDataAccess.DeleteBotComponent(retentionDays, isTransformExtra);
        }

        public async Task<int> CookArchiveAccountAsync(int batchSize)
        {
            return await archiveDataAccess.CookArchiveAccountAsync(batchSize);
        }

        public async Task RecheckArchiveAccountAsync()
        {
            await archiveDataAccess.RecheckArchiveAccountAsync();
        }

        public async Task<int> ArchiveAccountAsync(int batchSize)
        {
            return await archiveDataAccess.ArchiveAccountAsync(batchSize);
        }

        public async Task<int> CookArchiveDeviceAsync(int batchSize)
        {
            return await archiveDataAccess.CookArchiveDeviceAsync(batchSize);
        }

        public async Task<int> ArchiveDeviceAsync(int batchSize)
        {
            return await archiveDataAccess.ArchiveDeviceAsync(batchSize);
        }
    }
}
