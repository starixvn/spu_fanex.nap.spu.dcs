using Fanex.Data;
using Fanex.Data.Repository;
using Fanex.NAP.SPU.DCS.Features.Devices.Criteria;
using Fanex.NAP.SPU.DCS.Features.MBDevice.Criteria;
using Fanex.NAP.SPU.DCS.Features.MBDevice.Entities;
using System.Collections.Generic;

namespace Fanex.NAP.SPU.DCS.Features.MBDevice
{
    public interface IMBDeviceDataAccess
    {
        IEnumerable<MBTransactionEntity> GetMBTransaction(int limitTotalRecord, int numberOfBatch);

        void InsertDevice(string transactionInfoJson);

        int UpdateMaxMBTransIDTransformDevice();
    }

    public class MBDeviceDataAccess : IMBDeviceDataAccess
    {
        private readonly IDynamicRepository repository;

        public MBDeviceDataAccess(IDynamicRepository repository)
        {
            this.repository = repository ?? new DynamicRepository();
        }

        public IEnumerable<MBTransactionEntity> GetMBTransaction(int limitTotalRecord, int numberOfBatch)
        {
            var criteria = new GetMBTransactionCriteria
            {
                LimitTotalRecord = limitTotalRecord,
                NumberOfBatch = numberOfBatch
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<MBTransactionEntity>(criteria);
            }
        }

        public void InsertDevice(string transactionInfoJson)
        {
            var criteria = new InsertMBDeviceCriteria
            {
                MBTransactionInfoJson = transactionInfoJson
            };

            repository.Execute(criteria);
        }

        public int UpdateMaxMBTransIDTransformDevice()
        {
            var criteria = new UpdateMaxMBTransIDTransformDeviceCriteria { };

            return repository.Execute(criteria);
        }
    }
}
