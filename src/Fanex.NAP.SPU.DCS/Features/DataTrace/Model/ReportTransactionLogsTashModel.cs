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
        public ulong AccountID { get; set; }
        public string LoginName { get; set; }
        public int SubscriberID { get; set; }
        public string SubscriberName { get; set; }
        public int URLID { get; set; }
        public string URLDetails { get; set; }
        public int DeviceCodeID { get; set; }
        public int DeviceID { get; set; }
        public string DeviceCode { get; set; }
        public string FirstDeviceCode { get; set; }
        public int DeviceStatus { get; set; }
        public int DeviceFingerprintID { get; set; }
        public string FingerprintCode { get; set; }
        public string UserAgentKey { get; set; }
        public string IP { get; set; }
        public int IPID { get; set; }
        public int ActionResultID { get; set; }
        public string Action { get; set; }
        public string ActionResult { get; set; }
        public int Flagged { get; set; }
        public string RobotTracking { get; set; }
        public int PluginID { get; set; }
        public int TransStatus { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime InsertTime { get; set; }
        public ulong RawTransID { get; set; }
        public decimal BotDetectionValue { get; set; }
        public int BotComponentID { get; set; }
        public bool FakeIP { get; set; }
        public bool IsIncognitoMode { get; set; }
        public int JSChallengeInfoID { get; set; }
        public int FP_UserAgentID { get; set; }
        public int FP_DeviceMappingID { get; set; }
        public int FP_DeviceID { get; set; }
        public int FP_DeviceStatus { get; set; }
    }
}
