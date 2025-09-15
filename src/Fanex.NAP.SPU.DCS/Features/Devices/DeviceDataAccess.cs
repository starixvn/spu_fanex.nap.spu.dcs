namespace Fanex.NAP.SPU.DCS.Features.Devices
{
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.DCS.Features.Devices.Criteria;
    using Fanex.NAP.SPU.DCS.Features.Devices.Entities;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IDeviceDataAccess
    {
        Task<IEnumerable<StagingTransId>> GetStagingTransIdForDeviceAsync(int size, int noOfBatch);

        Task TransformDeviceInsertAsync(string transIDs);

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

        public async Task TransformDeviceInsertAsync(string transIDs)
        {
            var criteria = new TransformDeviceInsertCriteria
            {
                TransIDs = transIDs
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