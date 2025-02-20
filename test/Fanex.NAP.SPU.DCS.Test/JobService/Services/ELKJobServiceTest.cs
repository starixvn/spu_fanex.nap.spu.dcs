
namespace Fanex.NAP.SPU.DCS.Tests.JobService.Services
{
    using System;
    using System.Threading.Tasks;
    using Fanex.NAP.SPU.DCS.Features.ELK;
    using Fanex.NAP.SPU.DCS.JobService.Services;
    using Fanex.NAP.SPU.DCS.JobService;
    using NSubstitute;
    using Xunit;

    public class ELKJobServiceTest
    {
        private readonly IELKServices elkService;
        private readonly IAppSettings appSettings;
        private readonly ILogService logService;
        private readonly ELKJobService elkJobService;

        public ELKJobServiceTest()
        {
            elkService = Substitute.For<IELKServices>();
            appSettings = Substitute.For<IAppSettings>();
            logService = Substitute.For<ILogService>();
            elkJobService = new ELKJobService(elkService, appSettings, logService);
        }

        [Fact]
        public async Task Execute_WithValidDates_ReturnsTrue()
        {
            // Arrange
            var startTime = DateTime.Now.AddDays(-10);
            var endTime = DateTime.Now.AddDays(-2);

            // Act
            var result = await elkJobService.Execute(startTime, endTime);

            // Assert
            Assert.True(result);
            await logService.DidNotReceiveWithAnyArgs().ErrorAsync(Arg.Any<Exception>());
        }

        [Fact]
        public async Task Execute_WithException_ReturnsFalseAndLogsError()
        {
            // Arrange
            var startTime = DateTime.Now.AddDays(-1);
            var endTime = DateTime.Now;

            elkService.When(x => x.PrepareStagingForBetMessage(Arg.Any<DateTime>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<int>()))
                      .Do(x => { throw new Exception("Test exception"); });

            // Act
            var result = await elkJobService.Execute(startTime, endTime);

            // Assert
            Assert.False(result);
            await logService.Received(1).ErrorAsync(Arg.Any<Exception>());
        }

        [Fact]
        public async Task Execute_WithValidDatesAndBetDate_ReturnsTrue()
        {
            // Arrange
            var startTime = DateTime.Now.AddDays(-1);
            var endTime = DateTime.Now;
            var betDate = DateTime.Now.AddDays(-2);

            // Act
            var result = await elkJobService.Execute(startTime, endTime, betDate);

            // Assert
            Assert.True(result);
            await logService.DidNotReceiveWithAnyArgs().ErrorAsync(Arg.Any<Exception>());
        }

        [Fact]
        public async Task Execute_WithExceptionAndBetDate_ReturnsFalseAndLogsError()
        {
            // Arrange
            var startTime = DateTime.Now.AddDays(-1);
            var endTime = DateTime.Now;
            var betDate = DateTime.Now.AddDays(-2);

            elkService.When(x => x.PrepareStagingForBetMessageDaily(Arg.Any<DateTime>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<int>(), Arg.Any<string>(), Arg.Any<int>()))
                      .Do(x => { throw new Exception("Test exception"); });

            // Act
            var result = await elkJobService.Execute(startTime, endTime, betDate);

            // Assert
            Assert.False(result);
            await logService.Received(1).ErrorAsync(Arg.Any<Exception>());
        }
    }
}
