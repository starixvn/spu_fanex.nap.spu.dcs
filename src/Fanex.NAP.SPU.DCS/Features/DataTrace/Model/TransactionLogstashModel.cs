using System;

namespace Fanex.NAP.SPU.DCS.Features.DataTrace.Model
{
    public class TransactionLogstashModel
    {
        public ulong TransID { get; set; }

        public DateTime TransTime { get; set; }

        public ulong AccountID { get; set; }

        public int IsTransformed { get; set; }

        public string LoginName { get; set; }

        public string SubscriberName { get; set; }

        public string URL { get; set; }

        public ulong DeviceID { get; set; }

        public ulong DeviceCodeID { get; set; }

        public string FirstDeviceCode { get; set; }

        public int DeviceStatus { get; set; }

        public string FingerprintCode { get; set; }

        public string UserAgent { get; set; }

        public string IP { get; set; }

        public string IPID { get; set; }

        public string Action { get; set; }

        public string Flagged { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime InsertTime { get; set; }

        public ulong RawTransID { get; set; }

        public string BotDetection { get; set; }

        public string FakeIP { get; set; }

        public bool IsIncognitoMode { get; set; }
    }
}
