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

        int UpdateSumRawTransaction(SumTransactionModel sumTrans, string sumSubsJson, string sumDeviceTypeJson, string accountIPJson, string accountDeviceJson);

        NotifyTransactionModel GetNotificationTransaction();

        IEnumerable<CTSCustModel> GetCTSCustByDate(DateTime rptDate, int minCustID, int batchSize);

        IEnumerable<ulong> GetDCSCustByDate(DateTime transDate);

        int InsertMonitorTransaction(DateTime rptDate, string transJson);

        uint? InsertNotifyMonitorTransaction(DateTime rptDate, string notifyType, int batchSize);

        IEnumerable<NotifyMessageModel> GetNotifyMessage(uint? notifyMessageID, string notifyType, int batchSize);

        int UpdateNotifyMessage(string ids);
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
                    sumTransaction.SumSubscriberList = result.Read<SumSubscriberEntity>();
                    sumTransaction.SumDeviceTypeList = result.Read<SumDeviceTypeEntity>();
                    sumTransaction.AccountIPList = result.Read<AccountIPEntity>();
                    sumTransaction.AccountDeviceList = result.Read<AccountDeviceEntity>();

                    sumTransaction.MaxTransID = criteria.MaxTransID;
                    sumTransaction.DateScan = criteria.DateScan;
                    sumTransaction.AccountID = criteria.AccountID;

                    return sumTransaction;
                }
            }
        }

        public int UpdateSumRawTransaction(SumTransactionModel sumTrans, string sumSubsJson, string sumDeviceTypeJson, string accountIPJson, string accountDeviceJson)
        {
            var criteria = new UpdateSumTransactionCriteria
            {
                MaxTransID = sumTrans.MaxTransID,
                DateScan = sumTrans.DateScan,
                AccountID = sumTrans.AccountID,
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

        public IEnumerable<CTSCustModel> GetCTSCustByDate(DateTime rptDate, int minCustID, int batchSize)
        {
            var criteria = new GetCTSCustByDateCriteria
            {
                RptDate = rptDate,
                MinCustID = minCustID,
                BatchSize = batchSize
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<CTSCustModel>(criteria);
            }
        }

        public IEnumerable<ulong> GetDCSCustByDate(DateTime transDate)
        {
            var criteria = new GetDCSCustByDateCriteria
            {
                TransDate = transDate
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<ulong>(criteria);
            }
        }

        public int InsertMonitorTransaction(DateTime rptDate, string transJson)
        {
            var criteria = new InsertMonitorTransactionCriteria
            {
                RptDate = rptDate,
                TransJson = transJson
            };

            return repository.Execute(criteria);
        }

        public uint? InsertNotifyMonitorTransaction(DateTime rptDate, string notifyType, int batchSize)
        {
            var criteria = new InsertNotifyMonitorTransactionCriteria
            {
                RptDate = rptDate,
                NotifyType = notifyType,
                BatchSize = batchSize
            };

            repository.Execute(criteria);
            return criteria.NotifyMessageID;
        }

        public IEnumerable<NotifyMessageModel> GetNotifyMessage(uint? notifyMessageID, string notifyType, int batchSize)
        {
            var criteria = new GetNotifyMessageCriteria
            {
                NotifyType = notifyType,
                BatchSize = batchSize
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<NotifyMessageModel>(criteria);
            }
        }

        public int UpdateNotifyMessage(string ids)
        {
            var criteria = new UpdateNotifyMessageCriteria
            {
                IDs = ids
            };

            return repository.Execute(criteria);
        }
    }
}
