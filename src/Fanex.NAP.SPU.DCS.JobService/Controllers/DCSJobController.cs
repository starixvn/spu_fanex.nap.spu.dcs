namespace Fanex.NAP.SPU.DCS.JobService.Controllers
{
    using System.Threading.Tasks;
    using Fanex.NAP.SPU.DCS.JobService.Services;
    using Microsoft.AspNetCore.Mvc;
    using UAParser.FormFactor.Models;

    [ApiController]
    [Route("api/dcsjob")]
    public class DCSJobController : ControllerBase
    {
        private readonly IUserAgentJobService userAgentJobService;
        private readonly IDeviceJobService deviceJobService;
        private readonly ITransactionJobService transactionJobService;
        private readonly ISumTransactionJobService sumTransactionJobService;
        private readonly IAbnormalTransactionJobService abnormalTransactionJobService;
        private readonly IArchiveJobServices archiveJobServices;

        public DCSJobController(
            IUserAgentJobService userAgentJobService,
            IDeviceJobService deviceJobService,
            ITransactionJobService transactionJobService,
            ISumTransactionJobService sumTransactionJobService,
            IAbnormalTransactionJobService abnormalTransactionJobService,
            IArchiveJobServices archiveJobServices)
        {
            this.userAgentJobService = userAgentJobService;
            this.deviceJobService = deviceJobService;
            this.transactionJobService = transactionJobService;
            this.sumTransactionJobService = sumTransactionJobService;
            this.abnormalTransactionJobService = abnormalTransactionJobService;
            this.archiveJobServices = archiveJobServices;
        }

        [HttpPost("useragent")]
        public async Task<bool> UserAgent([FromForm] int size)
        {
            return await this.userAgentJobService.Execute(size, false);
        }

        [HttpGet("ConvertUserAgent")]
        public ClientInfo ConvertUserAgent(string userAgent)
        {
            return this.userAgentJobService.ConvertUserAgent(userAgent);
        }

        [HttpPost("transformdevice")]
        public async Task<bool> TransformDevice([FromForm] int size, [FromForm] int numberOfBatch)
        {
            return await this.deviceJobService.Execute(size, numberOfBatch, false);
        }

        [HttpPost("transformtransaction")]
        public async Task<bool> TransformTransaction([FromForm] int size, [FromForm] int numberOfBatch)
        {
            return await this.transactionJobService.Execute(size, numberOfBatch, false);
        }

        [HttpPost("sumtransaction")]
        public async Task<bool> SumTransaction()
        {
            return await this.sumTransactionJobService.Execute();
        }

        [HttpPost("pushabnormaltransaction")]
        public async Task<bool> PushAbnormalTransaction([FromForm] int size)
        {
            return await abnormalTransactionJobService.PushAbnormalTransaction(size);
        }

        [HttpPost("updatemodifieddate")]
        public async Task<bool> UpdateModifiedDate([FromForm] int batchSize)
        {
            return await archiveJobServices.Execute(batchSize, false);
        }

        [HttpPost("deletebotcomponent")]
        public async Task<bool> DeleteBotComponent([FromForm] int retentionDays)
        {
            return await archiveJobServices.DeleteBotComponent(retentionDays, false);
        }
    }
}
