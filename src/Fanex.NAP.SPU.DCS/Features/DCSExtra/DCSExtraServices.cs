using Fanex.NAP.SPU.DCS.Features.DCSExtra.Model;
using Sentry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fanex.NAP.SPU.DCS.Features.DCSExtra
{
    public interface IDCSExtraServices
    {
        string GetParameterValue(int parameterID);

        Task<IEnumerable<RawTransactionModel>> GetRawTransactionAsync(string subscriberName, string settingKey, DateTime lastTransTime, DateTime toTransTime);

        Task<IEnumerable<DeviceInfoModel>> GetDeviceInfoAsync(string deviceIDList);

        Task<IEnumerable<DeviceFingerprintModel>> GetDeviceFingerprintAsync(string deviceNoList);

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

    public class DCSExtraServices : IDCSExtraServices
    {
        private readonly IDCSExtraDataAccess dataAccess;
        private readonly ISentryClient sentryClient;

        public DCSExtraServices(IDCSExtraDataAccess dataAccess, ISentryClient sentryClient)
        {
            this.dataAccess = dataAccess;
            this.sentryClient = sentryClient;
        }

        public string GetParameterValue(int parameterID)
        {
            var result = dataAccess.GetParameterValue(parameterID);
            if (result != null && result.Any())
            {
                return result.FirstOrDefault();
            }
            return string.Empty;
        }

        public Task<IEnumerable<DeviceInfoModel>> GetDeviceInfoAsync(string deviceIDList)
        {
            return dataAccess.GetDeviceInfoAsync(deviceIDList);
        }

        public Task<IEnumerable<DeviceFingerprintModel>> GetDeviceFingerprintAsync(string deviceNoList)
        {
            return dataAccess.GetDeviceFingerprintAsync(deviceNoList);
        }

        public Task<IEnumerable<RawTransactionModel>> GetRawTransactionAsync(string subscriberName, string settingKey, DateTime lastTransTime, DateTime toTransTime)
        {
            return dataAccess.GetRawTransactionAsync(subscriberName, settingKey, lastTransTime, toTransTime);
        }

        public async Task InsertTransformRawTransAsync(string rawTransJson)
        {
            await dataAccess.InsertTransformRawTransAsync(rawTransJson);
        }

        public async Task UpdateParameterValueAsync(int parameterID, string parameterValue)
        {
            await dataAccess.UpdateParameterValueAsync(parameterID, parameterValue);
        }

        public Task<IEnumerable<TransactionInitialDataModel>> GetTransactionInitialDataAsync(int batchSize, int lastTransID_ParameterID, int lastCreatedDate_ParameterID)
        {
            return dataAccess.GetTransactionInitialDataAsync(batchSize, lastTransID_ParameterID, lastCreatedDate_ParameterID);
        }

        public async Task UpdateTransactionInitialDataAsync(string transJs)
        {
            try
            {
                await dataAccess.UpdateTransactionInitialDataAsync(transJs);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Task<IEnumerable<LocationByIPModel>> GetLocationByIPList(string ipIDs)
        {
            return dataAccess.GetLocationByIPList(ipIDs);
        }

        public Task InsertListCityAsync(string inputJs)
        {
            return dataAccess.InsertListCityAsync(inputJs);
        }

        public Task InsertListCountryAsync(string inputJs)
        {
            return dataAccess.InsertListCountryAsync(inputJs);
        }

        public Task InsertListISPAsync(string inputJs)
        {
            return dataAccess.InsertListISPAsync(inputJs);
        }

        public Task InsertListRegionAsync(string inputJs)
        {
            return dataAccess.InsertListRegionAsync(inputJs);
        }
    }
}
