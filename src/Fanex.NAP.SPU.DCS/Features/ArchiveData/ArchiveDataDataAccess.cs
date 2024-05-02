using Fanex.Data;
using Fanex.Data.Repository;
using Fanex.NAP.SPU.DCS.Features.AbnormalTransaction.Entities;
using Fanex.NAP.SPU.DCS.Features.AbnormalTransaction.Model;
using Fanex.NAP.SPU.DCS.Features.ArchiveData.Criteria;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fanex.NAP.SPU.DCS.Features.ArchiveData
{
    public interface IArchiveDataDataAccess
    {
        void UpdateModifiedDate(int batchSize, bool isTransformExtra);

        void DeleteBotComponent(int retentionDays, bool isTransformExtra);

        Task<int> CookArchiveAccountAsync(int batchSize);

        Task RecheckArchiveAccountAsync();

        Task<int> ArchiveAccountAsync(int batchSize);

        Task<int> CookArchiveDeviceAsync(int batchSize);

        Task<int> ArchiveDeviceAsync(int batchSize);
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

        public async Task<int> CookArchiveAccountAsync(int batchSize)
        {
            var criteria = new CookArchiveAccountCriteria
            {
                BatchSize = batchSize,
                ShouldContinue = 0
            };

            await repository.ExecuteAsync(criteria);
            return criteria.ShouldContinue;
        }

        public async Task RecheckArchiveAccountAsync()
        {
            var criteria = new RecheckArchiveAccountCriteria {};

            await repository.ExecuteAsync(criteria);
        }

        public async Task<int> ArchiveAccountAsync(int batchSize)
        {
            var criteria = new ArchiveAccountCriteria
            {
                BatchSize = batchSize,
                RowCount = 0
            };

            await repository.ExecuteAsync(criteria);
            return criteria.RowCount;
        }

        public async Task<int> CookArchiveDeviceAsync(int batchSize)
        {
            var criteria = new CookArchiveDeviceCriteria
            {
                BatchSize = batchSize,
                ShouldContinue = 0
            };

            await repository.ExecuteAsync(criteria);
            return criteria.ShouldContinue;
        }

        public async Task<int> ArchiveDeviceAsync(int batchSize)
        {
            var criteria = new ArchiveDeviceCriteria
            {
                BatchSize = batchSize,
                RowCount = 0
            };

            await repository.ExecuteAsync(criteria);
            return criteria.RowCount;
        }
    }
}
