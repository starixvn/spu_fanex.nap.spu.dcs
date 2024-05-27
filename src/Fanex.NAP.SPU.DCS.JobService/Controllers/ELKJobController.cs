using Fanex.NAP.SPU.DCS.JobService.Services;
using Microsoft.AspNetCore.Mvc;

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
            return await elkJobService.Execute(DateTime.UtcNow.AddMonths(-1), DateTime.UtcNow.AddMonths(-1));
        }

        [HttpPost("bettingmessagecountmanual")]
        public async Task<bool> GetBettingMessageCount(DateTime startTime, DateTime endTime)
        {
            return await elkJobService.Execute(startTime, endTime);
        }
    }
}
