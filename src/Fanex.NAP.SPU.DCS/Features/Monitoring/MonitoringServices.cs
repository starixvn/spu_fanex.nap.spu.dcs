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
            if (sumTransaction.MaxTransID != 0 && sumTransaction != null && sumTransaction.SumTransactionList.Any())
            {               
                var sumTransJson = JsonConvert.SerializeObject(sumTransaction.SumTransactionList);

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
