namespace Fanex.NAP.SPU.DCS.JobService.Controllers
{
    using Fanex.NAP.SPU.DCS.JobService.Services;

    [ApiController]
    [Route("api/testjob")]
    public class TestJobController : Controller
    {
        private readonly INotifyTransactionJobService notifyTransactionJobService;
        public TestJobController(INotifyTransactionJobService notifyTransactionJobService)
        {
            this.notifyTransactionJobService = notifyTransactionJobService;
        }

        [HttpPost("telegramnotifymessage")]
        public async Task<bool> TelegramNotifyMessage()
        {
            return await this.notifyTransactionJobService.TestTelegramNotifyMessage();
        }

        [HttpPost("sentry")]
        public async Task<bool> Sentry([FromForm] string message)
        {
            return await notifyTransactionJobService.TestSentryCaptureMessage(message);
        }
    }
}
