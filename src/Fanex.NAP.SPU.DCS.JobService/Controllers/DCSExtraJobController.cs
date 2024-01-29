namespace Fanex.NAP.SPU.DCS.JobService.Controllers
{
    using Fanex.NAP.SPU.DCS.JobService.Services;

    [ApiController]
    [Route("api/dcsextrajob")]
    public class DCSExtraJobController : Controller
    {
        private readonly IUserAgentJobService userAgentJobService;
        private readonly ITransactionJobService transactionJobService;
        private readonly IDeviceJobService deviceJobService;
        private readonly IArchiveJobServices archiveJobServices;

        public DCSExtraJobController(
            IUserAgentJobService userAgentJobService,
            ITransactionJobService transactionJobService,
            IDeviceJobService deviceJobService,
            IArchiveJobServices archiveJobServices)
        {
            this.userAgentJobService = userAgentJobService;
            this.transactionJobService = transactionJobService;
            this.deviceJobService = deviceJobService;
            this.archiveJobServices = archiveJobServices;
        }

        [HttpPost("useragent")]
        public async Task<bool> UserAgent([FromForm] int size)
        {
            return await userAgentJobService.Execute(size, true);
        }

        [HttpPost("transformtransaction")]
        public async Task<bool> TransformTransaction([FromForm] int size, [FromForm] int numberOfBatch)
        {
            return await transactionJobService.Execute(size, numberOfBatch, true);
        }

        [HttpPost("transformdevice")]
        public async Task<bool> TransformDevice([FromForm] int size, [FromForm] int numberOfBatch)
        {
            return await deviceJobService.Execute(size, numberOfBatch, true);
        }

        [HttpPost("updatemodifieddate")]
        public async Task<bool> UpdateModifiedDate([FromForm] int batchSize)
        {
            return await archiveJobServices.Execute(batchSize, true);
        }

        [HttpPost("deletebotcomponent")]
        public async Task<bool> DeleteBotComponent([FromForm] int retentionDays)
        {
            return await archiveJobServices.DeleteBotComponent(retentionDays, true);
        }
    }
}
