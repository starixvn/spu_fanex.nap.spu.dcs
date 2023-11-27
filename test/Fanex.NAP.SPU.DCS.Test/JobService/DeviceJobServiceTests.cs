namespace Fanex.NAP.SPU.DCS.Tests.JobService
{
    using Fanex.NAP.SPU.DCS.Features.Devices;
    using Fanex.NAP.SPU.DCS.JobService;
    using Sentry;

    public class DeviceJobServiceTests
    {
        private readonly ISentryClient sentryClient;
        private readonly IDeviceServices deviceService;
        private readonly IAppSettings appSettings;
    }
}
