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

        int UpdateSumRawTransaction(long maxTransID, string sumTransJson);

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

                    sumTransaction.SumTransactionList = result.Read<SumTransactionEntity>();

                    if (maxTransIDs != null && maxTransIDs.Any())
                    {
                        sumTransaction.MaxTransID = maxTransIDs.ToList()[0];
                    }

                    return sumTransaction;
                }
            }
        }

        public int UpdateSumRawTransaction(long maxTransID, string sumTransJson)
        {
            var criteria = new UpdateSumTransactionCriteria
            {
                MaxTransID = maxTransID,
                SumTransJson = sumTransJson
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
                    var yesterdayZeros = result.Read<string>();
                    var validLess50Percents = result.Read<string>();

                    if (yesterdayLess50Percents != null && yesterdayLess50Percents.Any())
                    {
                        notifyTransList.YesterdayLess50Percent = string.Join(',', yesterdayLess50Percents.Select(x=>x));
                    }

                    if (yesterdayZeros != null && yesterdayZeros.Any())
                    {
                        notifyTransList.YesterdayZero = string.Join(',', yesterdayZeros.Select(x => x));
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
