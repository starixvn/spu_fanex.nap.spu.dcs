using Fanex.NAP.SPU.DCS.JobService.Services;
using System.Diagnostics.CodeAnalysis;

namespace Fanex.NAP.SPU.DCS.JobService.Controllers
{
    [ExcludeFromCodeCoverage(Justification = "Excluded from code coverage")]
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

            var startTime = new DateTime(lastMonth.Year, lastMonth.Month, 1, 0, 0, 0);
            var endTime = new DateTime(utcNow.Year, utcNow.Month, 1, 0, 0, 0);

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
            var utcNow = DateTime.UtcNow;

            return await elkJobService.Execute(new DateTime(utcNow.Year, utcNow.Month, 1, 0, 0, 0), utcNow);
        }

        [HttpPost("bettingmessagecountdailymanual")]
        public async Task<bool> GetBettingMessageCountDaily(DateTime startTime, DateTime endTime)
        {            
            return await elkJobService.Execute(startTime, endTime);
        }

        [HttpPost("bettingmessagecountdailyleaguegroup")]
        public async Task<bool> GetBettingMessageCountDailyByLeagueGroup()
        {
            var utcNow = DateTime.UtcNow;

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
