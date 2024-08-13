namespace Fanex.NAP.SPU.DCS.Features.Devices
{
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.DCS.Features.ArchiveData.Criteria;
    using Fanex.NAP.SPU.DCS.Features.Devices.Criteria;
    using Fanex.NAP.SPU.DCS.Features.Devices.Entities;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IDeviceDataAccess
    {
        Task<IEnumerable<StagingTransId>> GetStagingTransIdForDeviceAsync(int size, int noOfBatch, bool isReadyDbExtra);

        Task<(IEnumerable<DeviceEvolutionEntity>, IEnumerable<DeviceEvolutionEntity>)> GetFPDeviceEvolutionAsync(int batchSize, bool isReadyDbExtra);

        Task TransformDeviceInsertAsync(string transIDs, bool isReadyDbExtra);

        Task InsertFPDeviceMappingAsync(int batchSize, bool isTransformExtra);

        Task InsertFPDeviceEvolutionAsync(string deviceEvolutionJson, bool isTransformExtra);
    }

    public class DeviceDataAccess : IDeviceDataAccess
    {
        private readonly IDynamicRepository repository;

        public DeviceDataAccess(IDynamicRepository repository)
        {
            this.repository = repository ?? new DynamicRepository();
        }

        public async Task<IEnumerable<StagingTransId>> GetStagingTransIdForDeviceAsync(int size, int noOfBatch, bool isReadyDbExtra)
        {
            var criteria = new GetDeviceStagingTransactionPackageCriteria
            {
                Size = size,
                NoOfBatch = noOfBatch,
                IsReadyDbExtra = isReadyDbExtra
            };

            return await repository.FetchAsync<StagingTransId>(criteria);
        }

        public async Task<(IEnumerable<DeviceEvolutionEntity>, IEnumerable<DeviceEvolutionEntity>)> GetFPDeviceEvolutionAsync(int batchSize, bool isReadyDbExtra)
        {
            var criteria = new GetFPDeviceEvolutionCriteria
            {
                BatchSize = batchSize,
                IsReadyDbExtra = isReadyDbExtra
            };

            return await repository.FetchMultipleAsync<DeviceEvolutionEntity, DeviceEvolutionEntity>(criteria);
        }

        public async Task<IEnumerable<DeviceEvolutionEntity>> GetAttributesFPDeviceEvolutionAsync(string fpFingerprintIDs, bool isReadyDbExtra)
        {
            var criteria = new GetAttributesFPDeviceEvolutionCriteria
            {
                FPFingerprintIDs = fpFingerprintIDs,
                IsReadyDbExtra = isReadyDbExtra
            };

            return await repository.FetchAsync<DeviceEvolutionEntity>(criteria);
        }

        public async Task TransformDeviceInsertAsync(string transIDs, bool isReadyDbExtra)
        {
            var criteria = new TransformDeviceInsertCriteria
            {
                TransIDs = transIDs,
                IsReadyDbExtra = isReadyDbExtra
            };

            await repository.ExecuteAsync(criteria);
        }

        public async Task InsertFPDeviceMappingAsync(int batchSize, bool isTransformExtra)
        {
            var criteria = new InsertFPDeviceMappingCriteria
            {
                BatchSize = batchSize,
                IsReadyDbExtra = isTransformExtra
            };

            await repository.ExecuteAsync(criteria);
        }

        public async Task InsertFPDeviceEvolutionAsync(string deviceEvolutionJson, bool isTransformExtra)
        {
            var criteria = new InsertFPDeviceEvolutionCriteria
            {
                DeviceEvolutionJson = deviceEvolutionJson,
                IsReadyDbExtra = isTransformExtra
            };

            await repository.ExecuteAsync(criteria);
        }
    }

}