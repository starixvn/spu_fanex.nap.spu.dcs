using Fanex.NAP.SPU.DCS.Features.DataTrace.Model;

namespace Fanex.NAP.SPU.DCS.JobService.Models
{
    public class LogstashModel
    {
        public TransactionLogstashModel Message { get; set; }

        public string Tags { get; set; }
    }
}
