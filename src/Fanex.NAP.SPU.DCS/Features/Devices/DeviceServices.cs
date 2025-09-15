namespace Fanex.NAP.SPU.DCS.Features.Devices
{
    using Fanex.NAP.SPU.DCS.Features.Devices.Entities;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public interface IDeviceServices
    {

        Task<IEnumerable<StagingTransId>> GetStagingTransIdForDeviceAsync(int size, int noOfBatch);

        Task<(IEnumerable<DeviceEvolutionEntity>, IEnumerable<DeviceEvolutionEntity>)> GetFPDeviceEvolutionAsync(int batchSize);

        Task TransformDeviceInsertAsync(IEnumerable<StagingTransId> stagingTransId);

        Task InsertFPDeviceMappingAsync(int batchSize);

        Task InsertFPDeviceEvolutionAsync(string deviceEvolutionJson);

        int UpdateMaxTransIDTransformDevice();
    }

    public class DeviceServices : IDeviceServices
    {
        private readonly IDeviceDataAccess deviceDataAcess;

        public DeviceServices(IDeviceDataAccess deviceDataAcess)
        {
            this.deviceDataAcess = deviceDataAcess;
        }

        public async Task<IEnumerable<StagingTransId>> GetStagingTransIdForDeviceAsync(int size, int noOfBatch)
        {
            if (size != 0 && noOfBatch != 0)
            {
                return await deviceDataAcess.GetStagingTransIdForDeviceAsync(size, noOfBatch);
            }

            return new List<StagingTransId>();
        }

        public async Task<(IEnumerable<DeviceEvolutionEntity>, IEnumerable<DeviceEvolutionEntity>)> GetFPDeviceEvolutionAsync(int batchSize)
        {
            if (batchSize > 0)
            {
                return await deviceDataAcess.GetFPDeviceEvolutionAsync(batchSize);
            }

            return (new List<DeviceEvolutionEntity>(), new List<DeviceEvolutionEntity>());
        }

        public async Task TransformDeviceInsertAsync(IEnumerable<StagingTransId> stagingTransId)
        {
            string transIDs = string.Join(",", stagingTransId.Select(t => t.TransId.ToString()));

            await deviceDataAcess.TransformDeviceInsertAsync(transIDs);
        }

        public async Task InsertFPDeviceMappingAsync(int batchSize)
        {
            await deviceDataAcess.InsertFPDeviceMappingAsync(batchSize);
        }

        public async Task InsertFPDeviceEvolutionAsync(string deviceEvolutionJson)
        {
            await deviceDataAcess.InsertFPDeviceEvolutionAsync(deviceEvolutionJson);
        }

        public int UpdateMaxTransIDTransformDevice()
        {
            return deviceDataAcess.UpdateMaxTransIDTransformDevice();
        }
    }
}
