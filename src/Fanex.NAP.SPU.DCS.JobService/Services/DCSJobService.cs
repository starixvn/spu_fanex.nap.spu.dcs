using Fanex.NAP.SPU.DCS.Features.SystemSetting;

namespace Fanex.NAP.SPU.DCS.JobService.Services
{
    public class DCSJobService
    {
        public IUserAgentJobService UserAgentJobService { get; }
        public IDeviceJobService DeviceJobService { get; }
        public ITransactionJobService TransactionJobService { get; }
        public ISumTransactionJobService SumTransactionJobService { get; }
        public IAbnormalTransactionJobService AbnormalTransactionJobService { get; }
        public IArchiveJobServices ArchiveJobServices { get; }
        public ISystemSettingServices SystemSettingServices { get; }
        public IMBTransactionJobService MBTransactionJobService { get; }
        public IMBDeviceJobService MBDeviceJobService { get; }

        public DCSJobService(
            IUserAgentJobService userAgentJobService,
            IDeviceJobService deviceJobService,
            ITransactionJobService transactionJobService,
            ISumTransactionJobService sumTransactionJobService,
            IAbnormalTransactionJobService abnormalTransactionJobService,
            IArchiveJobServices archiveJobServices,
            ISystemSettingServices systemSettingServices,
            IMBTransactionJobService mbTransactionJobService,
            IMBDeviceJobService mbDeviceJobService)
        {
            UserAgentJobService = userAgentJobService;
            DeviceJobService = deviceJobService;
            TransactionJobService = transactionJobService;
            SumTransactionJobService = sumTransactionJobService;
            AbnormalTransactionJobService = abnormalTransactionJobService;
            ArchiveJobServices = archiveJobServices;
            SystemSettingServices = systemSettingServices;
            MBTransactionJobService = mbTransactionJobService;
            MBDeviceJobService = mbDeviceJobService;
        }
    }
}
