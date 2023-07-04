namespace Fanex.NAP.SPU.DCS.JobService.Services
{
    using Fanex.NAP.SPU.DCS.Features.UserAgent;
    using Newtonsoft.Json;
    using Sentry;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using UAParser.FormFactor.Models;

    public interface IUserAgentJobService
    {
        Task<bool> Execute(int size);

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

        public async Task<bool> Execute(int size)
        {
            try
            {
                if(size > 0)
                {
                    await JobRun(size);
                }                                
            }
            catch (System.Exception ex)
            {
                sentryClient.CaptureException(ex);

                return await Task.FromResult(false);
            }

            return await Task.FromResult(true);            
        }

        private async Task JobRun(int size)
        {
            var tasks = new List<Task>();

            if (appSettings.IsReadyDbExtra)
            {
                TransformUserAgent(size, true);

                tasks.Add(Task.Run(() =>
                {
                    TransformUserAgent(size, true);
                }));
            }
            
            tasks.Add(Task.Run(() =>
            {
                TransformUserAgent(size, false);
            }));

            await Task.WhenAll(tasks.ToArray());
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
