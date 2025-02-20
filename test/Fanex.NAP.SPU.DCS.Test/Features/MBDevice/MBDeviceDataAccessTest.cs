

namespace Fanex.NAP.SPU.DCS.Tests.Features.MBDevice
{
    using AutoFixture;
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.DCS.Features.MBDevice.Criteria;
    using Fanex.NAP.SPU.DCS.Features.MBDevice;
    using NSubstitute;
    using Xunit;

    public class MBDeviceDataAccessTest
    {
        private readonly IDynamicRepository repository;
        private readonly IMBDeviceDataAccess mbDeviceDataAccess;
        private readonly IFixture fixture = new Fixture();

        public MBDeviceDataAccessTest()
        {
            repository = Substitute.For<IDynamicRepository>();
            mbDeviceDataAccess = new MBDeviceDataAccess(repository);
        }

        [Fact]
        public void GetMBTransaction_WhenCalled_ReturnsExpectedData()
        {
            //// Arrange
            //int limitTotalRecord = 10;
            //int numberOfBatch = 2;
            //var expectedTransactions = fixture.CreateMany<MBTransactionEntity>();
            //var criteria = fixture.Create<GetMBTransactionCriteria>();

            //var db = Substitute.For<IDatabase>();
            //db.Query<MBTransactionEntity>(Arg.Any<GetMBTransactionCriteria>()).Returns(expectedTransactions);
            //ObjectDbFactory.CreateInstance(Arg.Any<string>()).Returns(db);

            //// Act
            //var actual = mbDeviceDataAccess.GetMBTransaction(limitTotalRecord, numberOfBatch);

            //// Assert
            //var settingKey = criteria.GetSettingKey();
            //Assert.Equal(expectedTransactions, actual);
            //Assert.Equal("DCS_DC_Transform_MBTransaction_GetPackage", settingKey);
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
