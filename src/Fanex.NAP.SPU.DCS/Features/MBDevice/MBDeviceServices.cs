using Fanex.NAP.SPU.DCS.Features.MBDevices.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Fanex.NAP.SPU.DCS.Features.MBDevice
{
    public interface IMBDeviceServices
    {

        IEnumerable<MBTransactionEntity> GetTransaction(int numberOfTrans, int numberOfBatch);

        IMBDeviceServices TransformDeviceInsert(IEnumerable<MBTransactionEntity> transId);
    }

    public class MBDeviceServices : IMBDeviceServices
    {
        private readonly IMBDeviceDataAccess deviceDataAccess;

        public MBDeviceServices(IMBDeviceDataAccess deviceDataAccess)
        {
            this.deviceDataAccess = deviceDataAccess;
        }

        public IEnumerable<MBTransactionEntity> GetTransaction(int numberOfTrans, int numberOfBatch)
        {
            return this.deviceDataAccess.GetTransaction(numberOfTrans, numberOfBatch);
        }

        public IMBDeviceServices TransformDeviceInsert(IEnumerable<MBTransactionEntity> transId)
        {
            string transIDs = string.Join(",", transId.Select(t => t.TransId.ToString()));

            this.deviceDataAccess.InsertDevice(transIDs);
            return this;
        }
    }
}
