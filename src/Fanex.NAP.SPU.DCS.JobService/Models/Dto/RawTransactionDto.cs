using System;

namespace Fanex.NAP.SPU.DCS.JobService.Models.Dto
{
    public class RawTransactionDto
    {
        public string LoginName { get; set; }

        public string SubscriberName { get; set; }

        public DateTime TransTime { get; set; }

        public string UserAgent { get; set; }

        public string IP { get; set; }

        public long? PluginID { get; set; }

        public string URL { get; set; }

        public string Action { get; set; }

        public string ActionResult { get; set; }

        public string FlaggedCode { get; set; }

        public long? DeviceID { get; set; }

        public string DeviceCode { get; set; }

        public string FingerprintCode { get; set; }
    }
}
