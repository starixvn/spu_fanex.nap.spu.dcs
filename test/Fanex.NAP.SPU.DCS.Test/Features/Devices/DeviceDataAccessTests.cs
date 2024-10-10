namespace Fanex.NAP.SPU.DCS.Tests.Features.Devices
{
    using AutoFixture;
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.DCS.Features.Devices;
    using Fanex.NAP.SPU.DCS.Features.Devices.Entities;
    using NSubstitute;
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
        [InlineData(true, "DCS_ET_Transform_Device_GetPackage")]
        [InlineData(false, "DCS_DC_Transform_Device_GetPackage")]
        public async Task GetStagingTransIdForDeviceAsync_WhenCall_ReturnExactData(bool isExtra, string expectedSettingKey)
        {
            // Arrange
            var stagingTransIds = fixture.CreateMany<StagingTransId>();
            var criteria = fixture.Create<GetDeviceStagingTransactionPackageCriteria>();

            repository.FetchAsync<StagingTransId>(Arg.Do<GetDeviceStagingTransactionPackageCriteria>(x => criteria = x)).Returns(stagingTransIds);

            // Act
            var actual = await deviceDataAccess.GetStagingTransIdForDeviceAsync(1, 1, isExtra);

            // Assert
            var settingKey = criteria.GetSettingKey();
            Assert.Equal(stagingTransIds, actual);
            Assert.Equal(expectedSettingKey, settingKey);
        }
    }
}
