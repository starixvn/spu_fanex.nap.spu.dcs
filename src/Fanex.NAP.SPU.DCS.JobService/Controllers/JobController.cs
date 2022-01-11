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
        private readonly IFPDeviceJobService fPDeviceJobService;
        private readonly ITransactionJobService transactionJobService;
        private readonly IFPTransactionJobService fPTransactionJobService;
        private readonly ISumTransactionJobService sumTransactionJobService;
        private readonly INotifyTransactionJobService notifyTransactionJobService;

        public JobController(ISentryClient sentryClient,
            IUserAgentJobService userAgentJobService,
            IDeviceJobService deviceJobService,
            IFPDeviceJobService fPDeviceJobService,
            ITransactionJobService transactionJobService,
            IFPTransactionJobService fPTransactionJobService,
            ISumTransactionJobService sumTransactionJobService,
            INotifyTransactionJobService notifyTransactionJobService)
        {
            this.sentryClient = sentryClient;
            this.userAgentJobService = userAgentJobService;
            this.deviceJobService = deviceJobService;
            this.fPDeviceJobService = fPDeviceJobService;
            this.transactionJobService = transactionJobService;
            this.fPTransactionJobService = fPTransactionJobService;
            this.sumTransactionJobService = sumTransactionJobService;
            this.notifyTransactionJobService = notifyTransactionJobService;
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
        public async Task<bool> TransformDevice([FromForm]int size, [FromForm] int numberOfBatch)
        {            
            return await this.deviceJobService.Execute(size, numberOfBatch);
        }

        [HttpPost("transformfpdevice")]
        public async Task<bool> TransformFPDevice([FromForm] int size, [FromForm] int numberOfBatch)
        {
            return await this.fPDeviceJobService.Execute(size, numberOfBatch);
        }

        [HttpPost("transformtransaction")]
        public async Task<bool> TransformTransaction([FromForm] int size, [FromForm] int numberOfBatch)
        {
            return await this.transactionJobService.Execute(size, numberOfBatch);
        }

        [HttpPost("transformfptransaction")]
        public async Task<bool> TransformFPTransaction([FromForm] int size, [FromForm] int numberOfBatch)
        {
            return await this.fPTransactionJobService.Execute(size, numberOfBatch);
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
    }
}
