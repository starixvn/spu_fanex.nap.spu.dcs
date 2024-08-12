namespace Fanex.NAP.SPU.DCS.Features.Devices
{
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.DCS.Features.Devices.Criteria;
    using Fanex.NAP.SPU.DCS.Features.Devices.Entities;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IDeviceDataAccess
    {
        Task<IEnumerable<StagingTransId>> GetStagingTransIdForDeviceAsync(int size, int noOfBatch, bool isReadyDbExtra);

        Task TransformDeviceInsertAsync(string transIDs, bool isReadyDbExtra);
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

        public async Task TransformDeviceInsertAsync(string transIDs, bool isReadyDbExtra)
        {
            var criteria = new TransformDeviceInsertCriteria
            {
                TransIDs = transIDs,
                IsReadyDbExtra = isReadyDbExtra
            };

            await repository.ExecuteAsync(criteria);
        }
    }

}