using Fanex.NAP.SPU.DCS.Features.UserAgent;
using Newtonsoft.Json;
using Sentry;
using System.Linq;
using System.Threading.Tasks;

namespace Fanex.NAP.SPU.DCS.JobService.Services
{
    public interface IUserAgentJobService
    {
        Task<bool> Execute(int size);

    }
    public class UserAgentJobService : IUserAgentJobService
    {
        private readonly ISentryClient sentryClient;
        private readonly IUserAgentServices userAgentService;

        public UserAgentJobService(ISentryClient sentryClient, UserAgentServices userAgentService)
        {
            this.sentryClient = sentryClient;
            this.userAgentService = userAgentService;
        }

        public Task<bool> Execute(int size)
        {
            try
            {                
                var userAgents = userAgentService.GetUserAgent(size);

                if (userAgents != null && userAgents.Any())
                {
                    var transformUserAgent = userAgentService.TransformUserAgent(userAgents);

                    if (transformUserAgent.Any())
                    {
                        var userAgentJson = JsonConvert.SerializeObject(transformUserAgent);

                        userAgentService.UpdateUserAgent(userAgentJson);
                    }
                }

                return Task.FromResult(true);
            }
            catch (System.Exception ex)
            {
                sentryClient.CaptureException(ex);
            }

            return Task.FromResult(false);
        }
    }
}
