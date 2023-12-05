namespace Fanex.NAP.SPU.DCS.JobService.Services
{
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.NAP.SPU.DCS.Features.UserAgent;
    using Newtonsoft.Json;
    using Sentry;
    using UAParser.FormFactor.Models;

    public interface IUserAgentJobService
    {
        Task<bool> Execute(int size, bool isTransformExtra);

        ClientInfo ConvertUserAgent(string userAgent);

    }
    public class UserAgentJobService : IUserAgentJobService
    {
        private readonly ISentryClient sentryClient;
        private readonly IUserAgentServices userAgentService;
        private readonly IAppSettings appSettings;

        public UserAgentJobService(ISentryClient sentryClient, IUserAgentServices userAgentService, IAppSettings appSettings)
        {
            this.sentryClient = sentryClient;
            this.userAgentService = userAgentService;
            this.appSettings = appSettings;
        }

        public ClientInfo ConvertUserAgent(string userAgent)
        {
            return this.userAgentService.ConvertUserAgent(userAgent);
        }

        public async Task<bool> Execute(int size, bool isTransformExtra)
        {
            try
            {
                if (size > 0)
                {
                    JobRun(size, isTransformExtra);
                }
            }
            catch (System.Exception ex)
            {
                sentryClient.CaptureException(ex);

                return await Task.FromResult(false);
            }

            return await Task.FromResult(true);
        }

        private void JobRun(int size, bool isTransformExtra)
        {
            if (isTransformExtra)
            {
                if (appSettings.IsReadyDbExtra)
                    TransformUserAgent(size, true);
            }
            else
            {
                TransformUserAgent(size, false);
            }
        }

        private void TransformUserAgent(int size, bool isReadyDbExtra)
        {
            try
            {
                var userAgents = userAgentService.GetUserAgent(size, isReadyDbExtra);

                if (userAgents != null && userAgents.Any())
                {
                    var transformUserAgent = userAgentService.TransformUserAgent(userAgents);

                    if (transformUserAgent.Any())
                    {
                        var userAgentJson = JsonConvert.SerializeObject(transformUserAgent);

                        userAgentService.UpdateUserAgent(userAgentJson, isReadyDbExtra);
                    }
                }
            }
            catch (System.Exception ex)
            {
                sentryClient.CaptureException(ex);

                throw;
            }
        }
    }
}
