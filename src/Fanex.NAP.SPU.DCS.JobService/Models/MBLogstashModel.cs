using Fanex.NAP.SPU.DCS.Features.DataTrace.Model;

namespace Fanex.NAP.SPU.DCS.JobService.Models
{
    public class MBLogstashModel
    {
        public MBTransactionLogstashModel Message { get; set; }

        public string Tags { get; set; }
    }
}
