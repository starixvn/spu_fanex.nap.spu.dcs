

namespace Fanex.NAP.SPU.DCS.Tests.Features.ArchiveData
{
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.DCS.Features.ArchiveData.Criteria;
    using Fanex.NAP.SPU.DCS.Features.ArchiveData;
    using NSubstitute;
    using Xunit;
    using AutoFixture;

    public class ArchiveDataDataAccessTests
    {
        private readonly IDynamicRepository repository;
        private readonly IArchiveDataDataAccess archiveDataDataAccess;
        private IFixture fixture = new Fixture();

        public ArchiveDataDataAccessTests()
        {
            repository = Substitute.For<IDynamicRepository>();
            archiveDataDataAccess = new ArchiveDataDataAccess(repository);
        }

        [Theory]
        [InlineData(true, "DCS_ET_ArchiveData_ModifiedDate_Update")]
        [InlineData(false, "DCS_DC_ArchiveData_ModifiedDate_Update")]
        public void UpdateModifiedDate_WhenCalled_ExecutesWithCorrectCriteria(bool isTransformExtra, string expectedSettingKey)
        {
            // Arrange
            int batchSize = 10;
            var criteria = fixture.Create<UpdateModifiedDateCriteria>();

            repository.Execute(Arg.Do<UpdateModifiedDateCriteria>(x => criteria = x));

            // Act
            archiveDataDataAccess.UpdateModifiedDate(batchSize, isTransformExtra);

            // Assert
            var settingKey = criteria.GetSettingKey();
            var isValid = criteria.IsInvalid();

            repository.Received(1).Execute(Arg.Is<UpdateModifiedDateCriteria>(x =>
                x.BatchSize == batchSize && x.IsReadyDbExtra == isTransformExtra));
            Assert.Equal(expectedSettingKey, settingKey);
        }

        [Fact]
        public void UpdateMBLastAccessedDate_WhenCall_ReturnExactData()
        {
            // Arrange
            int batchSize = 10;

            var criteria = fixture.Create<UpdateMBLastAccessedDateCriteria>();

            var expectedSettingKey = "DCS_DC_ArchiveData_MBLastAccessedDate_Update";

            repository.Execute(Arg.Do<UpdateMBLastAccessedDateCriteria>(x => criteria = x));

            // Act
            archiveDataDataAccess.UpdateMBLastAccessedDate(batchSize);

            // Assert
            var settingKey = criteria.GetSettingKey();
            var isValid = criteria.IsInvalid();

            repository.Received(1).Execute(Arg.Is<UpdateMBLastAccessedDateCriteria>(x =>
                x.BatchSize == batchSize));

            Assert.Equal(expectedSettingKey, settingKey);
        }

        [Theory]
        [InlineData(true, "DCS_ET_ArchiveData_BotComponent_Delete")]
        [InlineData(false, "DCS_DC_ArchiveData_BotComponent_Delete")]
        public void DeleteBotComponent_WhenCall_ReturnExactData(bool isTransformExtra, string expectedSettingKey)
        {
            // Arrange
            int retentionDays = 30;
            var criteria = fixture.Create<DeleteBotComponentCriteria>();

            repository.Execute(Arg.Do<DeleteBotComponentCriteria>(x => criteria = x));

            // Act
            archiveDataDataAccess.DeleteBotComponent(retentionDays, isTransformExtra);

            // Assert
            var settingKey = criteria.GetSettingKey();
            var isValid = criteria.IsInvalid();

            repository.Received(1).Execute(Arg.Is<DeleteBotComponentCriteria>(x =>
                x.RetentionDays == retentionDays && x.IsReadyDbExtra == isTransformExtra));

            Assert.Equal(expectedSettingKey, settingKey);
        }
    }
}
