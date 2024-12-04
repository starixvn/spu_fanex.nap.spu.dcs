using Fanex.Data;
using Fanex.Data.Repository;
using Fanex.NAP.SPU.DCS.Features.MBDevice.Criteria;
using Fanex.NAP.SPU.DCS.Features.MBDevices.Entities;
using System.Collections.Generic;

namespace Fanex.NAP.SPU.DCS.Features.MBDevice
{
    public interface IMBDeviceDataAccess
    {
        IEnumerable<MBTransactionEntity> GetTransaction(int numberOfTrans, int numberOfBatch);

        void InsertDevice(string transIDs);
    }

    public class MBDeviceDataAccess : IMBDeviceDataAccess
    {
        private readonly IDynamicRepository repository;

        public MBDeviceDataAccess()
        {
            repository = new DynamicRepository();
        }

        public IEnumerable<MBTransactionEntity> GetTransaction(int numberOfTrans, int numberOfBatch)
        {
            var criteria = new GetMBTransactionCriteria
            {
                NumberOfTickets = numberOfTrans,
                NumberOfBatch = numberOfBatch
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<MBTransactionEntity>(criteria);
            }
        }

        public void InsertDevice(string transIDs)
        {
            var criteria = new InsertMBDeviceCriteria
            {
                TransIDs = transIDs,
            };

            repository.Execute(criteria);
        }
    }
}
