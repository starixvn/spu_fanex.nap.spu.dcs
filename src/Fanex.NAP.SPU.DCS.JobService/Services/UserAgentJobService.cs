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
        
        public UserAgentJobService(ISentryClient sentryClient)
        {
            this.sentryClient = sentryClient;            
        }

        public Task<bool> Execute(int size)
        {
            try
            {
                var userAgentService = new Features.UserAgent.Services();

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
