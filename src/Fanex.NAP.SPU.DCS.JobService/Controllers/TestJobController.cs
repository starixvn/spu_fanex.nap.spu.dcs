namespace Fanex.NAP.SPU.DCS.JobService.Controllers
{
    using Fanex.NAP.SPU.DCS.JobService.Services;

    [ApiController]
    [Route("api/testjob")]
    public class TestJobController : Controller
    {
        private readonly INotifyTransactionJobService notifyTransactionJobService;
        private readonly IAppSettings appSettings;
        
        public TestJobController(INotifyTransactionJobService notifyTransactionJobService, IAppSettings appSettings)
        {
            this.notifyTransactionJobService = notifyTransactionJobService;
            this.appSettings = appSettings;
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

        [HttpPost("Decrypt")]
        public string Decrypt()
        {
            return Fanex.Data.Cryptography.DecryptPassword("l4exNQ2KUgCjzhehUWfktg==");
        }

        [HttpGet("check-vault-enable")]
        public ActionResult<string> CheckVault()
        {
            if (!appSettings.VaultEnable)
            {
                return Ok("Vault is disabled!");
            }

            return Ok("Vault is enabled!");
        }
    }
}
