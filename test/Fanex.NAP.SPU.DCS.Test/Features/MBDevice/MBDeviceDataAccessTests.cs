

namespace Fanex.NAP.SPU.DCS.Tests.Features.MBDevice
{
    using AutoFixture;
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.DCS.Features.MBDevice.Criteria;
    using Fanex.NAP.SPU.DCS.Features.MBDevice;
    using NSubstitute;
    using Xunit;

    public class MBDeviceDataAccessTests
    {
        private readonly IDynamicRepository repository;
        private readonly IMBDeviceDataAccess mbDeviceDataAccess;
        private readonly IFixture fixture = new Fixture();

        public MBDeviceDataAccessTests()
        {
            repository = Substitute.For<IDynamicRepository>();
            mbDeviceDataAccess = new MBDeviceDataAccess(repository);
        }

        [Fact]
        public void InsertDevice_WhenCalled_ExecutesWithCorrectCriteria()
        {
            // Arrange
            var transactionInfoJson = fixture.Create<string>();

            var criteria = fixture.Create<InsertMBDeviceCriteria>();

            // Act
            mbDeviceDataAccess.InsertDevice(transactionInfoJson);

            // Assert
            repository.Received(1).Execute(Arg.Is<InsertMBDeviceCriteria>(c =>
                c.MBTransactionInfoJson == transactionInfoJson &&
                c.GetSettingKey() == "DCS_DC_Transform_MBDevice_Insert"));
        }
    }
}
