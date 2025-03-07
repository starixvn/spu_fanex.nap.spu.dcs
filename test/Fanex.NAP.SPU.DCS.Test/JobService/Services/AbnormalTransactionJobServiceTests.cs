namespace Fanex.NAP.SPU.DCS.Tests.JobService.Services
{
    using System;
    using System.Threading.Tasks;
    using Fanex.NAP.SPU.DCS.Features.AbnormalTransaction;
    using Fanex.NAP.SPU.DCS.JobService;
    using Fanex.NAP.SPU.DCS.JobService.Services;
    using NSubstitute;
    using NSubstitute.ExceptionExtensions;
    using Xunit;

    public class AbnormalTransactionJobServiceTests
    {
        private ILogService logService;
        private IAbnormalTransactionServices abnormalTransactionServices;
        private IAppSettings appSettings;

        [Fact]
        public async Task PushAbnormalTransaction_SizeGreaterThanZero_ReturnsResultFromServices()
        {
            // Arrange
            var service = GetService();
            int size = 5;

            abnormalTransactionServices.PushAbnormalTransaction(size, Arg.Any<string>(), Arg.Any<bool>()).Returns(true);

            // Act
            var result = await service.PushAbnormalTransaction(size);

            // Assert
            Assert.True(result);
            await abnormalTransactionServices.Received(1).PushAbnormalTransaction(size, Arg.Any<string>(), Arg.Any<bool>());
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
            await logService.Received().InfoMessageAsync(Arg.Any<string>());
        }

        [Fact]
        public async Task PushAbnormalTransaction_ExceptionThrown_CapturesExceptionAndReturnsFalse()
        {
            // Arrange
            var service = GetService();
            int size = 5;
            var exception = new Exception("Simulated exception");

            abnormalTransactionServices.PushAbnormalTransaction(size, Arg.Any<string>(), Arg.Any<bool>()).Throws(exception);

            // Act
            var result = await service.PushAbnormalTransaction(size);

            // Assert
            Assert.False(result);
            await logService.Received(1).ErrorAsync(exception);
        }

        private AbnormalTransactionJobService GetService()
        {
            logService = Substitute.For<ILogService>();
            appSettings = Substitute.For<IAppSettings>();
            abnormalTransactionServices = Substitute.For<IAbnormalTransactionServices>();
            return new AbnormalTransactionJobService(logService, appSettings, abnormalTransactionServices);
        }
    }
}
