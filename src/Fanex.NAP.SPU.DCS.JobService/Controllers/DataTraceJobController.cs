namespace Fanex.NAP.SPU.DCS.JobService.Controllers
{
    using System.Threading.Tasks;
    using System;
    using Fanex.NAP.SPU.DCS.JobService.Services;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("api/datatracejob")]
    public class DataTraceJobController : ControllerBase
    {
        private readonly IDataTraceJobService dataTraceJobService;

        public DataTraceJobController(IDataTraceJobService dataTraceJobService)
        {
            this.dataTraceJobService = dataTraceJobService;
        }

        [HttpPost("summarylogintransaction")]
        public async Task<bool> SummaryLoginTransaction([FromForm] int batchSize = 50000)
        {
            return await dataTraceJobService.SummaryLoginTransactionsAsync(batchSize);
        }

        [HttpPost("summarybettransaction")]
        public async Task<bool> SummaryBetTransaction([FromForm] DateTime? fromDate, [FromForm] DateTime? toDate)
        {
            return await dataTraceJobService.SummaryBetTransactionAsync(fromDate, toDate);
        }

        [HttpPost("summarymissingbettransaction")]
        public async Task<bool> SummaryMissingBetTransaction([FromForm] int batchSize = 50000)
        {
            return await dataTraceJobService.SummaryMissingBetTransactionAsync(batchSize);
        }

        [HttpPost("summarymissinglogintransaction")]
        public async Task<bool> SummaryMissingLoginTransaction([FromForm] int batchSize = 50000)
        {
            return await dataTraceJobService.SummaryMissingLoginTransactionAsync(batchSize);
        }

        [HttpPost("initsummarybettransaction")]
        public async Task<bool> InitSummaryBetTransaction([FromForm] DateTime fromDate, [FromForm] DateTime toDate)
        {
            return await dataTraceJobService.InitBetTransactionAsync(fromDate, toDate);
        }
    }
}
