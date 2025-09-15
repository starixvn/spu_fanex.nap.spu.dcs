using Fanex.Data.Repository;
using Fanex.NAP.SPU.DCS.Features.ArchiveData.Criteria;

namespace Fanex.NAP.SPU.DCS.Features.ArchiveData
{
    public interface IArchiveDataDataAccess
    {
        void UpdateModifiedDate(int batchSize);

        void UpdateMBLastAccessedDate(int batchSize);

        void DeleteBotComponent(int retentionDays);
    }

    public class ArchiveDataDataAccess : IArchiveDataDataAccess
    {
        private readonly IDynamicRepository repository;

        public ArchiveDataDataAccess(IDynamicRepository repository)
        {
            this.repository = repository ?? new DynamicRepository();
        }

        public void UpdateModifiedDate(int batchSize)
        {
            var criteria = new UpdateModifiedDateCriteria
            {
                BatchSize = batchSize
            };

            repository.Execute(criteria);
        }

        public void UpdateMBLastAccessedDate(int batchSize)
        {
            var criteria = new UpdateMBLastAccessedDateCriteria
            {
                BatchSize = batchSize
            };

            repository.Execute(criteria);
        }

        public void DeleteBotComponent(int retentionDays)
        {
            var criteria = new DeleteBotComponentCriteria
            {
                RetentionDays = retentionDays
            };

            repository.Execute(criteria);
        }
    }
}
