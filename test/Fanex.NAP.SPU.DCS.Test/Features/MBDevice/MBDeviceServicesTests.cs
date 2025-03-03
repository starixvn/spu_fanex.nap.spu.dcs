
namespace Fanex.NAP.SPU.DCS.Tests.Features.MBDevice
{
    using AutoFixture;
    using Fanex.NAP.SPU.DCS.Features.MBDevice.Entities;
    using Fanex.NAP.SPU.DCS.Features.MBDevice;
    using Xunit;
    using NSubstitute;

    public class MBDeviceServicesTests
    {
        private readonly IMBDeviceDataAccess deviceDataAccess;
        private readonly IMBDeviceServices deviceServices;
        private readonly IFixture fixture = new Fixture();

        public MBDeviceServicesTests()
        {
            deviceDataAccess = Substitute.For<IMBDeviceDataAccess>();
            deviceServices = new MBDeviceServices(deviceDataAccess);
        }

        [Fact]
        public void GetMBTransaction_WhenCalled_ReturnsExpectedData()
        {
            // Arrange
            int limitTotalRecord = 10;

            int numberOfBatch = 2;

            var expectedTransactions = fixture.CreateMany<MBTransactionEntity>();

            deviceDataAccess.GetMBTransaction(limitTotalRecord, numberOfBatch).Returns(expectedTransactions);

            // Act
            var actual = deviceServices.GetMBTransaction(limitTotalRecord, numberOfBatch);

            // Assert
            Assert.Equal(expectedTransactions, actual);
        }

        [Fact]
        public void TransformDeviceInsert_WhenCalled_ExecutesWithCorrectCriteria()
        {
            // Arrange
            var transactionInfoJson = fixture.Create<string>();

            // Act
            deviceServices.TransformDeviceInsert(transactionInfoJson);

            // Assert
            deviceDataAccess.Received(1).InsertDevice(transactionInfoJson);
        }
    }

}
