using Fanex.NAP.SPU.DCS.Features.DCSExtra.Model;
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
    }

    public class DCSExtraServices : IDCSExtraServices
    {
        private readonly IDCSExtraDataAccess dataAccess;

        public DCSExtraServices(IDCSExtraDataAccess dataAccess)
        {
            this.dataAccess = dataAccess;
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
    }
}
