

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

            // Act
            var result = await archiveJobServices.Execute(batchSize);

            // Assert
            Assert.True(result);
            archiveDataService.Received(1).UpdateModifiedDate(batchSize);
            await logService.DidNotReceiveWithAnyArgs().ErrorAsync(Arg.Any<Exception>());
        }

        [Fact]
        public async Task Execute_ThrowException_ShouldReturnFalseAndLogService()
        {
            // Arrange
            var archiveJobServices = GetService();

            int batchSize = 10;

            archiveDataService.When(x => x.UpdateModifiedDate(batchSize))
                            .Do(x => { throw new Exception("Sql exception"); });

            // Act
            var result = await archiveJobServices.Execute(batchSize);

            // Assert
            Assert.False(result);
            archiveDataService.Received().UpdateModifiedDate(batchSize);
            await logService.Received().ErrorAsync(Arg.Any<Exception>());
        }

        [Fact]
        public async Task Execute_InvalidBatchSize_CapturesExceptionAndReturnsFalse()
        {
            // Arrange
            var archiveJobServices = GetService();

            int invalidBatchSize = 0;

            // Act
            var result = await archiveJobServices.Execute(invalidBatchSize);

            // Assert
            Assert.False(result);
            await logService.Received(1).ErrorAsync(Arg.Any<Exception>());
            archiveDataService.DidNotReceiveWithAnyArgs().UpdateModifiedDate(Arg.Any<int>());
        }

        [Fact]
        public async Task DeleteBotComponent_InvalidRetentionDays_CapturesExceptionAndReturnsFalse()
        {
            // Arrange
            var archiveJobServices = GetService();

            int invalidRetentionDays = -1;

            // Act
            var result = await archiveJobServices.DeleteBotComponent(invalidRetentionDays);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteBotComponent_ValidRetentionDays_ShouldReturnsTrue()
        {
            // Arrange
            var archiveJobServices = GetService();

            int retentionDays = 1;

            // Act
            var result = await archiveJobServices.DeleteBotComponent(retentionDays);

            // Assert
            Assert.True(result);
            archiveDataService.Received().DeleteBotComponent(retentionDays);
        }

        [Fact]
        public async Task DeleteBotComponent_ValidRetentionDaysAndThrowException_CapturesExceptionAndReturnsFalse()
        {
            // Arrange
            var archiveJobServices = GetService();

            int retentionDays = 1;

            archiveDataService.When(x => x.DeleteBotComponent(retentionDays))
                            .Do(x => { throw new Exception("Sql exception"); });

            // Act
            var result = await archiveJobServices.DeleteBotComponent(retentionDays);

            // Assert
            Assert.False(result);
            await logService.Received().ErrorAsync(Arg.Any<Exception>());
        }

        [Fact]
        public async Task UpdateMBLastAccessedDate_ValidBatchSize_ReturnsTrue()
        {
            // Arrange
            var archiveJobServices = GetService();
            int batchSize = 10;

            // Act
            var result = await archiveJobServices.UpdateMBLastAccessedDate(batchSize);

            // Assert
            Assert.True(result);
            archiveDataService.Received(1).UpdateMBLastAccessedDate(batchSize);
            await logService.DidNotReceiveWithAnyArgs().ErrorAsync(Arg.Any<Exception>());
        }

        [Fact]
        public async Task UpdateMBLastAccessedDate_ThrowException_ShouldReturnFalseAndLogService()
        {
            // Arrange
            var archiveJobServices = GetService();
            int batchSize = 10;

            archiveDataService.When(x => x.UpdateMBLastAccessedDate(batchSize))
                              .Do(x => { throw new Exception("Sql exception"); });

            // Act
            var result = await archiveJobServices.UpdateMBLastAccessedDate(batchSize);

            // Assert
            Assert.False(result);
            archiveDataService.Received().UpdateMBLastAccessedDate(batchSize);
            await logService.Received().ErrorAsync(Arg.Any<Exception>());
        }

        [Fact]
        public async Task UpdateMBLastAccessedDate_InvalidBatchSize_CapturesExceptionAndReturnsFalse()
        {
            // Arrange
            var archiveJobServices = GetService();
            int invalidBatchSize = 0;

            // Act
            var result = await archiveJobServices.UpdateMBLastAccessedDate(invalidBatchSize);

            // Assert
            Assert.False(result);
            await logService.Received(1).ErrorAsync(Arg.Any<Exception>());
            archiveDataService.DidNotReceiveWithAnyArgs().UpdateMBLastAccessedDate(Arg.Any<int>());
        }

        public ArchiveJobServices GetService()
        {

            logService = Substitute.For<ILogService>();
            archiveDataService = Substitute.For<IArchiveDataService>();
            return new ArchiveJobServices(archiveDataService, logService);
        }
    }
}
