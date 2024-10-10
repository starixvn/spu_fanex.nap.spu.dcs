namespace Fanex.NAP.SPU.DCS.Tests.Features.Devices
{
    using AutoFixture;
    using Fanex.NAP.SPU.DCS.Features.Devices;
    using Fanex.NAP.SPU.DCS.Features.Devices.Entities;
    using NSubstitute;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Xunit;

    public class DeviceServicesTests
    {
        private IDeviceDataAccess deviceDataAcess;
        private IDeviceServices deviceServices;
        private IFixture fixture = new Fixture();
        private bool isExtra = false;

        public DeviceServicesTests()
        {
            deviceDataAcess = Substitute.For<IDeviceDataAccess>();

            deviceServices = new DeviceServices(deviceDataAcess);
        }

        [Fact]
        public async Task GetStagingTransIdForDeviceAsync_WhenCall_ReturnExactData()
        {

            // Arrange
            var stagingTransIds = fixture.CreateMany<StagingTransId>();
            deviceDataAcess.GetStagingTransIdForDeviceAsync(Arg.Any<int>(), Arg.Any<int>(), isExtra).Returns(stagingTransIds);

            // Act
            var actual = await deviceServices.GetStagingTransIdForDeviceAsync(1, 1, isExtra);

            // Assert
            Assert.Equal(stagingTransIds, actual);
        }

        [Fact]
        public async Task GetStagingTransIdForDeviceAsync_WhenInvalidParams_ReturnEmptyData()
        {
            // Arrange
            var expected = new List<StagingTransId>();

            // Act
            var actual = await deviceServices.GetStagingTransIdForDeviceAsync(0, 0, isExtra);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task TransformDeviceInsertAsync_WhenCall_ReturnExactData()
        {
            //// Arrange
            //var stagingTransIds = fixture.CreateMany<StagingTransId>();
            //var expected = fixture.CreateMany<IdentityDevice>();
            //deviceDataAcess.TransformDeviceInsertAsync(Arg.Any<string>(), isExtra).Returns(expected);

            //// Act
            //var actual = await deviceServices.TransformDeviceInsertAsync(stagingTransIds, isExtra);

            //// Assert
            //Assert.Equal(expected, actual);
        }

    }
}
