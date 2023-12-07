

namespace Fanex.NAP.SPU.DCS.Tests.JobService.Services
{
    using System;
    using System.Threading.Tasks;
    using Fanex.NAP.SPU.DCS.Features.ArchiveData;
    using Fanex.NAP.SPU.DCS.JobService.Services;
    using NSubstitute;
    using Xunit;

    public class ArchiveJobServicesTests
    {
        private IArchiveDataService archiveDataService;
        private ILogService logService;

        [Fact]
        public async Task Execute_ValidBatchSize_ReturnsTrue()
        {
            // Arrange
            var archiveJobServices = GetService();

            int batchSize = 10;
            bool isTransformExtra = true;

            // Act
            var result = await archiveJobServices.Execute(batchSize, isTransformExtra);

            // Assert
            Assert.True(result);
            archiveDataService.Received(1).UpdateModifiedDate(batchSize, isTransformExtra);
            await logService.DidNotReceiveWithAnyArgs().ErrorAsync(Arg.Any<Exception>());
        }

        [Fact]
        public async Task Execute_InvalidBatchSize_CapturesExceptionAndReturnsFalse()
        {
            // Arrange
            var archiveJobServices = GetService();

            int invalidBatchSize = 0;
            bool isTransformExtra = true;

            // Act
            var result = await archiveJobServices.Execute(invalidBatchSize, isTransformExtra);

            // Assert
            Assert.False(result);
            await logService.Received(1).ErrorAsync(Arg.Any<Exception>());
            archiveDataService.DidNotReceiveWithAnyArgs().UpdateModifiedDate(Arg.Any<int>(), Arg.Any<bool>());
        }

        [Fact]
        public async Task DeleteBotComponent_InvalidRetentionDays_CapturesExceptionAndReturnsFalse()
        {
            // Arrange
            var archiveJobServices = GetService();

            int invalidRetentionDays = -1;
            bool isTransformExtra = true;

            // Act
            var result = await archiveJobServices.DeleteBotComponent(invalidRetentionDays, isTransformExtra);

            // Assert
            Assert.False(result);
        }

        public ArchiveJobServices GetService()
        {

            logService = Substitute.For<ILogService>();
            archiveDataService = Substitute.For<IArchiveDataService>();
            return new ArchiveJobServices(archiveDataService, logService);
        }
    }
}
