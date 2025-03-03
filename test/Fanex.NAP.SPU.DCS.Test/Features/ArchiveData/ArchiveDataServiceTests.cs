
namespace Fanex.NAP.SPU.DCS.Tests.Features.ArchiveData
{
    using Fanex.NAP.SPU.DCS.Features.ArchiveData;
    using NSubstitute;
    using Xunit;

    public class ArchiveDataServiceTests
    {
        private readonly IArchiveDataDataAccess archiveDataAccess;
        private readonly IArchiveDataService archiveDataService;

        public ArchiveDataServiceTests()
        {
            archiveDataAccess = Substitute.For<IArchiveDataDataAccess>();
            archiveDataService = new ArchiveDataService(archiveDataAccess);
        }

        [Fact]
        public void UpdateModifiedDate_WhenCalled_ExecutesDataAccessMethod()
        {
            // Arrange
            int batchSize = 10;
            bool isTransformExtra = true;

            // Act
            archiveDataService.UpdateModifiedDate(batchSize, isTransformExtra);

            // Assert
            archiveDataAccess.Received(1).UpdateModifiedDate(batchSize, isTransformExtra);
        }

        [Fact]
        public void UpdateMBLastAccessedDate_WhenCalled_ExecutesDataAccessMethod()
        {
            // Arrange
            int batchSize = 10;

            // Act
            archiveDataService.UpdateMBLastAccessedDate(batchSize);

            // Assert
            archiveDataAccess.Received(1).UpdateMBLastAccessedDate(batchSize);
        }

        [Fact]
        public void DeleteBotComponent_WhenCalled_ExecutesDataAccessMethod()
        {
            // Arrange
            int retentionDays = 30;
            bool isTransformExtra = true;

            // Act
            archiveDataService.DeleteBotComponent(retentionDays, isTransformExtra);

            // Assert
            archiveDataAccess.Received(1).DeleteBotComponent(retentionDays, isTransformExtra);
        }
    }

}
