using Fanex.NAP.SPU.DCS.JobService.Services;

namespace Fanex.NAP.SPU.DCS.JobService.Controllers
{
    [ApiController]
    [Route("api/elkjob")]
    public class ELKJobController : ControllerBase
    {
        private readonly IELKJobService elkJobService;

        public ELKJobController(IELKJobService elkJobService)
        {
            this.elkJobService = elkJobService;
        }

        [HttpPost("bettingmessagecount")]
        public async Task<bool> GetBettingMessageCount()
        {
            var utcNow = DateTime.UtcNow;

            return await elkJobService.Execute(new DateTime(utcNow.Year, utcNow.Month - 1, 1), new DateTime(utcNow.Year, utcNow.Month, 1));
            
        }

        [HttpPost("bettingmessagecountdaily")]
        public async Task<bool> GetBettingMessageCountDaily()
        {
            var utcNow = DateTime.UtcNow;

            return await elkJobService.Execute(new DateTime(utcNow.Year, utcNow.Month, 1), utcNow);

        }

        [HttpPost("bettingmessagecountmanual")]
        public async Task<bool> GetBettingMessageCount(DateTime startTime, DateTime endTime)
        {
            return await elkJobService.Execute(startTime, endTime);
        }
    }
}
