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
            if (sumTransaction != null && sumTransaction.MaxTransID != 0)
            {
                if (sumTransaction.SumSubscriberList.Any()
                    || sumTransaction.SumDeviceTypeList.Any()
                    || sumTransaction.AccountIPList.Any()
                    || sumTransaction.AccountDeviceList.Any())
                {
                    var sumSubsJson = JsonConvert.SerializeObject(sumTransaction.SumSubscriberList);
                    var sumDeviceTypeJson = JsonConvert.SerializeObject(sumTransaction.SumDeviceTypeList);
                    var accountIPJson = JsonConvert.SerializeObject(sumTransaction.AccountIPList);
                    var accountDeviceJson = JsonConvert.SerializeObject(sumTransaction.AccountDeviceList);

                    return this.monitoringDataAccess.UpdateSumRawTransaction(sumTransaction.MaxTransID, sumSubsJson, sumDeviceTypeJson, accountIPJson, accountDeviceJson);
                }
            }

            return 1;
        }
       
        public NotifyTransactionModel GetNotificationTransaction()
        {
            return this.monitoringDataAccess.GetNotificationTransaction();
        }

    }
}
