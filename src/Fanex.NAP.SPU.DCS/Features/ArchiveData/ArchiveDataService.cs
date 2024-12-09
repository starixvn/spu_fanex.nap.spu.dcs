namespace Fanex.NAP.SPU.DCS.Features.ArchiveData
{
    public interface IArchiveDataService
    {
        void UpdateModifiedDate(int batchSize, bool isTransformExtra);

        void UpdateMBLastAccessedDate(int batchSize);

        void DeleteBotComponent(int retentionDays, bool isTransformExtra);
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

        public void UpdateMBLastAccessedDate(int batchSize)
        {
            archiveDataAccess.UpdateMBLastAccessedDate(batchSize);
        }

        public void DeleteBotComponent(int retentionDays, bool isTransformExtra)
        {
            archiveDataAccess.DeleteBotComponent(retentionDays, isTransformExtra);
        }
    }
}
