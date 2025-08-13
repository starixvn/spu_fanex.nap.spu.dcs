using System;

namespace Fanex.NAP.SPU.DCS.Features.DataTrace.Model
{
    public class MBTransactionLogstashModel
    {
        public ulong TransID { get; set; }

        public DateTime TransTime { get; set; }

        public int SubscriberID { get; set; }

        public string SubscriberName { get; set; }

        public ulong MBAccountID { get; set; }

        public int IsTransformed { get; set; }

        public string LoginName { get; set; }

        public ulong MBDeviceMappingID { get; set; }

        public ulong MBDeviceCodeTaggingID { get; set; }

        public ulong MBDeviceCodeMachineAccountID { get; set; }

        public ulong MBDeviceCodeMachineMediaID { get; set; }

        public int RecoverType { get; set; }

        public ulong MBDeviceID { get; set; }

        public int DeviceStatus { get; set; }

        public string IP { get; set; }

        public string IPID { get; set; }

        public int Flagged { get; set; }

        public int IsEmulator { get; set; }

        public ulong MBDeviceModelID { get; set; }

        public string ModelName { get; set; }

        public string Manufacturer { get; set; }

        public string Brand { get; set; }

        public ulong MBOSID { get; set; }

        public string OSName { get; set; }

        public string SDKVersion { get; set; }

        public ulong MBUserSettingID { get; set; }

        public string Country { get; set; }

        public string Timezone { get; set; }

        public string Language { get; set; }

        public string DeviceDetails { get; set; }
    }
}
