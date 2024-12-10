using Fanex.NAP.SPU.DCS.Features.MBDevice.Entities;
using System.Collections.Generic;

namespace Fanex.NAP.SPU.DCS.Features.MBDevice
{
    public interface IMBDeviceServices
    {

        IEnumerable<MBTransactionEntity> GetMBTransaction(int limitTotalRecord, int numberOfBatch);

        void TransformDeviceInsert(string transactionInfoJson);
    }

    public class MBDeviceServices : IMBDeviceServices
    {
        private readonly IMBDeviceDataAccess deviceDataAccess;

        public MBDeviceServices(IMBDeviceDataAccess deviceDataAccess)
        {
            this.deviceDataAccess = deviceDataAccess;
        }

        public IEnumerable<MBTransactionEntity> GetMBTransaction(int limitTotalRecord, int numberOfBatch)
        {
            return deviceDataAccess.GetMBTransaction(limitTotalRecord, numberOfBatch);
        }

        public void TransformDeviceInsert(string transactionInfoJson)
        {
            deviceDataAccess.InsertDevice(transactionInfoJson);
        }
    }
}
