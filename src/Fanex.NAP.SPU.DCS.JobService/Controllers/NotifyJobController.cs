namespace Fanex.NAP.SPU.DCS.JobService.Controllers
{
    using Fanex.NAP.SPU.DCS.JobService.Services;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("api/notifyjob")]
    public class NotifyJobController : Controller
    {
        private readonly INotifyTransactionJobService notifyTransactionJobService;

        public NotifyJobController(
            INotifyTransactionJobService notifyTransactionJobService)
        {
            this.notifyTransactionJobService = notifyTransactionJobService;
        }

        [HttpPost("transaction")]
        public async Task<bool> Transaction()
        {
            return await this.notifyTransactionJobService.Execute();
        }

        [HttpPost("monitortransaction")]
        public async Task<bool> MonitorTransaction([FromForm] string rptDateStr, [FromForm] int batchSize, [FromForm] int limitShowCust)
        {
            return await notifyTransactionJobService.MonitorTransactionAndNotify(rptDateStr, batchSize, limitShowCust);
        }

        [HttpPost("messagebyid")]
        public async Task<bool> MessageByID([FromForm] uint notifyMessageID)
        {
            return await notifyTransactionJobService.NotifyMessageByID(notifyMessageID);
        }

        [HttpPost("messagebynotifytype")]
        public async Task<bool> MessageByNotifyType([FromForm] string notifyType, [FromForm] int batchSize)
        {
            return await notifyTransactionJobService.NotifyMessageByNotifyType(notifyType, batchSize);
        }
    }
}
