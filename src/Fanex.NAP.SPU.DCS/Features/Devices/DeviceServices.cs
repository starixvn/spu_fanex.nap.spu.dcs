using Fanex.NAP.SPU.DCS.Features.Devices.Entities;
using Fanex.NAP.SPU.DCS.Shares;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fanex.NAP.SPU.DCS.Features.Devices
{
    public interface IDeviceServices
    {
        IEnumerable<StagingTransIdEntity> GetStagingTransIdForDevice(int size, int noOfBatch);

        DataResult UpdateDevices(IEnumerable<StagingTransIdEntity> stagingTransId);
    }

    public class DeviceServices : IDeviceServices
    {
        private readonly IDeviceDataAccess deviceDataAcess;

        public DeviceServices(IDeviceDataAccess deviceDataAcess)
        {
            this.deviceDataAcess = deviceDataAcess;
        }

        public IEnumerable<StagingTransIdEntity> GetStagingTransIdForDevice(int size, int noOfBatch)
        {
            if(size != 0 && noOfBatch != 0)
            {
                return this.deviceDataAcess.GetStagingTransIdForDevice(size, noOfBatch);
            }

            return new List<StagingTransIdEntity>();
        }

        public DataResult UpdateDevices(IEnumerable<StagingTransIdEntity> stagingTransId)
        {

            if(stagingTransId != null && stagingTransId.Any())
            {
                var transIdJson = JsonConvert.SerializeObject(stagingTransId);

                return this.deviceDataAcess.UpdateDevice(transIdJson);
            }

            return new DataResult
            {
                ErrorCode = -1,
                ErrorMessage = "Parameter is empty."
            };            
        }
    }
}
