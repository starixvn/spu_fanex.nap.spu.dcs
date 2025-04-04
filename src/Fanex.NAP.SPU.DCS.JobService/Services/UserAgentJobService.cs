namespace Fanex.NAP.SPU.DCS.JobService.Services
{
    using Fanex.NAP.SPU.DCS.Features.UserAgent;
    using UAParser.FormFactor.Models;

    public interface IUserAgentJobService
    {
        bool Execute(int size, bool isTransformExtra);

        ClientInfo ConvertUserAgent(string userAgent);

    }
    public class UserAgentJobService : IUserAgentJobService
    {
        private readonly ILogService logService;
        private readonly IUserAgentServices userAgentService;
        private readonly IAppSettings appSettings;

        public UserAgentJobService(ILogService logService, IUserAgentServices userAgentService, IAppSettings appSettings)
        {
            this.logService = logService;
            this.userAgentService = userAgentService;
            this.appSettings = appSettings;
        }

        public ClientInfo ConvertUserAgent(string userAgent)
        {
            return this.userAgentService.ConvertUserAgent(userAgent);
        }

        public bool Execute(int size, bool isTransformExtra)
        {
            try
            {
                if (size < 0 ) 
                {
                    return false;
                }

                var isReadyDbExtra = isTransformExtra && appSettings.IsReadyDbExtra;

                var userAgents = userAgentService.GetUserAgent(size, isReadyDbExtra);

                if (userAgents != null && userAgents.Any())
                {
                    var transformUserAgent = userAgentService.TransformUserAgent(userAgents);

                    if (transformUserAgent.Any())
                    {
                        var userAgentJson = JsonSerializer.Serialize(transformUserAgent);

                        userAgentService.UpdateUserAgent(userAgentJson, isReadyDbExtra);
                    }
                }
            }
            catch (Exception ex)
            {
                logService.ErrorAsync(ex);

                return false;
            }

            return true;
        }
    }
}
