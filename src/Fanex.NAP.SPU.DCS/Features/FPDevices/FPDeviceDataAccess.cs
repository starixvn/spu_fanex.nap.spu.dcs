namespace Fanex.NAP.SPU.DCS.Features.FPDevices
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.DCS.Features.FPDevices.Criteria;
    using Fanex.NAP.SPU.DCS.Features.FPDevices.Entities;
    using Fanex.NAP.SPU.DCS.Shares;    
    using System.Collections.Generic;

    public interface IFPDeviceDataAccess
    {
        IEnumerable<FPStagingTransIdEntity> GetFPStagingTransIdForDevice(int size, int noOfBatch);

        DataResult UpdateDevice(string transIdJson);
    }

    public class FPDeviceDataAccess : IFPDeviceDataAccess
    {
        public IEnumerable<FPStagingTransIdEntity> GetFPStagingTransIdForDevice(int size, int noOfBatch)
        {
            var criteria = new GetFPDeviceStagingTransactionPackageCriteria
            {
                Size = size,
                NoOfBatch = noOfBatch
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<FPStagingTransIdEntity>(criteria);
            }
        }

        public DataResult UpdateDevice(string transIdJson)
        {
            var repository = new DynamicRepository();

            var criteria = new UpdateFPDeviceIdCriteria
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
