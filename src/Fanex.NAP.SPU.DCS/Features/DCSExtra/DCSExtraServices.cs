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

        IEnumerable<DeviceInfoModel> GetDeviceInfo(string deviceIDList);

        IEnumerable<DeviceFingerprintModel> GetDeviceFingerprint(string deviceNoList);

        Task InsertTransformRawTransAsync(string rawTransJson);

        Task UpdateParameterValueAsync(int parameterID, string parameterValue);
    }

    public class DCSExtraServices : IDCSExtraServices
    {
        private readonly IDCSExtraDataAccess dataAccess;
        private readonly ISentryClient sentryClient;
        public delegate Task<IEnumerable<RawTransactionModel>> GetRawTransactionDelegate(string subscriberName, DateTime lastTransTime, DateTime toTransTime);

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

        public IEnumerable<DeviceInfoModel> GetDeviceInfo(string deviceIDList)
        {
            return dataAccess.GetDeviceInfo(deviceIDList);
        }

        public IEnumerable<DeviceFingerprintModel> GetDeviceFingerprint(string deviceNoList)
        {
            return dataAccess.GetDeviceFingerprint(deviceNoList);
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
