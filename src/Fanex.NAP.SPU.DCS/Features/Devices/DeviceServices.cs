namespace Fanex.NAP.SPU.DCS.Features.Devices
{
    using Fanex.NAP.SPU.DCS.Features.Devices.Entities;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public interface IDeviceServices
    {

        Task<IEnumerable<StagingTransId>> GetStagingTransIdForDeviceAsync(int size, int noOfBatch, bool isReadyDbExtra);

        Task<(IEnumerable<DeviceEvolutionEntity>, IEnumerable<DeviceEvolutionEntity>)> GetFPDeviceEvolutionAsync(int batchSize, bool isReadyDbExtra);

        Task TransformDeviceInsertAsync(IEnumerable<StagingTransId> stagingTransId, bool isReadyDbExtra);

        Task InsertFPDeviceMappingAsync(int batchSize, bool isTransformExtra);

        Task InsertFPDeviceEvolutionAsync(string deviceEvolutionJson, bool isTransformExtra);

        int UpdateMaxTransIDTransformDevice();
    }

    public class DeviceServices : IDeviceServices
    {
        private readonly IDeviceDataAccess deviceDataAcess;

        public DeviceServices(IDeviceDataAccess deviceDataAcess)
        {
            this.deviceDataAcess = deviceDataAcess;
        }

        public async Task<IEnumerable<StagingTransId>> GetStagingTransIdForDeviceAsync(int size, int noOfBatch, bool isReadyDbExtra)
        {
            if (size != 0 && noOfBatch != 0)
            {
                return await deviceDataAcess.GetStagingTransIdForDeviceAsync(size, noOfBatch, isReadyDbExtra);
            }

            return new List<StagingTransId>();
        }

        public async Task<(IEnumerable<DeviceEvolutionEntity>, IEnumerable<DeviceEvolutionEntity>)> GetFPDeviceEvolutionAsync(int batchSize, bool isReadyDbExtra)
        {
            if (batchSize > 0)
            {
                return await deviceDataAcess.GetFPDeviceEvolutionAsync(batchSize, isReadyDbExtra);
            }

            return (new List<DeviceEvolutionEntity>(), new List<DeviceEvolutionEntity>());
        }

        public async Task TransformDeviceInsertAsync(IEnumerable<StagingTransId> stagingTransId, bool isReadyDbExtra)
        {
            string transIDs = string.Join(",", stagingTransId.Select(t => t.TransId.ToString()));

            await deviceDataAcess.TransformDeviceInsertAsync(transIDs, isReadyDbExtra);
        }

        public async Task InsertFPDeviceMappingAsync(int batchSize, bool isTransformExtra)
        {
            await deviceDataAcess.InsertFPDeviceMappingAsync(batchSize, isTransformExtra);
        }

        public async Task InsertFPDeviceEvolutionAsync(string deviceEvolutionJson, bool isTransformExtra)
        {
            await deviceDataAcess.InsertFPDeviceEvolutionAsync(deviceEvolutionJson, isTransformExtra);
        }

        public int UpdateMaxTransIDTransformDevice()
        {
            return deviceDataAcess.UpdateMaxTransIDTransformDevice();
        }
    }
}
