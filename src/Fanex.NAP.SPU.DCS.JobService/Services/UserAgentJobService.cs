namespace Fanex.NAP.SPU.DCS.JobService.Services
{
    using Fanex.NAP.SPU.DCS.Features.UserAgent;
    using UAParser.FormFactor.Models;

    public interface IUserAgentJobService
    {
        Task<bool> Execute(int size);

        ClientInfo ConvertUserAgent(string userAgent);

    }
    public class UserAgentJobService(ILogService logService, IUserAgentServices userAgentService) : IUserAgentJobService
    {
        private readonly ILogService logService = logService;
        private readonly IUserAgentServices userAgentService = userAgentService;

        public ClientInfo ConvertUserAgent(string userAgent)
        {
            return this.userAgentService.ConvertUserAgent(userAgent);
        }

        public async Task<bool> Execute(int size)
        {
            try
            {
                if (size < 0 ) 
                {
                    return false;
                }

                var userAgents = userAgentService.GetUserAgent(size);

                if (userAgents != null && userAgents.Any())
                {
                    var transformUserAgent = userAgentService.TransformUserAgent(userAgents);

                    if (transformUserAgent.Any())
                    {
                        var userAgentJson = JsonSerializer.Serialize(transformUserAgent);

                        userAgentService.UpdateUserAgent(userAgentJson);
                    }
                }
            }
            catch (Exception ex)
            {
                await logService.ErrorAsync(ex);

                return false;
            }

            return true;
        }
    }
}
