using System;

namespace Fanex.NAP.SPU.DCS.Features.DataTrace.Model
{
    public class SendTransactionToLogstashRequest
    {
        public string SubscriberName { get; set; }
        public string AccountName { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int BatchSize { get; set; } = 25000;
    }
}
