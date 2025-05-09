namespace Fanex.NAP.SPU.DCS.JobService.Services
{
    using Fanex.Logging;
    using Fanex.NAP.SPU.DCS.Features.AbnormalTransaction;
    using Fanex.NAP.SPU.DCS.Features.ArchiveData;
    using Fanex.NAP.SPU.DCS.Features.Common;
    using Fanex.NAP.SPU.DCS.Features.DataTrace;
    using Fanex.NAP.SPU.DCS.Features.Devices;
    using Fanex.NAP.SPU.DCS.Features.ELK;
    using Fanex.NAP.SPU.DCS.Features.MBDevice;
    using Fanex.NAP.SPU.DCS.Features.MBTransactions;
    using Fanex.NAP.SPU.DCS.Features.Monitoring;
    using Fanex.NAP.SPU.DCS.Features.SystemSetting;
    using Fanex.NAP.SPU.DCS.Features.Transactions;
    using Fanex.NAP.SPU.DCS.Features.UserAgent;
    using Microsoft.Extensions.DependencyInjection;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage(Justification = "Excluded from code coverage")]
    public static class InitService
    {
        public static void RegisterService(this IServiceCollection services)
        {
            RegisterUserAgentService(services);
            RegisterTransformDevice(services);
            RegisterTransformTransaction(services);
            RegisterMonitoring(services);
            RegisterAbnormalTransaction(services);
            RegisterArchiveService(services);
            RegisterDWTransactionService(services);
            RegisterLogService(services);
            RegisterTelegramService(services);
            RegisterOtherServices(services);
            RegisterELKService(services);
            RegisterTransformMBTransaction(services);
            RegisterTransformMBDevice(services);
            RegisterLogstashService(services);
            RegisterDCSJobService(services);
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

        private static void RegisterTransformMBTransaction(this IServiceCollection services)
        {
            services.AddSingleton<IMBTransactionDataAccess, MBTransactionDataAccess>();
            services.AddSingleton<IMBTransactionServices, MBTransactionServices>();
            services.AddSingleton<IMBTransactionJobService, MBTransactionJobService>();
        }
        private static void RegisterTransformMBDevice(this IServiceCollection services)
        {
            services.AddSingleton<IMBDeviceDataAccess, MBDeviceDataAccess>();
            services.AddSingleton<IMBDeviceServices, MBDeviceServices>();
            services.AddSingleton<IMBDeviceJobService, MBDeviceJobService>();
        }

        private static void RegisterAbnormalTransaction(this IServiceCollection services)
        {
            services.AddSingleton<IAbnormalTransactionDataAccess, AbnormalTransactionDataAccess>();
            services.AddSingleton<IAbnormalTransactionServices, AbnormalTransactionServices>();
            services.AddSingleton<IAbnormalTransactionJobService, AbnormalTransactionJobService>();
        }

        private static void RegisterArchiveService(this IServiceCollection services)
        {
            services.AddSingleton<IArchiveDataDataAccess, ArchiveDataDataAccess>();
            services.AddSingleton<IArchiveDataService, ArchiveDataService>();
            services.AddSingleton<IArchiveJobServices, ArchiveJobServices>();
        }

        private static void RegisterDWTransactionService(this IServiceCollection services)
        {
            services.AddSingleton<IDataTraceJobService, DataTraceJobService>();
            services.AddSingleton<IDataTraceServices, DataTraceServices>();
            services.AddSingleton<IDataTraceDataAccess, DataTraceDataAccess>();
        }

        private static void RegisterLogService(this IServiceCollection services)
        {
            services.AddSingleton<ILogger, Logger>();
            services.AddSingleton<ILogService, LogService>();
        }

        private static void RegisterTelegramService(this IServiceCollection services)
        {
            services.AddSingleton<INotificationService, NotificationService>();
        }

        private static void RegisterOtherServices(this IServiceCollection services)
        {
            services.AddSingleton<ISystemSettingServices, SystemSettingServices>();
            services.AddSingleton<ISystemSettingDataAccess, SystemSettingDataAccess>();
           
        }

        private static void RegisterELKService(this IServiceCollection services)
        {           
            services.AddSingleton<IELKDataAccess, ELKDataAccess>();
            services.AddSingleton<IELKServices, ELKServices>();
            services.AddSingleton<IELKJobService, ELKJobService>();
        }

        private static void RegisterLogstashService(this IServiceCollection services)
        {
            services.AddSingleton<ILogstashService, LogstashService>();
        }

        private static void RegisterDCSJobService(this IServiceCollection services)
        {
            services.AddSingleton<DCSTransactionJobService>();
            services.AddSingleton<ICommonJobService, CommonJobService>();
            services.AddSingleton<ICommonServices, CommonServices>();
            services.AddSingleton<ICommonDataAccess, CommonDataAccess>();
        }
    }
}
