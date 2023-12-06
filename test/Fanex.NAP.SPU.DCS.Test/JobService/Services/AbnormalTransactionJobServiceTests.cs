namespace Fanex.NAP.SPU.DCS.Tests.JobService.Services
{
    using Fanex.NAP.SPU.DCS.Features.AbnormalTransaction;
    using Fanex.NAP.SPU.DCS.JobService.Services;
    using Fanex.NAP.SPU.DCS.JobService;
    using NSubstitute;
    using Sentry;
    using System.Threading.Tasks;
    using System;
    using Xunit;
    using NSubstitute.ExceptionExtensions;

    public class AbnormalTransactionJobServiceTests
    {
        private ISentryClient sentryClient;
        private IAbnormalTransactionServices abnormalTransactionServices;
        private IAppSettings appSettings;

        [Fact]
        public async Task PushAbnormalTransaction_SizeGreaterThanZero_ReturnsResultFromServices()
        {
            // Arrange
            var service = GetService();

            int size = 5;

            abnormalTransactionServices.PushAbnormalTransaction(size, Arg.Any<string>()).Returns(true);

            // Act
            var result = await service.PushAbnormalTransaction(size);

            // Assert
            Assert.True(result);
            await abnormalTransactionServices.Received(1).PushAbnormalTransaction(size, Arg.Any<string>());
        }

        [Fact]
        public async Task PushAbnormalTransaction_SizeZero_CapturesMessageAndReturnsFalse()
        {
            // Arrange
            var service = GetService();

            // Act
            var result = await service.PushAbnormalTransaction(0);

            // Assert
            Assert.False(result);
            sentryClient.Received(1).CaptureMessage("PushAbnormalTransaction size = 0");
        }

        [Fact]
        public async Task PushAbnormalTransaction_ExceptionThrown_CapturesExceptionAndReturnsFalse()
        {
            // Arrange
            var service = GetService();

            int size = 5;

            abnormalTransactionServices.PushAbnormalTransaction(size, Arg.Any<string>()).Throws(new Exception("Simulated exception"));

            // Act
            var result = await service.PushAbnormalTransaction(size);

            // Assert
            Assert.False(result);
            sentryClient.Received(1).CaptureException(new Exception("Simulated exception"));
        }

        private AbnormalTransactionJobService GetService()
        {
            sentryClient = Substitute.For<ISentryClient>();
            appSettings = Substitute.For<IAppSettings>();
            abnormalTransactionServices = Substitute.For<IAbnormalTransactionServices>();
            return new AbnormalTransactionJobService(sentryClient, appSettings, abnormalTransactionServices);
        }
    }
}
