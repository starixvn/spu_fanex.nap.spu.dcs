using Fanex.Data.Repository;
using Fanex.NAP.SPU.DCS.Features.ArchiveData.Criteria;

namespace Fanex.NAP.SPU.DCS.Features.ArchiveData
{
    public interface IArchiveDataDataAccess
    {
        void UpdateModifiedDate(int batchSize, bool isTransformExtra);

        void DeleteBotComponent(int retentionDays, bool isTransformExtra);
    }

    public class ArchiveDataDataAccess : IArchiveDataDataAccess
    {
        private readonly IDynamicRepository repository;

        public ArchiveDataDataAccess()
        {
            repository = new DynamicRepository();
        }

        public void UpdateModifiedDate(int batchSize, bool isTransformExtra)
        {
            var criteria = new UpdateModifiedDateCriteria
            {
                BatchSize = batchSize,
                IsReadyDbExtra = isTransformExtra
            };

            repository.Execute(criteria);
        }

        public void DeleteBotComponent(int retentionDays, bool isTransformExtra)
        {
            var criteria = new DeleteBotComponentCriteria
            {
                RetentionDays = retentionDays,
                IsReadyDbExtra = isTransformExtra
            };

            repository.Execute(criteria);
        }
    }
}
