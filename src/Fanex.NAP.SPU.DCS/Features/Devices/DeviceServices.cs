namespace Fanex.NAP.SPU.DCS.Features.Devices
{
    using Fanex.NAP.SPU.DCS.Features.Devices.Entities;
    using Fanex.NAP.SPU.DCS.Shares;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Linq;

    public interface IDeviceServices
    {
        IEnumerable<StagingTransIdEntity> GetStagingTransIdForDevice(int size, int noOfBatch, bool isReadyDbExtra);

        DataResult UpdateDevices(IEnumerable<StagingTransIdEntity> stagingTransId, bool isReadyDbExtra);
    }

    public class DeviceServices : IDeviceServices
    {
        private readonly IDeviceDataAccess deviceDataAcess;

        public DeviceServices(IDeviceDataAccess deviceDataAcess)
        {
            this.deviceDataAcess = deviceDataAcess;
        }

        public IEnumerable<StagingTransIdEntity> GetStagingTransIdForDevice(int size, int noOfBatch, bool isReadyDbExtra)
        {
            if(size != 0 && noOfBatch != 0)
            {
                return this.deviceDataAcess.GetStagingTransIdForDevice(size, noOfBatch, isReadyDbExtra);
            }

            return new List<StagingTransIdEntity>();
        }

        public DataResult UpdateDevices(IEnumerable<StagingTransIdEntity> stagingTransId, bool isReadyDbExtra)
        {            
            if(stagingTransId != null && stagingTransId.Any())
            {
                var transIdJson = JsonConvert.SerializeObject(stagingTransId);

                return this.deviceDataAcess.UpdateDevice(transIdJson, isReadyDbExtra);
            }

            return new DataResult
            {
                ErrorCode = -1,
                ErrorMessage = "Parameter is empty."
            };            
        }
    }
}
