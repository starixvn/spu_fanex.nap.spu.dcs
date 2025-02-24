namespace Fanex.NAP.SPU.DCS.Tests.Features.Devices
{
    using AutoFixture;
    using Fanex.NAP.SPU.DCS.Features.Devices;
    using Fanex.NAP.SPU.DCS.Features.Devices.Entities;
    using NSubstitute;
    using System.Collections.Generic;
    using System.Linq;
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
        public async Task GetFPDeviceEvolutionAsync_WhenBatchSizeIsGreaterThanZero_ReturnsExactData()
        {
            // Arrange
            var expectedDeviceEvolutions = (fixture.CreateMany<DeviceEvolutionEntity>(), fixture.CreateMany<DeviceEvolutionEntity>());

            deviceDataAcess.GetFPDeviceEvolutionAsync(Arg.Any<int>(), Arg.Any<bool>()).Returns(expectedDeviceEvolutions);

            // Act
            var actual = await deviceServices.GetFPDeviceEvolutionAsync(1, isExtra);

            // Assert
            Assert.Equal(expectedDeviceEvolutions, actual);
            await deviceDataAcess.Received(1).GetFPDeviceEvolutionAsync(1, isExtra);
        }

        [Fact]
        public async Task GetFPDeviceEvolutionAsync_WhenBatchSizeIsZero_ReturnsEmptyData()
        {
            // Arrange
            var expectedEmptyDeviceEvolutionList = new List<DeviceEvolutionEntity>();

            var expectedEmptyAdditionalDeviceEvolutionList = new List<DeviceEvolutionEntity>();

            // Act
            var (actualDeviceEvolutionList, actualAdditionalDeviceEvolutionList) = await deviceServices.GetFPDeviceEvolutionAsync(0, isExtra);

            // Assert
            Assert.Equal(expectedEmptyDeviceEvolutionList, actualDeviceEvolutionList);
            Assert.Equal(expectedEmptyAdditionalDeviceEvolutionList, actualAdditionalDeviceEvolutionList);

            await deviceDataAcess.DidNotReceive().GetFPDeviceEvolutionAsync(Arg.Any<int>(), Arg.Any<bool>());
        }

        [Fact]
        public async Task InsertFPDeviceEvolutionAsync_WhenCalled_ShouldCallExactFunction()
        {
            // Arrange
            string deviceEvolutionJson = "{\"id\":1,\"name\":\"Device 1\"}";
            bool isTransformExtra = true;

            // Act
            await deviceServices.InsertFPDeviceEvolutionAsync(deviceEvolutionJson, isTransformExtra);

            // Assert
            await deviceDataAcess.Received(1).InsertFPDeviceEvolutionAsync(deviceEvolutionJson, isTransformExtra);
        }

        [Fact]
        public async Task TransformDeviceInsertAsync_WhenCalled_ShouldCallExactFunction()
        {
            // Arrange
            var stagingTransIds = fixture.CreateMany<StagingTransId>();

            bool isReadyDbExtra = true;

            string transIDs = string.Join(",", stagingTransIds.Select(t => t.TransId.ToString()));

            // Act
            await deviceServices.TransformDeviceInsertAsync(stagingTransIds, isReadyDbExtra);

            // Assert
            await deviceDataAcess.Received(1).TransformDeviceInsertAsync(transIDs, isReadyDbExtra);
        }

        [Fact]
        public async Task InsertFPDeviceMappingAsync_WhenCalled_ShouldCallExactFunction()
        {
            // Arrange
            int batchSize = 10;
            bool isTransformExtra = true;

            // Act
            await deviceServices.InsertFPDeviceMappingAsync(batchSize, isTransformExtra);

            // Assert
            await deviceDataAcess.Received(1).InsertFPDeviceMappingAsync(batchSize, isTransformExtra);
        }

    }
}
