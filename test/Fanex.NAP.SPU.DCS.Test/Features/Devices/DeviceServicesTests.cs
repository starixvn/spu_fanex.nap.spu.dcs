namespace Fanex.NAP.SPU.DCS.Tests.Features.Devices
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoFixture;
    using Fanex.NAP.SPU.DCS.Features.Devices;
    using Fanex.NAP.SPU.DCS.Features.Devices.Entities;
    using FluentAssertions;
    using NSubstitute;
    using Xunit;

    public class DeviceServicesTests
    {
        private IDeviceDataAccess deviceDataAcess;
        private IDeviceServices deviceServices;
        private IFixture fixture = new Fixture();

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
            deviceDataAcess.GetStagingTransIdForDeviceAsync(Arg.Any<int>(), Arg.Any<int>()).Returns(stagingTransIds);

            // Act
            var actual = await deviceServices.GetStagingTransIdForDeviceAsync(1, 1);

            // Assert
            Assert.Equal(stagingTransIds, actual);
        }

        [Fact]
        public async Task GetStagingTransIdForDeviceAsync_WhenInvalidParams_ReturnEmptyData()
        {
            // Arrange
            var expected = new List<StagingTransId>();

            // Act
            var actual = await deviceServices.GetStagingTransIdForDeviceAsync(0, 0);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task TransformDeviceInsertAsync_WhenCall_ReturnExactData()
        {
            // Arrange
            var stagingTransIds = fixture.CreateMany<StagingTransId>();
            var expected = fixture.CreateMany<IdentityDevice>();
            deviceDataAcess.TransformDeviceInsertAsync(Arg.Any<string>()).Returns(expected);

            // Act
            var actual = await deviceServices.TransformDeviceInsertAsync(stagingTransIds);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task GetIdentityDeviceRecoverAsync_WhenCall_ReturnExactData()
        {
            // Arrange
            var transactionRecovers = fixture.CreateMany<TransactionRecover>();
            var transactionEvolutions = fixture.CreateMany<TransactionEvolution>();

            var expected = (transactionRecovers, transactionEvolutions);

            deviceDataAcess.GetIdentityDeviceRecoverAsync(Arg.Any<string>()).Returns(expected);

            // Act
            var actual = await deviceServices.GetIdentityDeviceRecoverAsync("transIds");

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task GetIdentityDeviceRecoverAsync_WhenInvalidParams_ReturnEmptyData()
        {
            var expected = (new List<TransactionRecover>(), new List<TransactionEvolution>());

            // Act
            var actual = await deviceServices.GetIdentityDeviceRecoverAsync("");

            // Assert
            actual.Should().Equals(expected);
        }

        [Fact]
        public async Task IdentityDeviceInsertAsync_WhenCall_ReturnExactData()
        {
            // Arrange
            var transactionRecovers = fixture.CreateMany<TransactionRecover>();
            deviceDataAcess.IdentityDeviceInsertAsync(Arg.Any<string>()).Returns(transactionRecovers);

            // Act
            var actual = await deviceServices.IdentityDeviceInsertAsync("transIds");

            // Assert
            Assert.Equal(transactionRecovers, actual);
        }

        [Fact]
        public async Task IdentityEvolutionInsertAsync_WhenCall_ReturnExactData()
        {
            // Arrange
            var transactionRecovers = fixture.CreateMany<TransactionRecover>();
            var model = fixture.CreateMany<IdentifyEvolutionModel>();
            deviceDataAcess.IdentityEvolutionInsertAsync(Arg.Any<string>()).Returns(transactionRecovers);

            // Act
            var actual = await deviceServices.IdentityEvolutionInsertAsync(model);

            // Assert
            Assert.Equal(transactionRecovers, actual);
        }

        [Fact]
        public async Task TransformTransaction07InsertAsync_WhenCall_ReturnExactData()
        {
            // Arrange
            var model = fixture.CreateMany<TransformTransactionModel>();
            deviceDataAcess.TransformTransaction07InsertAsync(Arg.Any<string>()).Returns(1);

            // Act
            var actual = await deviceServices.TransformTransaction07InsertAsync(model);

            // Assert
            Assert.Equal(1, actual);
        }

        [Fact]
        public async Task TransformTransaction07InsertAsync_WhenEmpty_ReturnDefault()
        {
            // Arrange
            var model =  new List<TransformTransactionModel>();

            // Act
            var actual = await deviceServices.TransformTransaction07InsertAsync(model);

            // Assert
            Assert.Equal(0, actual);
        }
    }
}
