using Fanex.Data;
using Fanex.Data.Repository;
using Fanex.NAP.SPU.DCS.Features.DCSExtra.Criteria;
using Fanex.NAP.SPU.DCS.Features.DCSExtra.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fanex.NAP.SPU.DCS.Features.DCSExtra
{
    public interface IDCSExtraDataAccess
    {
        IEnumerable<string> GetParameterValue(int parameterID);

        IEnumerable<DeviceInfoModel> GetDeviceInfo(string deviceIDList);

        IEnumerable<DeviceFingerprintModel> GetDeviceFingerprint(string deviceNoList);

        Task<IEnumerable<RawTransactionModel>> GetRawTransactionAsync(string subscriberName, string settingKey, DateTime lastTransTime, DateTime toTransTime);

        Task InsertTransformRawTransAsync(string rawTransJson);

        Task UpdateParameterValueAsync(int parameterID, string parameterValue);
    }

    public class DCSExtraDataAccess : IDCSExtraDataAccess
    {
        private readonly IDynamicRepository repository;

        public DCSExtraDataAccess()
        {
            repository = new DynamicRepository();
        }

        public IEnumerable<DeviceInfoModel> GetDeviceInfo(string deviceIDList)
        {
            var criteria = new DeviceInfoGetCriteria
            {
                ip_DeviceIDList = deviceIDList
            };

            using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
            return db.Query<DeviceInfoModel>(criteria);
        }

        public IEnumerable<DeviceFingerprintModel> GetDeviceFingerprint(string deviceNoList)
        {
            var criteria = new LastDeviceFingerprintGetCriteria
            {
                ip_DeviceNoList = deviceNoList
            };

            using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
            return db.Query<DeviceFingerprintModel>(criteria);
        }

        public IEnumerable<string> GetParameterValue(int parameterID)
        {
            var criteria = new ParameterValueGetCriteria
            {
                ParameterID = parameterID
            };

            using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
            return db.Query<string>(criteria);
        }

        public async Task<IEnumerable<RawTransactionModel>> GetRawTransactionAsync(string subscriberName, string settingKey, DateTime lastTransTime, DateTime toTransTime)
        {
            var criteria = new RawTransactionGetCriteria
            {
                ip_SubscriberName = subscriberName,
                ip_LastTransTime = lastTransTime,
                ip_ToTransTime = toTransTime
            };

            using var db = ObjectDbFactory.CreateInstance(settingKey);
            return await db.QueryAsync<RawTransactionModel>(criteria);
        }

        public async Task InsertTransformRawTransAsync(string rawTransJson)
        {
            var criteria = new InsertTransformRawTransCriteria
            {
                RawTransJson = rawTransJson
            };

            await repository.ExecuteAsync(criteria);
        }

        public async Task UpdateParameterValueAsync(int parameterID, string parameterValue)
        {
            var criteria = new ParameterValueUpdateCriteria
            {
                ParameterID = parameterID,
                ParameterValue = parameterValue
            };

            await repository.ExecuteAsync(criteria);
        }
    }
}
