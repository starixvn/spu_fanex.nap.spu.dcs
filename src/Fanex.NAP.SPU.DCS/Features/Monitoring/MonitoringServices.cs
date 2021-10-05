namespace Fanex.NAP.SPU.DCS.Features.Monitoring
{
    using Fanex.NAP.SPU.DCS.Features.Monitoring.Entities;
    using Fanex.NAP.SPU.DCS.Features.Monitoring.Model;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public interface IMonitoringServices
    {
        SumTransactionModel GetSumTransaction();

        int UpdateSumRawTransaction(SumTransactionModel sumTransaction);

        NotifyTransactionModel GetNotificationTransaction();
    }

    public class MonitoringServices : IMonitoringServices
    {
        private readonly IMonitoringDataAccess monitoringDataAccess;

        public MonitoringServices(IMonitoringDataAccess monitoringDataAccess)
        {
            this.monitoringDataAccess = monitoringDataAccess;
        }
       
        public SumTransactionModel GetSumTransaction()
        {
            return this.monitoringDataAccess.GetSumTransaction();
        }

        public int UpdateSumRawTransaction(SumTransactionModel sumTransaction)
        {
            if (sumTransaction != null && sumTransaction.SumTransactionList.Any())
            {
                var sumTransactionGroup = sumTransaction.SumTransactionList
                     .GroupBy(x => new { x.TransDate, x.SubscriberID })
                     .Select(x => new { Group = x.Key, Items = x.ToList() })
                     .Select(x => new SumTransactionEntity
                     {
                         TransDate = x.Group.TransDate,
                         SubscriberID = x.Group.SubscriberID,
                         ValidBrowserless = x.Items.Sum(y => y.ValidBrowserless),
                         ValidDeviceStatusNew = x.Items.Sum(y => y.ValidDeviceStatusNew),
                         ValidDeviceStatusOld = x.Items.Sum(y => y.ValidDeviceStatusOld),
                         ValidDeviceStatusRecover = x.Items.Sum(y => y.ValidDeviceStatusRecover),
                         ValidNoDevice = x.Items.Sum(y => y.ValidNoDevice),
                         ValidTotal = x.Items.Sum(y => y.ValidTotal)
                     });

                var sumTransJson = JsonConvert.SerializeObject(sumTransactionGroup);

                return this.monitoringDataAccess.UpdateSumRawTransaction(sumTransaction.MaxTransID, sumTransJson);
            }

            return 1;
        }
       
        public NotifyTransactionModel GetNotificationTransaction()
        {
            return this.monitoringDataAccess.GetNotificationTransaction();
        }

    }
}
