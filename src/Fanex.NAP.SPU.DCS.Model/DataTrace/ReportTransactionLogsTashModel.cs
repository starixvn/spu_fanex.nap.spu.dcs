using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.SPU.DCS.Features.DataTrace.Model
{
    public class ReportTransactionLogstashModel
    {
        public ulong TransID { get; set; }
        public DateTime TransTime { get; set; }
        public int URLID { get; set; }
        public int DeviceCode { get; set; }
        public string FirstDeviceCode { get; set; }
        public string SubscriberName { get; set; }
        public int DeviceStatus { get; set; }
        public int DeviceFingerprintCode { get; set; }
        public string UserAgentKey { get; set; }
        public string IP { get; set; }
        public int IPID { get; set; }
        public int ActionResultCode { get; set; }
        public int Flagged { get; set; }
        public int PluginCode { get; set; }
        public int TransStatus { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime InsertTime { get; set; }
        public ulong RawTransCode { get; set; }
        public decimal BotDetectionValue { get; set; }
        public int BotComponentCode { get; set; }
        public bool FakeIP { get; set; }
        public bool IsIncognitoMode { get; set; }
        public int JSChallengeInfoCode { get; set; }
        public int FP_UserAgentCode { get; set; }
        public int FP_DeviceMappingCode { get; set; }
        public int FP_DeviceCode { get; set; }
        public int FP_DeviceStatus { get; set; }
    }
}
