namespace Fanex.NAP.SPU.DCS.Features.Monitoring
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.NAP.SPU.DCS.Features.Monitoring.Model;
    using Newtonsoft.Json;

    public interface IMonitoringServices
    {
        SumTransactionModel GetSumTransaction();

        int UpdateSumRawTransaction(SumTransactionModel sumTransaction);

        NotifyTransactionModel GetNotificationTransaction();

        IEnumerable<CTSCustModel> GetCTSCustByDate(DateTime rptDate, int minCustID, int batchSize);

        IEnumerable<ulong> GetDCSCustByDate(DateTime transDate);

        void InsertMonitorTransaction(DateTime rptDate, string transJson);
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

                    return this.monitoringDataAccess.UpdateSumRawTransaction(sumTransaction, sumSubsJson, sumDeviceTypeJson, accountIPJson, accountDeviceJson);
                }
            }

            return 1;
        }

        public NotifyTransactionModel GetNotificationTransaction()
        {
            return this.monitoringDataAccess.GetNotificationTransaction();
        }

        public IEnumerable<CTSCustModel> GetCTSCustByDate(DateTime rptDate, int minCustID, int batchSize)
        {
            return monitoringDataAccess.GetCTSCustByDate(rptDate, minCustID, batchSize);
        }

        public IEnumerable<ulong> GetDCSCustByDate(DateTime transDate)
        {
            return monitoringDataAccess.GetDCSCustByDate(transDate);
        }

        public void InsertMonitorTransaction(DateTime rptDate, string transJson)
        {
            monitoringDataAccess.InsertMonitorTransaction(rptDate, transJson);
        }
    }
}
