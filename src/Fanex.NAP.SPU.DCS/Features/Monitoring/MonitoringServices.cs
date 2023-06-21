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

        IEnumerable<CustModel> GetMainCustByDate(DateTime rptDate, int minCustID, int batchSize);

        IEnumerable<ulong> GetDCSCustByDate(DateTime transDate);

        int InsertMonitorTransaction(DateTime rptDate, string transJson);

        uint? InsertNotifyMonitorTransaction(DateTime rptDate, string notifyType, string groupId, int batchSize);

        IEnumerable<NotifyMessageModel> GetNotifyMessage(uint? notifyMessageID, string notifyType, int batchSize);

        int UpdateNotifyMessage(string ids);
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

        public IEnumerable<CustModel> GetMainCustByDate(DateTime rptDate, int minCustID, int batchSize)
        {
            return monitoringDataAccess.GetMainCustByDate(rptDate, minCustID, batchSize);
        }

        public IEnumerable<ulong> GetDCSCustByDate(DateTime transDate)
        {
            return monitoringDataAccess.GetDCSCustByDate(transDate);
        }

        public int InsertMonitorTransaction(DateTime rptDate, string transJson)
        {
            return monitoringDataAccess.InsertMonitorTransaction(rptDate, transJson);
        }

        public uint? InsertNotifyMonitorTransaction(DateTime rptDate, string notifyType, string groupId, int batchSize)
        {
            return monitoringDataAccess.InsertNotifyMonitorTransaction(rptDate, notifyType, groupId, batchSize);
        }

        public IEnumerable<NotifyMessageModel> GetNotifyMessage(uint? notifyMessageID, string notifyType, int batchSize)
        {
            return monitoringDataAccess.GetNotifyMessage(notifyMessageID, notifyType, batchSize);
        }

        public int UpdateNotifyMessage(string ids)
        {
            return monitoringDataAccess.UpdateNotifyMessage(ids);
        }
    }
}
