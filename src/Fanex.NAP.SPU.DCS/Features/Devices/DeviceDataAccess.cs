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
        Task<IEnumerable<StagingTransId>> GetStagingTransIdForDeviceAsync(int size, int noOfBatch);

        Task<(IEnumerable<DeviceEvolutionEntity>, IEnumerable<DeviceEvolutionEntity>)> GetFPDeviceEvolutionAsync(int batchSize);

        Task TransformDeviceInsertAsync(string transIDs);

        Task InsertFPDeviceMappingAsync(int batchSize);

        Task InsertFPDeviceEvolutionAsync(string deviceEvolutionJson);

        int UpdateMaxTransIDTransformDevice();
    }

    public class DeviceDataAccess : IDeviceDataAccess
    {
        private readonly IDynamicRepository repository;

        public DeviceDataAccess(IDynamicRepository repository)
        {
            this.repository = repository ?? new DynamicRepository();
        }

        public async Task<IEnumerable<StagingTransId>> GetStagingTransIdForDeviceAsync(int size, int noOfBatch)
        {
            var criteria = new GetDeviceStagingTransactionPackageCriteria
            {
                Size = size,
                NoOfBatch = noOfBatch
            };

            return await repository.FetchAsync<StagingTransId>(criteria);
        }

        public async Task<(IEnumerable<DeviceEvolutionEntity>, IEnumerable<DeviceEvolutionEntity>)> GetFPDeviceEvolutionAsync(int batchSize)
        {
            var criteria = new GetFPDeviceEvolutionCriteria
            {
                BatchSize = batchSize
            };

            return await repository.FetchMultipleAsync<DeviceEvolutionEntity, DeviceEvolutionEntity>(criteria);
        }

        public async Task TransformDeviceInsertAsync(string transIDs)
        {
            var criteria = new TransformDeviceInsertCriteria
            {
                TransIDs = transIDs
            };

            await repository.ExecuteAsync(criteria);
        }

        public async Task InsertFPDeviceMappingAsync(int batchSize)
        {
            var criteria = new InsertFPDeviceMappingCriteria
            {
                BatchSize = batchSize
            };

            await repository.ExecuteAsync(criteria);
        }

        public async Task InsertFPDeviceEvolutionAsync(string deviceEvolutionJson)
        {
            var criteria = new InsertFPDeviceEvolutionCriteria
            {
                DeviceEvolutionJson = deviceEvolutionJson
            };

            await repository.ExecuteAsync(criteria);
        }

        public int UpdateMaxTransIDTransformDevice()
        {
            var criteria = new UpdateMaxTransIDTransformDeviceCriteria { };

            return repository.Execute(criteria);
        }
    }

}