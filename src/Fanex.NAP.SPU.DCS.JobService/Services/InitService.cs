using Fanex.NAP.SPU.DCS.Features.UserAgent;
using Microsoft.Extensions.DependencyInjection;

namespace Fanex.NAP.SPU.DCS.JobService.Services
{
    public static class InitService
    {
        public static void RegisterService(this IServiceCollection services)
        {
            services.AddSingleton<IUserAgentDataAccess, UserAgentDataAccess>();
            services.AddSingleton<IUserAgentServices, UserAgentServices>();
        }
    }
}
