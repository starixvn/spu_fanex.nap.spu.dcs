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
    }

    public class DCSExtraServices : IDCSExtraServices
    {
        private readonly IDCSExtraDataAccess dataAccess;
        public delegate Task<IEnumerable<RawTransactionModel>> GetRawTransactionDelegate(string subscriberName, DateTime lastTransTime, DateTime toTransTime);

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
            try
            {
                return dataAccess.GetDeviceInfoAsync(deviceIDList);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Task<IEnumerable<DeviceFingerprintModel>> GetDeviceFingerprintAsync(string deviceNoList)
        {
            try
            {
                return dataAccess.GetDeviceFingerprintAsync(deviceNoList);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<RawTransactionModel>> GetRawTransactionAsync(string subscriberName, string settingKey, DateTime lastTransTime, DateTime toTransTime)
        {
            try
            {
                return await dataAccess.GetRawTransactionAsync(subscriberName, settingKey, lastTransTime, toTransTime);
            }
            catch (Exception)
            {
                throw;
            }
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
