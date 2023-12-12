namespace Fanex.NAP.SPU.DCS.Tests.Features.Monitoring
{
    using System;
    using AutoFixture;
    using Fanex.NAP.SPU.DCS.Features.Monitoring;
    using Fanex.NAP.SPU.DCS.Features.Monitoring.Model;
    using NSubstitute;
    using Xunit;

    public class MonitoringServicesTests
    {
        private IMonitoringDataAccess monitoringDataAccess;
        private IMonitoringServices monitoringServices;

        public MonitoringServicesTests()
        {
            monitoringDataAccess = Substitute.For<IMonitoringDataAccess>();

            monitoringServices = new MonitoringServices(monitoringDataAccess);
        }

        [Fact]
        public void UpdateSumRawTransaction_WhenCall_ReturnExactData()
        {
            // Arrange
            var fixture = new Fixture();
            var sumTransaction = fixture.Create<SumTransactionModel>();
            monitoringDataAccess.UpdateSumRawTransaction(
                    Arg.Any<SumTransactionModel>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>()
                ).Returns(1);

            // Act
            var actual = monitoringServices.UpdateSumRawTransaction(sumTransaction);

            // Assert
            Assert.Equal(1, actual);
        }

        [Fact]
        public void GetNotificationTransaction_WhenCall_ReturnExactData()
        {
            // Arrange
            var fixture = new Fixture();
            var notifyTransactionModel = fixture.Create<NotifyTransactionModel>();
            monitoringDataAccess.GetNotificationTransaction().Returns(notifyTransactionModel);

            // Act
            var actual = monitoringServices.GetNotificationTransaction();

            // Assert
            Assert.Equal(notifyTransactionModel, actual);
        }

        [Fact]
        public void GetMainCustByDate_WhenCall_ReturnExactData()
        {
            // Arrange
            var fixture = new Fixture();
            var custModels = fixture.CreateMany<CustModel>();
            monitoringDataAccess.GetMainCustByDate(Arg.Any<DateTime>(), Arg.Any<int>(), Arg.Any<int>()).Returns(custModels);

            // Act
            var actual = monitoringServices.GetMainCustByDate(DateTime.Now, 1, 1);

            // Assert
            Assert.Equal(custModels, actual);
        }

        [Fact]
        public void InsertMonitorTransaction_WhenCall_ReturnExactData()
        {
            // Arrange
            monitoringDataAccess.InsertMonitorTransaction(Arg.Any<DateTime>(), Arg.Any<string>()).Returns(1);

            // Act
            var actual = monitoringServices.InsertMonitorTransaction(DateTime.Now, "transJson");

            // Assert
            Assert.Equal(1, actual);
        }

        [Fact]
        public void InsertNotifyMonitorTransaction_WhenCall_ReturnExactData()
        {
            // Arrange
            uint expectd = 1;
            monitoringDataAccess.InsertNotifyMonitorTransaction(Arg.Any<DateTime>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<int>()).Returns(expectd);

            // Act
            var actual = monitoringServices.InsertNotifyMonitorTransaction(DateTime.Now, "transJson", "groupId", 1);

            // Assert
            Assert.Equal(expectd, actual);
        }

        [Fact]
        public void GetNotifyMessageById_WhenCall_ReturnExactData()
        {
            // Arrange
            var fixture = new Fixture();
            uint notifyMessageID = 1;
            var notifyMessageModel = fixture.Create<NotifyMessageModel>();
            monitoringDataAccess.GetNotifyMessageById(Arg.Any<uint>()).Returns(notifyMessageModel);

            // Act
            var actual = monitoringServices.GetNotifyMessageById(notifyMessageID);

            // Assert
            Assert.Equal(notifyMessageModel, actual);
        }

        [Fact]
        public void GetNotifyMessage_WhenCall_ReturnExactData()
        {
            // Arrange
            var fixture = new Fixture();
            var notifyType = "notifyType";
            var batchSize = 1;
            var notifyMessageModels = fixture.CreateMany<NotifyMessageModel>();
            monitoringDataAccess.GetNotifyMessage(notifyType, batchSize).Returns(notifyMessageModels);

            // Act
            var actual = monitoringServices.GetNotifyMessage(notifyType, batchSize);

            // Assert
            Assert.Equal(notifyMessageModels, actual);
        }

        [Fact]
        public void UpdateNotifyMessagesToNotified_WhenCall_ReturnExactData()
        {
            // Arrange
            monitoringDataAccess.UpdateNotifyMessagesToNotified(Arg.Any<string>()).Returns(1);

            // Act
            var actual = monitoringServices.UpdateNotifyMessagesToNotified("ids");

            // Assert
            Assert.Equal(1, actual);
        }
    }
}
