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

        public int SubscriberID { get; set; }

        public string SubscriberName { get; set; }

        public string URL { get; set; }

        public ulong DeviceID { get; set; }

        public ulong DeviceCodeID { get; set; }

        public string FirstDeviceCode { get; set; }

        public int DeviceStatus { get; set; }

        public string FingerprintCode { get; set; }

        public string UserAgent { get; set; }

        public string DeviceType { get; set; }

        public string IP { get; set; }

        public string IPID { get; set; }

        public string Action { get; set; }

        public string Flagged { get; set; }

        public int FlaggedID { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime InsertTime { get; set; }

        public ulong RawTransID { get; set; }

        public string BotDetection { get; set; }

        public string FakeIP { get; set; }

        public bool IsIncognitoMode { get; set; }

        public ulong FPPatternID01 { get; set; }

        public ulong FPPatternID02 { get; set; }

        public int TransFlow { get; set; }

        public int ActivatorVersionID { get; set; }

        public string ActivatorVersion { get; set; }

        public int FingerprintVersionID { get; set; }

        public string FingerprintVersion { get; set;}

        public int TaggingType { get; set; }

        public ulong FPGroupHardwareID { get; set; }

        public ulong FPGroupGraphicID { get; set; }

        public ulong FPGroupAudioID { get; set; }

        public ulong FPGroupBrowserID { get; set; }

        public ulong FPGroupPreferencesID { get; set; }

        public ulong WebRTCIPID { get; set; }

        public string WebRTCIPDetails { get; set; }
    }
}
