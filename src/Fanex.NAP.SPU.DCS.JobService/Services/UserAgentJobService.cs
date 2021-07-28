using Fanex.NAP.SPU.DCS.Features.UserAgent;
using Newtonsoft.Json;
using Sentry;
using System.Linq;
using System.Threading.Tasks;

namespace Fanex.NAP.SPU.DCS.JobService.Services
{
    public interface IUserAgentJobService
    {
        bool Execute(int size);

    }
    public class UserAgentJobService : IUserAgentJobService
    {
        private readonly ISentryClient sentryClient;
        private readonly IUserAgentServices userAgentService;

        public UserAgentJobService(ISentryClient sentryClient, IUserAgentServices userAgentService)
        {
            this.sentryClient = sentryClient;
            this.userAgentService = userAgentService;
        }

        public bool Execute(int size)
        {
            try
            {
                if(size > 0)
                {
                    JobRun(size);
                }                                
            }
            catch (System.Exception ex)
            {
                sentryClient.CaptureException(ex);

                return false;
            }

            return true;            
        }

        private void JobRun(int size)
        {
            var userAgents = userAgentService.GetUserAgent(size);

            if (userAgents != null && userAgents.Any())
            {
                var transformUserAgent = userAgentService.TransformUserAgent(userAgents);

                if (transformUserAgent.Any())
                {
                    var userAgentJson = JsonConvert.SerializeObject(transformUserAgent);

                    var result = userAgentService.UpdateUserAgent(userAgentJson);

                    if (result.ErrorCode != 0)
                    {
                        sentryClient.CaptureMessage(result.ErrorMessage, Sentry.Protocol.SentryLevel.Error);
                    }
                }
            }
        }
    }
}
