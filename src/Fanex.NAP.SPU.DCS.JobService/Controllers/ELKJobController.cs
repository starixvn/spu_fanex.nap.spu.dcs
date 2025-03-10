using Fanex.NAP.SPU.DCS.JobService.Services;
using System.Diagnostics.CodeAnalysis;

#pragma warning disable
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
            var utcNow = DateTime.UtcNow;

            var lastmonth = utcNow.AddMonths(-1);

            return await elkJobService.Execute(new DateTime(lastmonth.Year, lastmonth.Month, 1), new DateTime(utcNow.Year, utcNow.Month, 1));

        }

        [HttpPost("bettingmessagecountdaily")]
        public async Task<bool> GetBettingMessageCountDaily()
        {
            var utcNow = DateTime.UtcNow;

            return await elkJobService.Execute(new DateTime(utcNow.Year, utcNow.Month, 1), utcNow);

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

        [HttpPost("bettingmessagecountmanual")]
        public async Task<bool> GetBettingMessageCount(DateTime startTime, DateTime endTime)
        {
            return await elkJobService.Execute(startTime, endTime);
        }
    }
}
#pragma warning restore
