

namespace Fanex.NAP.SPU.DCS.ServiceAPI.Services
{
    using Fanex.Logging;
    using Fanex.NAP.SPU.DCS.Features.DataTrace;

    public static class InitService
    {
        public static void RegisterService(this IServiceCollection services)
        {
            RegisterTransactionService(services);
            RegisterLogService(services);


        }

        private static void RegisterTransactionService(this IServiceCollection services)
        {
            services.AddSingleton<IDataTraceDataAccess, DataTraceDataAccess>();
            services.AddSingleton<IDataTraceServices, DataTraceServices>();
        }

        private static void RegisterLogService(this IServiceCollection services)
        {
            services.AddSingleton<ILogger, Logger>();
            services.AddSingleton<ILogService, LogService>();
        }
    }
}
