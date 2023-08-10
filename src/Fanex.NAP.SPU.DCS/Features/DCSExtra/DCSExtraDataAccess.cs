using Fanex.Data;
using Fanex.Data.Repository;
using Fanex.NAP.SPU.DCS.Features.DCSExtra.Criteria;
using Fanex.NAP.SPU.DCS.Features.DCSExtra.Model;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace Fanex.NAP.SPU.DCS.Features.DCSExtra
{
    public interface IDCSExtraDataAccess
    {
        IEnumerable<string> GetParameterValue(int parameterID);

        Task<IEnumerable<DeviceInfoModel>> GetDeviceInfoAsync(string deviceIDList);

        Task<IEnumerable<DeviceFingerprintModel>> GetDeviceFingerprintAsync(string deviceNoList);

        Task<IEnumerable<RawTransactionModel>> GetRawTransactionAsync(string subscriberName, string settingKey, DateTime lastTransTime, DateTime toTransTime);

        Task InsertTransformRawTransAsync(string rawTransJson);

        Task UpdateParameterValueAsync(int parameterID, string parameterValue);

        Task<IEnumerable<TransactionInitialDataModel>> GetTransactionInitialDataAsync(int batchSize, int lastTransID_ParameterID, int lastCreatedDate_ParameterID);

        Task UpdateTransactionInitialDataAsync(string transJs);

        Task<IEnumerable<LocationByIPModel>> GetLocationByIPList(string ipIDs);

        Task InsertListCityAsync(string inputJs);

        Task InsertListCountryAsync(string inputJs);

        Task InsertListISPAsync(string inputJs);

        Task InsertListRegionAsync(string inputJs);
    }

    public class DCSExtraDataAccess : IDCSExtraDataAccess
    {
        private readonly IDynamicRepository repository;

        public DCSExtraDataAccess()
        {
            repository = new DynamicRepository();
        }

        public Task<IEnumerable<DeviceInfoModel>> GetDeviceInfoAsync(string deviceIDList)
        {
            var criteria = new DeviceInfoGetCriteria
            {
                ip_DeviceIDList = deviceIDList
            };

            using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
            return db.QueryAsync<DeviceInfoModel>(criteria);
        }

        public Task<IEnumerable<DeviceFingerprintModel>> GetDeviceFingerprintAsync(string deviceNoList)
        {
            var criteria = new LastDeviceFingerprintGetCriteria
            {
                ip_DeviceNoList = deviceNoList
            };

            using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
            return db.QueryAsync<DeviceFingerprintModel>(criteria);
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

        public Task<IEnumerable<RawTransactionModel>> GetRawTransactionAsync(string subscriberName, string settingKey, DateTime lastTransTime, DateTime toTransTime)
        {
            var criteria = new RawTransactionGetCriteria
            {
                ip_SubscriberName = subscriberName,
                ip_LastTransTime = lastTransTime,
                ip_ToTransTime = toTransTime
            };

            using var db = ObjectDbFactory.CreateInstance(settingKey);
            return db.QueryAsync<RawTransactionModel>(criteria);
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

        public Task<IEnumerable<TransactionInitialDataModel>> GetTransactionInitialDataAsync(int batchSize, int lastTransID_ParameterID, int lastCreatedDate_ParameterID)
        {
            var criteria = new TransactionInitialDataGetCriteria
            {
                BatchSize = batchSize,
                LastTransID_ParameterID = lastTransID_ParameterID,
                LastCreatedDate_ParameterID = lastCreatedDate_ParameterID
            };

            using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
            return db.QueryAsync<TransactionInitialDataModel>(criteria);
        }

        public Task UpdateTransactionInitialDataAsync(string transJs)
        {
            var criteria = new TransactionInitialDataUpdate
            {
                TransJs = transJs
            };

            return repository.ExecuteAsync(criteria);
        }

        public Task<IEnumerable<LocationByIPModel>> GetLocationByIPList(string ipIDs)
        {
            var criteria = new LocationByIPListGet
            {
                IPs = ipIDs
            };

            using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
            return db.QueryAsync<LocationByIPModel>(criteria);
        }

        public Task InsertListCityAsync(string inputJs)
        {
            var criteria = new CityInsertCriteria
            {
                InputJs = inputJs
            };

            return repository.ExecuteAsync(criteria);
        }

        public Task InsertListCountryAsync(string inputJs)
        {
            var criteria = new CountryInsertCriteria
            {
                InputJs = inputJs
            };

            return repository.ExecuteAsync(criteria);
        }

        public Task InsertListISPAsync(string inputJs)
        {
            var criteria = new ISPInsertCriteria
            {
                InputJs = inputJs
            };

            return repository.ExecuteAsync(criteria);
        }

        public Task InsertListRegionAsync(string inputJs)
        {
            var criteria = new RegionInsertCriteria
            {
                InputJs = inputJs
            };

            return repository.ExecuteAsync(criteria);
        }
    }
}
