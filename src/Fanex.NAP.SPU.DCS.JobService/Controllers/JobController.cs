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

        public JobController(ISentryClient sentryClient, IUserAgentJobService userAgentJobService)
        {
            this.sentryClient = sentryClient;
            this.userAgentJobService = userAgentJobService;
        }

        [HttpPost("useragent")]
        public async Task<bool> UserAgent(int size, string jobName)
        {
            sentryClient.CaptureMessage($"{jobName} is running at {DateTime.Now}");

            return await this.userAgentJobService.Execute(size);

        }
    }
}
