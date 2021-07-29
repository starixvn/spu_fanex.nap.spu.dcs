using Fanex.NAP.SPU.DCS.JobService.Services;
using Microsoft.AspNetCore.Mvc;
using Sentry;
using System;
using System.Threading.Tasks;

namespace Fanex.NAP.SPU.DCS.JobService.Controllers
{
    [ApiController]
    [Route("api/job")]
    public class JobController : ControllerBase
    {
        private readonly ISentryClient sentryClient;
        private readonly IUserAgentJobService userAgentJobService;
        private readonly IDeviceJobService deviceJobService;
        private readonly ITransactionJobService transactionJobService;

        public JobController(ISentryClient sentryClient,
            IUserAgentJobService userAgentJobService,
            IDeviceJobService deviceJobService,
            ITransactionJobService transactionJobService)
        {
            this.sentryClient = sentryClient;
            this.userAgentJobService = userAgentJobService;
            this.deviceJobService = deviceJobService;
            this.transactionJobService = transactionJobService;
        }

        [HttpPost("useragent")]
        public Task<bool> UserAgent([FromForm] int size)
        {
            var result = this.userAgentJobService.Execute(size);

            return Task.FromResult(result);

        }

        [HttpPost("transformdevice")]
        public async Task<bool> TransformDevice([FromForm]int size, [FromForm] int numberOfBatch)
        {
            sentryClient.CaptureMessage($"Transform Device {DateTime.Now}");

            return await this.deviceJobService.Execute(size, numberOfBatch);
        }

        [HttpPost("transformtransaction")]
        public async Task<bool> TransformTransaction([FromForm] int size, [FromForm] int numberOfBatch)
        {
            return await this.transactionJobService.Execute(size, numberOfBatch);
        }
    }
}
