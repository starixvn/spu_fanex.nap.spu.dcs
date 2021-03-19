using Fanex.Data;
using Fanex.Data.Repository;
using Fanex.NAP.SPU.DCS.Features.Devices.Criteria;
using Fanex.NAP.SPU.DCS.Features.Devices.Entities;
using Fanex.NAP.SPU.DCS.Shares;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fanex.NAP.SPU.DCS.Features.Devices
{
    public interface IDeviceDataAccess
    {
        IEnumerable<StagingTransIdEntity> GetStagingTransIdForDevice(int size, int noOfBatch);

        DataResult UpdateDevice(string transIdJson);
    }

    public class DeviceDataAccess : IDeviceDataAccess
    {
        public IEnumerable<StagingTransIdEntity> GetStagingTransIdForDevice(int size, int noOfBatch)
        {
            var criteria = new GetDeviceStagingTransactionPackageCriteria
            {
                Size = size,
                NoOfBatch = noOfBatch
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<StagingTransIdEntity>(criteria);
            }
        }

        public DataResult UpdateDevice(string transIdJson)
        {
            var repository = new DynamicRepository();

            var criteria = new UpdateDeviceIdCriteria
            {
                TransIdJson = transIdJson
            };

            var errCode = repository.Execute(criteria);

            return new DataResult
            {
                ErrorCode = errCode
            };
        }
    }
}
