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
        public async Task<bool> UserAgent(string jobName, [FromForm] int size)
        {
            sentryClient.CaptureMessage($"{jobName} is running at {DateTime.Now}");

            return await this.userAgentJobService.Execute(size);

        }

        [HttpPost("transformdevice")]
        public async Task<bool> TransformDevice(string jobName, [FromForm] int size, [FromForm] int numberOfBatch)
        {
            sentryClient.CaptureMessage($"{jobName} is running at {DateTime.Now}");

            return await this.deviceJobService.Execute(size, numberOfBatch);

        }

        [HttpPost("transformtransaction")]
        public async Task<bool> TransformTransaction(string jobName, [FromForm] int size, [FromForm] int numberOfBatch)
        {
            sentryClient.CaptureMessage($"{jobName} is running at {DateTime.Now}");

            return await this.transactionJobService.Execute(size, numberOfBatch);

        }
    }
}
