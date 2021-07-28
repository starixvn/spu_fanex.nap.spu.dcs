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
        public bool UserAgent(int size)
        {
            return this.userAgentJobService.Execute(size);

        }

        [HttpPost("transformdevice")]
        public async Task<bool> TransformDevice(int size, int numberOfBatch)
        {
            sentryClient.CaptureMessage($"Transform Device {DateTime.Now}");

            return await this.deviceJobService.Execute(size, numberOfBatch);
        }

        [HttpPost("transformtransaction")]
        public bool TransformTransaction(int size, int numberOfBatch)
        {
            return this.transactionJobService.Execute(size, numberOfBatch);
        }
    }
}
