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

        int InsertMonitorTransaction(DateTime rptDate, string transJson);

        uint InsertNotifyMonitorTransaction(DateTime rptDate, string notifyType, string groupId, int limitShowCust);

        NotifyMessageModel GetNotifyMessageById(uint notifyMessageID);

        IEnumerable<NotifyMessageModel> GetNotifyMessage(string notifyType, int batchSize);

        int UpdateNotifyMessagesToNotified(string ids);
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
            if (sumTransaction != null && sumTransaction.MaxTransID != 0 && CheckValidTrans(sumTransaction))
            {
                var sumSubsJson = JsonConvert.SerializeObject(sumTransaction.SumSubscriberList);
                var sumDeviceTypeJson = JsonConvert.SerializeObject(sumTransaction.SumDeviceTypeList);
                var accountIPJson = JsonConvert.SerializeObject(sumTransaction.AccountIPList);
                var accountDeviceJson = JsonConvert.SerializeObject(sumTransaction.AccountDeviceList);

                return this.monitoringDataAccess.UpdateSumRawTransaction(sumTransaction, sumSubsJson, sumDeviceTypeJson, accountIPJson, accountDeviceJson);
            }

            return 1;
        }

        private static bool CheckValidTrans(SumTransactionModel sumTransaction)
        {
            return sumTransaction.SumSubscriberList.Any()
                                || sumTransaction.SumDeviceTypeList.Any()
                                || sumTransaction.AccountIPList.Any()
                                || sumTransaction.AccountDeviceList.Any();
        }

        public NotifyTransactionModel GetNotificationTransaction()
        {
            return this.monitoringDataAccess.GetNotificationTransaction();
        }

        public IEnumerable<CustModel> GetMainCustByDate(DateTime rptDate, int minCustID, int batchSize)
        {
            return monitoringDataAccess.GetMainCustByDate(rptDate, minCustID, batchSize);
        }

        public int InsertMonitorTransaction(DateTime rptDate, string transJson)
        {
            return monitoringDataAccess.InsertMonitorTransaction(rptDate, transJson);
        }

        public uint InsertNotifyMonitorTransaction(DateTime rptDate, string notifyType, string groupId, int limitShowCust)
        {
            return monitoringDataAccess.InsertNotifyMonitorTransaction(rptDate, notifyType, groupId, limitShowCust);
        }

        public NotifyMessageModel GetNotifyMessageById(uint notifyMessageID)
        {
            return monitoringDataAccess.GetNotifyMessageById(notifyMessageID);
        }

        public IEnumerable<NotifyMessageModel> GetNotifyMessage(string notifyType, int batchSize)
        {
            return monitoringDataAccess.GetNotifyMessage(notifyType, batchSize);
        }

        public int UpdateNotifyMessagesToNotified(string ids)
        {
            return monitoringDataAccess.UpdateNotifyMessagesToNotified(ids);
        }
    }
}
