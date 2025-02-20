
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
    }
}
