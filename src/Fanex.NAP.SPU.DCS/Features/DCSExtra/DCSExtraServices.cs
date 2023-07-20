using Fanex.NAP.SPU.DCS.Features.DCSExtra.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fanex.NAP.SPU.DCS.Features.DCSExtra
{
    public interface IDCSExtraServices
    {
        string GetParameterValue(int parameterID);

        IEnumerable<RawTransactionModel> GetRawTransaction(string subscriberName, DateTime lastTransTime, DateTime toTransTime);

        IEnumerable<DeviceInfoModel> GetDeviceInfo(string deviceIDList);

        IEnumerable<DeviceFingerprintModel> GetDeviceFingerprint(string deviceNoList);
    }

    public class DCSExtraServices : IDCSExtraServices
    {
        private readonly IDCSExtraDataAccess dataAccess;
        public delegate IEnumerable<RawTransactionModel> GetRawTransactionDelegate(string subscriberName, DateTime lastTransTime, DateTime toTransTime);

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

        public IEnumerable<DeviceInfoModel> GetDeviceInfo(string deviceIDList)
        {
            return dataAccess.GetDeviceInfo(deviceIDList);
        }

        public IEnumerable<DeviceFingerprintModel> GetDeviceFingerprint(string deviceNoList)
        {
            return dataAccess.GetDeviceFingerprint(deviceNoList);
        }

        public IEnumerable<RawTransactionModel> GetRawTransaction(string subscriberName, DateTime lastTransTime, DateTime toTransTime)
        {
            string methodName = "GetRawTransaction" + subscriberName;
            GetRawTransactionDelegate methodDelegate = (GetRawTransactionDelegate)Delegate.CreateDelegate(typeof(GetRawTransactionDelegate), this, methodName);

            return methodDelegate(subscriberName, lastTransTime, toTransTime);
        }

        private IEnumerable<RawTransactionModel> GetRawTransactionCTMax(string subscriberName, DateTime lastTransTime, DateTime toTransTime)
        {
            return dataAccess.GetRawTransactionCTMax(subscriberName, lastTransTime, toTransTime);
        }

        private IEnumerable<RawTransactionModel> GetRawTransactionVelki(string subscriberName, DateTime lastTransTime, DateTime toTransTime)
        {
            return dataAccess.GetRawTransactionVelki(subscriberName, lastTransTime, toTransTime);
        }
    }
}
