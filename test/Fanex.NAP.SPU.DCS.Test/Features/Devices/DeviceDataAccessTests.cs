namespace Fanex.NAP.SPU.DCS.Tests.Features.Devices
{
    using AutoFixture;
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.DCS.Features.ArchiveData.Criteria;
    using Fanex.NAP.SPU.DCS.Features.Devices;
    using Fanex.NAP.SPU.DCS.Features.Devices.Criteria;
    using Fanex.NAP.SPU.DCS.Features.Devices.Entities;
    using NSubstitute;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    public class DeviceDataAccessTests
    {
        private IDynamicRepository repository;
        private IDeviceDataAccess deviceDataAccess;
        private IFixture fixture = new Fixture();

        public DeviceDataAccessTests()
        {
            repository = Substitute.For<IDynamicRepository>();
            deviceDataAccess = new DeviceDataAccess(repository);
        }

        [Theory]
        [InlineData("DCS_DC_Transform_Device_GetPackage")]
        public async Task GetStagingTransIdForDeviceAsync_WhenCall_ReturnExactData(string expectedSettingKey)
        {
            // Arrange
            var stagingTransIds = fixture.CreateMany<StagingTransId>();

            var criteria = fixture.Create<GetDeviceStagingTransactionPackageCriteria>();

            repository.FetchAsync<StagingTransId>(Arg.Do<GetDeviceStagingTransactionPackageCriteria>(x => criteria = x)).Returns(stagingTransIds);

            // Act
            var actual = await deviceDataAccess.GetStagingTransIdForDeviceAsync(1, 1);

            // Assert
            var settingKey = criteria.GetSettingKey();
            Assert.Equal(stagingTransIds, actual);
            Assert.Equal(expectedSettingKey, settingKey);
        }

        [Theory]
        [InlineData("DCS_DC_Transform_Device_Insert")]
        public async Task TransformDeviceInsertAsync_WhenCalled_ExecutesWithCorrectCriteria(string expectedSettingKey)
        {
            // Arrange
            var transIDs = "1,2,3";
            var criteria = fixture.Create<TransformDeviceInsertCriteria>();

            repository.ExecuteAsync(Arg.Do<TransformDeviceInsertCriteria>(x => criteria = x)).Returns(1);

            // Act
            await deviceDataAccess.TransformDeviceInsertAsync(transIDs);

            // Assert
            Assert.NotNull(criteria);
            Assert.Equal(transIDs, criteria.TransIDs);

            var settingKey = criteria.GetSettingKey();
            var isValid = criteria.IsInvalid();

            Assert.Equal(expectedSettingKey, settingKey);
        }
    }
}
