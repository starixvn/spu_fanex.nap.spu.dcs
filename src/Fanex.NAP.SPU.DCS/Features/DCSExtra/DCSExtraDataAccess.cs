using Fanex.Data;
using Fanex.Data.Repository;
using Fanex.NAP.SPU.DCS.Features.DCSExtra.Criteria;
using Fanex.NAP.SPU.DCS.Features.DCSExtra.Model;
using System;
using System.Collections.Generic;

namespace Fanex.NAP.SPU.DCS.Features.DCSExtra
{
    public interface IDCSExtraDataAccess
    {
        IEnumerable<string> GetParameterValue(int parameterID);

        IEnumerable<DeviceInfoModel> GetDeviceInfo(string deviceIDList);

        IEnumerable<DeviceFingerprintModel> GetDeviceFingerprint(string deviceNoList);

        IEnumerable<RawTransactionModel> GetRawTransactionCTMax(string subscriberName, DateTime lastTransTime, DateTime toTransTime);

        IEnumerable<RawTransactionModel> GetRawTransactionVelki(string subscriberName, DateTime lastTransTime, DateTime toTransTime);
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

        public IEnumerable<RawTransactionModel> GetRawTransactionCTMax(string subscriberName, DateTime lastTransTime, DateTime toTransTime)
        {
            var criteria = new RawTransactionCTMaxGetCriteria
            {
                ip_SubscriberName = subscriberName,
                ip_LastTransTime = lastTransTime,
                ip_ToTransTime = toTransTime
            };

            using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
            return db.Query<RawTransactionModel>(criteria);
        }

        public IEnumerable<RawTransactionModel> GetRawTransactionVelki(string subscriberName, DateTime lastTransTime, DateTime toTransTime)
        {
            var criteria = new RawTransactionVelkiGetCriteria
            {
                ip_SubscriberName = subscriberName,
                ip_LastTransTime = lastTransTime,
                ip_ToTransTime = toTransTime
            };

            using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
            return db.Query<RawTransactionModel>(criteria);
        }
    }
}
