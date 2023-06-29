namespace Fanex.NAP.SPU.DCS.Features.Devices
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.DCS.Features.Devices.Criteria;
    using Fanex.NAP.SPU.DCS.Features.Devices.Entities;
    using Fanex.NAP.SPU.DCS.Shares;
    using System.Collections.Generic;

    public interface IDeviceDataAccess
    {
        IEnumerable<StagingTransIdEntity> GetStagingTransIdForDevice(int size, int noOfBatch, bool isReadyDbExtra);

        DataResult UpdateDevice(string transIdJson, bool isReadyDbExtra);
    }

    public class DeviceDataAccess : IDeviceDataAccess
    {
        public IEnumerable<StagingTransIdEntity> GetStagingTransIdForDevice(int size, int noOfBatch, bool isReadyDbExtra)
        {
            var criteria = new GetDeviceStagingTransactionPackageCriteria
            {
                Size = size,
                NoOfBatch = noOfBatch,
                IsReadyDbExtra = isReadyDbExtra
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<StagingTransIdEntity>(criteria);
            }
        }

        public DataResult UpdateDevice(string transIdJson, bool isReadyDbExtra)
        {
            var repository = new DynamicRepository();

            var criteria = new UpdateDeviceIdCriteria
            {
                TransIdJson = transIdJson,
                IsReadyDbExtra = isReadyDbExtra
            };

            var errCode = repository.Execute(criteria);

            return new DataResult
            {
                ErrorCode = errCode
            };
        }
    }
}
