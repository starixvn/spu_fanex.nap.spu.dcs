namespace Fanex.NAP.SPU.DCS.Tests.JobService
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using AutoFixture;
    using Fanex.NAP.SPU.DCS.Features.Devices;
    using Fanex.NAP.SPU.DCS.Features.Devices.Entities;
    using Fanex.NAP.SPU.DCS.JobService;
    using Fanex.NAP.SPU.DCS.JobService.Services;
    using NSubstitute;
    using Sentry;
    using Xunit;

    public class DeviceJobServiceTests
    {
        private ISentryClient sentryClient;
        private IDeviceServices deviceService;
        private IAppSettings appSettings;

        private readonly IFixture fixture = new Fixture();


        [Fact]
        public async void TransformDevice_WhenHasStagingTransIds_ShouldCallExactFunction()
        {
            var stagingTransIds = fixture.Create<IEnumerable<StagingTransId>>();
            var identityDevices = fixture.Create<IdentityDevice>();
            var transactionRecover = fixture.Create<IEnumerable<TransactionRecover>>();
            var transactionEvolution = fixture.Create<IEnumerable<TransactionEvolution>>();

            var deviceRecovers = (transactionRecover, transactionEvolution);

            var deviceJobService = GetService();

            deviceService.GetStagingTransIdForDeviceAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<bool>()).Returns(stagingTransIds);
            deviceService.TransformDeviceInsertAsync(Arg.Any<IGrouping<int, StagingTransId>>(), Arg.Any<bool>()).Returns(identityDevices);
            deviceService.GetIdentityDeviceRecoverAsync(Arg.Any<string>()).Returns(deviceRecovers);

            await deviceJobService.TransformDevice(1, 1, false);
        }

        private DeviceJobService GetService()
        {
            sentryClient = Substitute.For<ISentryClient>();
            deviceService = Substitute.For<IDeviceServices>();
            appSettings = Substitute.For<IAppSettings>();
            return new DeviceJobService(sentryClient, deviceService, appSettings);
        }
    }
}
