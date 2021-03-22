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

        public JobController(ISentryClient sentryClient, 
            IUserAgentJobService userAgentJobService, 
            IDeviceJobService deviceJobService)
        {
            this.sentryClient = sentryClient;
            this.userAgentJobService = userAgentJobService;
            this.deviceJobService = deviceJobService;
        }

        [HttpPost("useragent")]
        public async Task<bool> UserAgent(string jobName, int size)
        {
            sentryClient.CaptureMessage($"{jobName} is running at {DateTime.Now}");

            return await this.userAgentJobService.Execute(size);

        }

        [HttpPost("transformdevice")]
        public async Task<bool> TransformDevice(string jobName, int size, int numberOfBatch)
        {
            sentryClient.CaptureMessage($"{jobName} is running at {DateTime.Now}");

            return await this.deviceJobService.Execute(size, numberOfBatch);

        }
    }
}
