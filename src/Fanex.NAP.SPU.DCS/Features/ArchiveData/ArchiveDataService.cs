namespace Fanex.NAP.SPU.DCS.Features.ArchiveData
{
    public interface IArchiveDataService
    {
        void UpdateModifiedDate(int batchSize, bool isTransformExtra);

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

        public void DeleteBotComponent(int retentionDays, bool isTransformExtra)
        {
            archiveDataAccess.DeleteBotComponent(retentionDays, isTransformExtra);
        }
    }
}
