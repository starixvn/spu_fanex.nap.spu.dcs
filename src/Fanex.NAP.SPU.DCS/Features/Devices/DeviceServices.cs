namespace Fanex.NAP.SPU.DCS.Features.Devices
{
    using Fanex.NAP.SPU.DCS.Features.Devices.Entities;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public interface IDeviceServices
    {

        Task<IEnumerable<StagingTransId>> GetStagingTransIdForDeviceAsync(int size, int noOfBatch);

        Task TransformDeviceInsertAsync(IEnumerable<StagingTransId> stagingTransId);

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

        public async Task TransformDeviceInsertAsync(IEnumerable<StagingTransId> stagingTransId)
        {
            string transIDs = string.Join(",", stagingTransId.Select(t => t.TransId.ToString()));

            await deviceDataAcess.TransformDeviceInsertAsync(transIDs);
        }

        public int UpdateMaxTransIDTransformDevice()
        {
            return deviceDataAcess.UpdateMaxTransIDTransformDevice();
        }
    }
}
