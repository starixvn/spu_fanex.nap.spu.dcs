namespace Fanex.NAP.SPU.DCS.JobService.Controllers
{
    using Fanex.NAP.SPU.DCS.JobService.Services;
    using Microsoft.AspNetCore.Mvc;
    using Sentry;
    using System.Threading.Tasks;
    using UAParser.FormFactor.Models;

    [ApiController]
    [Route("api/job")]
    public class JobController : ControllerBase
    {
        private readonly ISentryClient sentryClient;
        private readonly IUserAgentJobService userAgentJobService;
        private readonly IDeviceJobService deviceJobService;
        private readonly ITransactionJobService transactionJobService;
        private readonly ISumTransactionJobService sumTransactionJobService;
        private readonly INotifyTransactionJobService notifyTransactionJobService;
        private readonly IAbnormalTransactionJobService abnormalTransactionJobService;

        public JobController(ISentryClient sentryClient,
            IUserAgentJobService userAgentJobService,
            IDeviceJobService deviceJobService,
            ITransactionJobService transactionJobService,
            ISumTransactionJobService sumTransactionJobService,
            INotifyTransactionJobService notifyTransactionJobService,
            IAbnormalTransactionJobService abnormalTransactionJobService)
        {
            this.sentryClient = sentryClient;
            this.userAgentJobService = userAgentJobService;
            this.deviceJobService = deviceJobService;
            this.transactionJobService = transactionJobService;
            this.sumTransactionJobService = sumTransactionJobService;
            this.notifyTransactionJobService = notifyTransactionJobService;
            this.abnormalTransactionJobService = abnormalTransactionJobService;
        }

        [HttpPost("useragent")]
        public Task<bool> UserAgent([FromForm] int size)
        {
            var result = this.userAgentJobService.Execute(size);

            return Task.FromResult(result);

        }

        [HttpGet("ConvertUserAgent")]
        public ClientInfo ConvertUserAgent(string userAgent)
        {
            return this.userAgentJobService.ConvertUserAgent(userAgent);
        }

        [HttpPost("transformdevice")]
        public async Task<bool> TransformDevice([FromForm] int size, [FromForm] int numberOfBatch)
        {
            return await this.deviceJobService.Execute(size, numberOfBatch);
        }

        [HttpPost("transformtransaction")]
        public async Task<bool> TransformTransaction([FromForm] int size, [FromForm] int numberOfBatch)
        {
            return await this.transactionJobService.Execute(size, numberOfBatch);
        }

        [HttpPost("sumtransaction")]
        public async Task<bool> SumTransaction()
        {
            return await this.sumTransactionJobService.Execute();
        }

        [HttpPost("notifytransaction")]
        public async Task<bool> NotifyTransaction()
        {
            return await this.notifyTransactionJobService.Execute();
        }

        [HttpPost("testtelegramnotifymessage")]
        public async Task<bool> TestTelegramNotifyMessage()
        {
            return await this.notifyTransactionJobService.TestTelegramNotifyMessage();
        }

        [HttpPost("pushabnormaltransaction")]
        public async Task<bool> PushAbnormalTransaction([FromForm] int size)
        {
            return await abnormalTransactionJobService.PushAbnormalTransaction(size);
        }

        [HttpPost("monitortransactionlevelaccount")]
        public async Task<bool> MonitorTransactionLevelAccount([FromForm] int batchSize)
        {
            return await notifyTransactionJobService.MonitorTransactionLevelAccount(batchSize);
        }

        [HttpPost("testSentry")]
        public async Task<bool> TestSentry([FromForm] string message)
        {
            return await notifyTransactionJobService.TestSentryCaptureMessage(message);
        }

        [HttpPost("notifymessage")]
        public async Task<bool> NotifyMessage([FromForm] string groupId, [FromForm] string notifyType, [FromForm] int batchSize)
        {
            return await notifyTransactionJobService.NotifyMessage(groupId, notifyType, batchSize);
        }
    }
}
