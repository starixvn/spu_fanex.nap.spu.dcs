using System;

namespace Fanex.NAP.SPU.DCS.Features.DCSExtra.Model
{
    public class RawTransactionModel
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

        public int? Flagged { get; set; }

        public long? DeviceID { get; set; }
    }
}
