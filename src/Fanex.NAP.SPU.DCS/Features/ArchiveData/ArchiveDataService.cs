namespace Fanex.NAP.SPU.DCS.Features.ArchiveData
{
    public interface IArchiveDataService
    {
        void UpdateModifiedDate(int batchSize);

        void UpdateMBLastAccessedDate(int batchSize);

        void DeleteBotComponent(int retentionDays);
    }

    public class ArchiveDataService : IArchiveDataService
    {
        private readonly IArchiveDataDataAccess archiveDataAccess;

        public ArchiveDataService(IArchiveDataDataAccess archiveDataAccess)
        {
            this.archiveDataAccess = archiveDataAccess;
        }

        public void UpdateModifiedDate(int batchSize)
        {
            archiveDataAccess.UpdateModifiedDate(batchSize);
        }

        public void UpdateMBLastAccessedDate(int batchSize)
        {
            archiveDataAccess.UpdateMBLastAccessedDate(batchSize);
        }

        public void DeleteBotComponent(int retentionDays)
        {
            archiveDataAccess.DeleteBotComponent(retentionDays);
        }
    }
}
