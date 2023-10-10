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

        [HttpPost("summarybettickets")]
        public async Task<bool> SummaryBetTickets([FromForm] DateTime? fromDate, [FromForm] DateTime? toDate)
        {
            return await dataTraceJobService.SummaryBetTicketAsync(fromDate, toDate);
        }

        [HttpPost("summarymissingbetticket")]
        public async Task<bool> SummaryMissingBetTicket([FromForm] int batchSize = 50000)
        {
            return await dataTraceJobService.SummaryMissingBetTicketAsync(batchSize);
        }

        [HttpPost("summarymissinglogintransaction")]
        public async Task<bool> SummaryMissingLoginTransaction([FromForm] int batchSize = 50000)
        {
            return await dataTraceJobService.SummaryMissingLoginTransactionAsync(batchSize);
        }
    }
}
