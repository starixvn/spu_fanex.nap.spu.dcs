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
            var utcNow = DateTimeOffset.UtcNow;
            var lastMonth = utcNow.AddMonths(-1);

            var startTime = new DateTimeOffset(lastMonth.Year, lastMonth.Month, 1, 0, 0, 0, TimeSpan.Zero);
            var endTime = new DateTimeOffset(utcNow.Year, utcNow.Month, 1, 0, 0, 0, TimeSpan.Zero);

            return await elkJobService.Execute(startTime, endTime);
        }

        [HttpPost("bettingmessagecountmanual")]
        public async Task<bool> GetBettingMessageCount(DateTime startTime, DateTime endTime)
        {
            return await elkJobService.Execute(startTime, endTime);
        }

        [HttpPost("bettingmessagecountdaily")]
        public async Task<bool> GetBettingMessageCountDaily()
        {
            var utcNow = DateTimeOffset.UtcNow;

            return await elkJobService.Execute(new DateTimeOffset(utcNow.Year, utcNow.Month, 1, 0, 0, 0, TimeSpan.Zero), utcNow);
        }

        [HttpPost("bettingmessagecountdailymanual")]
        public async Task<bool> GetBettingMessageCountDaily(DateTime startTime, DateTime endTime)
        {            
            return await elkJobService.Execute(startTime, endTime);
        }

        [HttpPost("bettingmessagecountdailyleaguegroup")]
        public async Task<bool> GetBettingMessageCountDailyByLeagueGroup()
        {
            var utcNow = DateTimeOffset.UtcNow;

            var yesterday = utcNow.AddDays(-1);

            var beforeYesterday = yesterday.AddDays(-1);

            await elkJobService.Execute(beforeYesterday, yesterday, beforeYesterday);

            return await elkJobService.Execute(yesterday, utcNow, yesterday);
        }

        [HttpPost("bettingmessagecountdailyleaguegroupmanual")]
        public async Task<bool> GetBettingMessageCountByLeagueGroup(DateTime startTime, DateTime endTime)
        {            
            return await elkJobService.Execute(startTime, endTime, startTime);
      }
    }
}
