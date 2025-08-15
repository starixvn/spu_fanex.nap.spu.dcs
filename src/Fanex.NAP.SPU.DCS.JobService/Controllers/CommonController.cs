using Fanex.NAP.SPU.DCS.JobService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace Fanex.NAP.SPU.DCS.JobService.Controllers
{
    [ApiController]
    public class CommonController(IAppSettings appSettings) : ControllerBase
    {
        private readonly IAppSettings appSettings = appSettings;
         
        [HttpGet("whoami")]
        public WhoamiModel Index()
        {
            var httpConnectionFeature = HttpContext.Features.Get<IHttpConnectionFeature>();
            var localIpAddress = httpConnectionFeature?.LocalIpAddress;

            return new WhoamiModel
            {
                Environment = appSettings.Environment,
                ServerName = Environment.MachineName,
                ServerTime = DateTime.Now,
                LocalIP = localIpAddress?.ToString()
            };
        }
    }
}
