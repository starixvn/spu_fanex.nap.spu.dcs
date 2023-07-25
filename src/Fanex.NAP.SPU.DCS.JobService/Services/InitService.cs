namespace Fanex.NAP.SPU.DCS.JobService.Services
{
    using Fanex.NAP.SPU.DCS.Features.AbnormalTransaction;
    using Fanex.NAP.SPU.DCS.Features.DCSExtra;
    using Fanex.NAP.SPU.DCS.Features.Devices;
    using Fanex.NAP.SPU.DCS.Features.Monitoring;
    using Fanex.NAP.SPU.DCS.Features.Transactions;
    using Fanex.NAP.SPU.DCS.Features.UserAgent;
    using Microsoft.Extensions.DependencyInjection;

    public static class InitService
    {
        public static void RegisterService(this IServiceCollection services)
        {
            RegisterUserAgentService(services);
            RegisterTransformDevice(services);
            RegisterTransformTransaction(services);
            RegisterMonitoring(services);
            RegisterAbnormalTransaction(services);
            RegisterDCSExtra(services);
        }

        private static void RegisterUserAgentService(this IServiceCollection services)
        {
            services.AddSingleton<IUserAgentDataAccess, UserAgentDataAccess>();
            services.AddSingleton<IUserAgentServices, UserAgentServices>();
            services.AddSingleton<IUserAgentJobService, UserAgentJobService>();
        }

        private static void RegisterTransformDevice(this IServiceCollection services)
        {
            services.AddSingleton<IDeviceDataAccess, DeviceDataAccess>();
            services.AddSingleton<IDeviceServices, DeviceServices>();
            services.AddSingleton<IDeviceJobService, DeviceJobService>();
        }

        private static void RegisterMonitoring(this IServiceCollection services)
        {
            services.AddSingleton<IMonitoringDataAccess, MonitoringDataAccess>();
            services.AddSingleton<IMonitoringServices, MonitoringServices>();
            services.AddSingleton<ISumTransactionJobService, SumTransactionJobService>();
            services.AddSingleton<INotifyTransactionJobService, NotifyTransactionJobService>();
        }

        private static void RegisterTransformTransaction(this IServiceCollection services)
        {
            services.AddSingleton<ITransactionDataAccess, TransactionDataAccess>();
            services.AddSingleton<ITransactionServices, TransactionServices>();
            services.AddSingleton<ITransactionJobService, TransactionJobService>();
        }

        private static void RegisterAbnormalTransaction(this IServiceCollection services)
        {
            services.AddSingleton<IAbnormalTransactionDataAccess, AbnormalTransactionDataAccess>();
            services.AddSingleton<IAbnormalTransactionServices, AbnormalTransactionServices>();
            services.AddSingleton<IAbnormalTransactionJobService, AbnormalTransactionJobService>();
        }

        private static void RegisterDCSExtra(this IServiceCollection services)
        {
            services.AddSingleton<IDCSExtraDataAccess, DCSExtraDataAccess>();
            services.AddSingleton<IDCSExtraServices, DCSExtraServices>();
            services.AddSingleton<IDCSExtraService, DCSExtraService>();
        }
    }
}
