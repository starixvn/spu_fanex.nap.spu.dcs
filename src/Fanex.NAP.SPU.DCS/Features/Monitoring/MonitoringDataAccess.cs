namespace Fanex.NAP.SPU.DCS.Features.Monitoring
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.DCS.Features.Monitoring.Criteria;
    using Fanex.NAP.SPU.DCS.Features.Monitoring.Entities;
    using Fanex.NAP.SPU.DCS.Features.Monitoring.Model;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public interface IMonitoringDataAccess
    {
        SumTransactionModel GetSumTransaction();

        int UpdateSumRawTransaction(long maxTransID, string sumSubsJson, string sumDeviceTypeJson, string accountIPJson, string accountDeviceJson);

        NotifyTransactionModel GetNotificationTransaction();
    }
    public class MonitoringDataAccess : IMonitoringDataAccess
    {
        private readonly IDynamicRepository repository;

        public MonitoringDataAccess()
        {
            repository = new DynamicRepository();
        }

        public SumTransactionModel GetSumTransaction()
        {
            var sumTransaction = new SumTransactionModel();
            var criteria = new GetSumTransactionCriteria();

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (var result = db.QueryMultiple(criteria))
                {
                    var maxTransIDs = result.Read<long>();

                    sumTransaction.SumSubscriberList = result.Read<SumSubscriberEntity>();
                    sumTransaction.SumDeviceTypeList = result.Read<SumDeviceTypeEntity>();
                    sumTransaction.AccountIPList = result.Read<AccountIPEntity>();
                    sumTransaction.AccountDeviceList = result.Read<AccountDeviceEntity>();

                    if (maxTransIDs != null && maxTransIDs.Any())
                    {
                        sumTransaction.MaxTransID = maxTransIDs.First();
                    }

                    return sumTransaction;
                }
            }
        }

        public int UpdateSumRawTransaction(long maxTransID, string sumSubsJson, string sumDeviceTypeJson, string accountIPJson, string accountDeviceJson)
        {
            var criteria = new UpdateSumTransactionCriteria
            {
                MaxTransID = maxTransID,
                SumSubscriberJson = sumSubsJson,
                SumDeviceTypeJson = sumDeviceTypeJson,
                AccountIPJson = accountIPJson,
                AccountDeviceJson = accountDeviceJson
            };

           return repository.Execute(criteria);
        }

        public NotifyTransactionModel GetNotificationTransaction()
        {
            var notifyTransList = new NotifyTransactionModel();
            var criteria = new GetNotificationTransactionCriteria();
           
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (var result = db.QueryMultiple(criteria))
                {
                    var yesterdayLess50Percents = result.Read<string>();                    
                    var validLess50Percents = result.Read<string>();

                    if (yesterdayLess50Percents != null && yesterdayLess50Percents.Any())
                    {
                        notifyTransList.YesterdayLess50Percent = string.Join(',', yesterdayLess50Percents.Select(x=>x));
                    }                   

                    if (validLess50Percents != null && validLess50Percents.Any())
                    {
                        notifyTransList.ValidLess50Percent = string.Join(',', validLess50Percents.Select(x => x));
                    }                    

                    return notifyTransList;
                }
            }
        }
    }
}
