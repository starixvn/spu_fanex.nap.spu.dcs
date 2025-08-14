namespace Fanex.NAP.SPU.DCS.Tests.JobService.Services
{
    using System;
    using System.Threading.Tasks;
    using AutoFixture;
    using Fanex.NAP.SPU.DCS.Features.AbnormalTransaction;
    using Fanex.NAP.SPU.DCS.JobService;
    using Fanex.NAP.SPU.DCS.JobService.Services;
    using NSubstitute;
    using NSubstitute.ExceptionExtensions;
    using Xunit;

    public class AbnormalTransactionJobServiceTests
    {
        private readonly ILogService logService;
        private readonly IAbnormalTransactionServices abnormalTransactionServices;
        private readonly IAppSettings appSettings;
        private readonly IFixture fixture = new Fixture();

        public AbnormalTransactionJobServiceTests()
        {
            logService = Substitute.For<ILogService>();
            appSettings = Substitute.For<IAppSettings>();
            abnormalTransactionServices = Substitute.For<IAbnormalTransactionServices>();
        }

        [Fact]
        public async Task PushAbnormalTransaction_SizeGreaterThanZero_ReturnsResultFromServices()
        {
            // Arrange
            int size = 5;
            var url = fixture.Create<string>();
            var ignorePushData = fixture.Create<bool>();

            appSettings.AbnormalTransactionUrl.Returns(url);
            appSettings.AbnormalTransactionIgnorePushData.Returns(ignorePushData);

            var service = new AbnormalTransactionJobService(logService, appSettings, abnormalTransactionServices);

            abnormalTransactionServices.PushAbnormalTransaction(size, url, ignorePushData).Returns(true);

            // Act
            var result = await service.PushAbnormalTransaction(size);

            // Assert
            Assert.True(result);
            await abnormalTransactionServices.Received(1).PushAbnormalTransaction(size, url, ignorePushData);
        }

        [Fact]
        public async Task PushAbnormalTransaction_SizeZero_CapturesMessageAndReturnsFalse()
        {
            // Arrange
            var service = new AbnormalTransactionJobService(logService, appSettings, abnormalTransactionServices);

            // Act
            var result = await service.PushAbnormalTransaction(0);

            // Assert
            Assert.False(result);
            await logService.Received().InfoMessageAsync("PushAbnormalTransaction size = 0");
            await abnormalTransactionServices.DidNotReceive().PushAbnormalTransaction(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<bool>());
        }

        [Fact]
        public async Task PushAbnormalTransaction_ExceptionThrown_CapturesExceptionAndReturnsFalse()
        {
            // Arrange
            int size = 5;
            var exception = new Exception("Simulated exception");

            var service = new AbnormalTransactionJobService(logService, appSettings, abnormalTransactionServices);

            abnormalTransactionServices.PushAbnormalTransaction(size, Arg.Any<string>(), Arg.Any<bool>()).Throws(exception);

            // Act
            var result = await service.PushAbnormalTransaction(size);

            // Assert
            Assert.False(result);
            await logService.Received(1).ErrorAsync(exception);
        }

        [Fact]
        public async Task PushAbnormalTransaction_NegativeSize_CapturesMessageAndReturnsFalse()
        {
            // Arrange
            var service = new AbnormalTransactionJobService(logService, appSettings, abnormalTransactionServices);
            
            int size = -1;

            // Act
            var result = await service.PushAbnormalTransaction(size);

            // Assert
            Assert.False(result);
            await logService.Received().InfoMessageAsync("PushAbnormalTransaction size = 0");
            await abnormalTransactionServices.DidNotReceive().PushAbnormalTransaction(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<bool>());
        }

        [Fact]
        public async Task PushAbnormalTransaction_LargeSize_PassesToServices()
        {
            // Arrange
            int size = int.MaxValue;
            var url = fixture.Create<string>();
            var ignorePushData = fixture.Create<bool>();

            appSettings.AbnormalTransactionUrl.Returns(url);
            appSettings.AbnormalTransactionIgnorePushData.Returns(ignorePushData);

            var service = new AbnormalTransactionJobService(logService, appSettings, abnormalTransactionServices);

            abnormalTransactionServices.PushAbnormalTransaction(size, url, ignorePushData).Returns(true);

            // Act
            var result = await service.PushAbnormalTransaction(size);

            // Assert
            Assert.True(result);
            await abnormalTransactionServices.Received(1).PushAbnormalTransaction(size, url, ignorePushData);
        }

        [Fact]
        public async Task PushAbnormalTransaction_ServicesReturnFalse_ReturnsFalse()
        {
            // Arrange
            int size = 5;
            var url = fixture.Create<string>();
            var ignorePushData = fixture.Create<bool>();

            appSettings.AbnormalTransactionUrl.Returns(url);
            appSettings.AbnormalTransactionIgnorePushData.Returns(ignorePushData);

            var service = new AbnormalTransactionJobService(logService, appSettings, abnormalTransactionServices);

            abnormalTransactionServices.PushAbnormalTransaction(size, url, ignorePushData).Returns(false);

            // Act
            var result = await service.PushAbnormalTransaction(size);

            // Assert
            Assert.False(result);
            await abnormalTransactionServices.Received(1).PushAbnormalTransaction(size, url, ignorePushData);
        }

        [Fact]
        public async Task PushAbnormalTransaction_WithNullUrl_PassesNullToServices()
        {
            // Arrange
            int size = 5;
            string url = null;
            var ignorePushData = fixture.Create<bool>();

            appSettings.AbnormalTransactionUrl.Returns(url);
            appSettings.AbnormalTransactionIgnorePushData.Returns(ignorePushData);

            var service = new AbnormalTransactionJobService(logService, appSettings, abnormalTransactionServices);

            abnormalTransactionServices.PushAbnormalTransaction(size, url, ignorePushData).Returns(true);

            // Act
            var result = await service.PushAbnormalTransaction(size);

            // Assert
            Assert.True(result);
            await abnormalTransactionServices.Received(1).PushAbnormalTransaction(size, url, ignorePushData);
        }

        [Fact]
        public async Task PushAbnormalTransaction_WithEmptyUrl_PassesEmptyToServices()
        {
            // Arrange
            int size = 5;
            string url = string.Empty;
            var ignorePushData = fixture.Create<bool>();

            appSettings.AbnormalTransactionUrl.Returns(url);
            appSettings.AbnormalTransactionIgnorePushData.Returns(ignorePushData);

            var service = new AbnormalTransactionJobService(logService, appSettings, abnormalTransactionServices);

            abnormalTransactionServices.PushAbnormalTransaction(size, url, ignorePushData).Returns(true);

            // Act
            var result = await service.PushAbnormalTransaction(size);

            // Assert
            Assert.True(result);
            await abnormalTransactionServices.Received(1).PushAbnormalTransaction(size, url, ignorePushData);
        }

        [Fact]
        public async Task PushAbnormalTransaction_WithIgnorePushDataTrue_PassesToServices()
        {
            // Arrange
            int size = 5;
            var url = fixture.Create<string>();
            bool ignorePushData = true;

            appSettings.AbnormalTransactionUrl.Returns(url);
            appSettings.AbnormalTransactionIgnorePushData.Returns(ignorePushData);

            var service = new AbnormalTransactionJobService(logService, appSettings, abnormalTransactionServices);

            abnormalTransactionServices.PushAbnormalTransaction(size, url, ignorePushData).Returns(true);

            // Act
            var result = await service.PushAbnormalTransaction(size);

            // Assert
            Assert.True(result);
            await abnormalTransactionServices.Received(1).PushAbnormalTransaction(size, url, true);
        }

        [Fact]
        public async Task PushAbnormalTransaction_WithIgnorePushDataFalse_PassesToServices()
        {
            // Arrange
            int size = 5;
            var url = fixture.Create<string>();
            bool ignorePushData = false;

            appSettings.AbnormalTransactionUrl.Returns(url);
            appSettings.AbnormalTransactionIgnorePushData.Returns(ignorePushData);

            var service = new AbnormalTransactionJobService(logService, appSettings, abnormalTransactionServices);

            abnormalTransactionServices.PushAbnormalTransaction(size, url, ignorePushData).Returns(true);

            // Act
            var result = await service.PushAbnormalTransaction(size);

            // Assert
            Assert.True(result);
            await abnormalTransactionServices.Received(1).PushAbnormalTransaction(size, url, false);
        }

        [Fact]
        public async Task PushAbnormalTransaction_MultipleCallsWithDifferentSizes_HandlesCorrectly()
        {
            // Arrange
            var url = fixture.Create<string>();
            var ignorePushData = fixture.Create<bool>();

            appSettings.AbnormalTransactionUrl.Returns(url);
            appSettings.AbnormalTransactionIgnorePushData.Returns(ignorePushData);

            var service = new AbnormalTransactionJobService(logService, appSettings, abnormalTransactionServices);

            abnormalTransactionServices.PushAbnormalTransaction(10, url, ignorePushData).Returns(true);
            abnormalTransactionServices.PushAbnormalTransaction(20, url, ignorePushData).Returns(false);

            // Act
            var result1 = await service.PushAbnormalTransaction(10);
            var result2 = await service.PushAbnormalTransaction(20);

            // Assert
            Assert.True(result1);
            Assert.False(result2);
            await abnormalTransactionServices.Received(1).PushAbnormalTransaction(10, url, ignorePushData);
            await abnormalTransactionServices.Received(1).PushAbnormalTransaction(20, url, ignorePushData);
        }

        [Fact]
        public async Task PushAbnormalTransaction_WhenAppSettingsThrowsException_PropagatesException()
        {
            // Arrange
            int size = 5;

            appSettings.AbnormalTransactionUrl.Returns(x => { throw new Exception("Settings error"); });

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () => 
            {
                var service = new AbnormalTransactionJobService(logService, appSettings, abnormalTransactionServices);
                await service.PushAbnormalTransaction(size);
            });
        }

        [Fact]
        public async Task PushAbnormalTransaction_BoundaryValueSize_HandlesCorrectly()
        {
            // Arrange
            int size = 1; // Minimum valid size
            var url = fixture.Create<string>();
            var ignorePushData = fixture.Create<bool>();

            appSettings.AbnormalTransactionUrl.Returns(url);
            appSettings.AbnormalTransactionIgnorePushData.Returns(ignorePushData);

            var service = new AbnormalTransactionJobService(logService, appSettings, abnormalTransactionServices);

            abnormalTransactionServices.PushAbnormalTransaction(size, url, ignorePushData).Returns(true);

            // Act
            var result = await service.PushAbnormalTransaction(size);

            // Assert
            Assert.True(result);
            await abnormalTransactionServices.Received(1).PushAbnormalTransaction(size, url, ignorePushData);
        }

        [Fact]
        public void Constructor_WithValidParameters_CreatesInstance()
        {
            // Act
            var service = new AbnormalTransactionJobService(logService, appSettings, abnormalTransactionServices);

            // Assert
            Assert.NotNull(service);
        }

        [Fact]
        public void Constructor_WithNullLogService_ShouldThrowOrHandle()
        {
            // Act & Assert
            var exception = Record.Exception(() => new AbnormalTransactionJobService(null, appSettings, abnormalTransactionServices));
            
            // Constructor now has null checks and will throw ArgumentNullException
            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact]
        public void Constructor_WithNullAppSettings_ShouldThrowOrHandle()
        {
            // Act & Assert
            var exception = Record.Exception(() => new AbnormalTransactionJobService(logService, null, abnormalTransactionServices));
            
            // Constructor now has null checks and will throw ArgumentNullException
            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact]
        public void Constructor_WithNullAbnormalTransactionServices_ShouldThrowOrHandle()
        {
            // Act & Assert
            var exception = Record.Exception(() => new AbnormalTransactionJobService(logService, appSettings, null));
            
            // Constructor now has null checks and will throw ArgumentNullException
            Assert.IsType<ArgumentNullException>(exception);
        }
    }
}
