
namespace Fanex.NAP.SPU.DCS.Tests.Features.Monitoring
{
    using System;
    using AutoFixture;
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.DCS.Features.Monitoring.Criteria;
    using Fanex.NAP.SPU.DCS.Features.Monitoring.Model;
    using Fanex.NAP.SPU.DCS.Features.Monitoring;
    using NSubstitute;
    using Xunit;

    public class MonitoringDataAccessTest
    {
        private readonly IDynamicRepository repository;
        private readonly IMonitoringDataAccess monitoringDataAccess;
        private readonly IFixture fixture = new Fixture();

        public MonitoringDataAccessTest()
        {
            repository = Substitute.For<IDynamicRepository>();
            monitoringDataAccess = new MonitoringDataAccess(repository);
        }

        [Fact]
        public void GetSumTransaction_WhenCalled_ReturnsExpectedData()
        {
            //// Arrange
            //var expectedSumTransaction = fixture.Create<SumTransactionModel>();
            //var criteria = new GetSumTransactionCriteria();

            //var db = Substitute.For<IObjectDb>();
            //db.QueryMultiple(Arg.Any<GetSumTransactionCriteria>()).Returns(expectedSumTransaction);

            //ObjectDbFactory.CreateInstance(criteria.GetSettingKey()).Returns(db);

            //// Act
            //var actual = monitoringDataAccess.GetSumTransaction();

            //// Assert
            //Assert.Equal(expectedSumTransaction.SumSubscriberList, actual.SumSubscriberList);
            //Assert.Equal(expectedSumTransaction.SumDeviceTypeList, actual.SumDeviceTypeList);
            //Assert.Equal(expectedSumTransaction.AccountIPList, actual.AccountIPList);
            //Assert.Equal(expectedSumTransaction.AccountDeviceList, actual.AccountDeviceList);
            //Assert.Equal(expectedSumTransaction.MaxTransID, actual.MaxTransID);
            //Assert.Equal(expectedSumTransaction.DateScan, actual.DateScan);
            //Assert.Equal(expectedSumTransaction.AccountID, actual.AccountID);
        }

        [Fact]
        public void UpdateSumRawTransaction_WhenCalled_ExecutesWithCorrectCriteria()
        {
            // Arrange
            var sumTrans = fixture.Create<SumTransactionModel>();
            var sumSubsJson = fixture.Create<string>();
            var sumDeviceTypeJson = fixture.Create<string>();
            var accountIPJson = fixture.Create<string>();
            var accountDeviceJson = fixture.Create<string>();
            var criteria = fixture.Create<UpdateSumTransactionCriteria>();

            // Act
            monitoringDataAccess.UpdateSumRawTransaction(sumTrans, sumSubsJson, sumDeviceTypeJson, accountIPJson, accountDeviceJson);

            // Assert
            repository.Received(1).Execute(Arg.Is<UpdateSumTransactionCriteria>(c =>
                c.MaxTransID == sumTrans.MaxTransID &&
                c.DateScan == sumTrans.DateScan &&
                c.AccountID == sumTrans.AccountID &&
                c.SumSubscriberJson == sumSubsJson &&
                c.SumDeviceTypeJson == sumDeviceTypeJson &&
                c.AccountIPJson == accountIPJson &&
                c.AccountDeviceJson == accountDeviceJson &&
                c.GetSettingKey() == "DCS_DC_SumTransaction_Update"));
        }

        [Fact]
        public void GetNotificationTransaction_WhenCalled_ReturnsExpectedData()
        {
            //// Arrange
            //var expectedNotifyTransaction = fixture.Create<NotifyTransactionModel>();
            //var criteria = fixture.Create<GetNotificationTransactionCriteria>();

            //var db = Substitute.For<IDatabase>();
            //db.QueryMultiple(Arg.Any<GetNotificationTransactionCriteria>()).Returns(new QueryMultipleResultMock(expectedNotifyTransaction));
            //ObjectDbFactory.CreateInstance(Arg.Any<string>()).Returns(db);

            //// Act
            //var actual = monitoringDataAccess.GetNotificationTransaction();

            //// Assert
            //var settingKey = criteria.GetSettingKey();
            //Assert.Equal(expectedNotifyTransaction, actual);
            //Assert.Equal("DCS_DC_Notification_Transaction", settingKey);
        }

        [Fact]
        public void GetMainCustByDate_WhenCalled_ReturnsExpectedData()
        {
            //// Arrange
            //var rptDate = fixture.Create<DateTime>();
            //var minCustID = fixture.Create<int>();
            //var batchSize = fixture.Create<int>();
            //var expectedCustModels = fixture.CreateMany<CustModel>();
            //var criteria = new GetMainCustByDateCriteria
            //{
            //    RptDate = rptDate,
            //    MinCustID = minCustID,
            //    BatchSize = batchSize
            //};

            //var db = Substitute.For<IObjectDb>();
            //db.Query<CustModel>(criteria).Returns(expectedCustModels);
            //ObjectDbFactory.CreateInstance(Arg.Any<string>()).Returns(db);

            //// Act
            //var actual = monitoringDataAccess.GetMainCustByDate(rptDate, minCustID, batchSize);

            //// Assert
            //Assert.Equal(expectedCustModels, actual);
            //db.Received(1).Query<CustModel>(criteria);
        }

        [Fact]
        public void InsertMonitorTransaction_WhenCalled_ExecutesWithCorrectCriteria()
        {
            // Arrange
            var rptDate = fixture.Create<DateTime>();
            var transJson = fixture.Create<string>();
            var criteria = fixture.Create<InsertMonitorTransactionCriteria>();

            // Act
            monitoringDataAccess.InsertMonitorTransaction(rptDate, transJson);

            // Assert
            repository.Received(1).Execute(Arg.Is<InsertMonitorTransactionCriteria>(c =>
                c.RptDate == rptDate &&
                c.TransJson == transJson &&
                c.GetSettingKey() == "DCS_DC_MonitorTransaction_Insert"));
        }

        [Fact]
        public void InsertNotifyMonitorTransaction_WhenCalled_ExecutesWithCorrectCriteria()
        {
            // Arrange
            var rptDate = fixture.Create<DateTime>();
            var notifyType = fixture.Create<string>();
            var groupId = fixture.Create<string>();
            var limitShowCust = fixture.Create<int>();
            var criteria = fixture.Create<InsertNotifyMonitorTransactionCriteria>();
            var expectedSettingKey = "DCS_DC_NotifyMessage_InsertMonitorTransaction";

            repository.Execute(Arg.Any<InsertNotifyMonitorTransactionCriteria>()).Returns(1);

            // Act
            var result = monitoringDataAccess.InsertNotifyMonitorTransaction(rptDate, notifyType, groupId, limitShowCust);

            // Assert
            var settingKey = criteria.GetSettingKey();
            Assert.Equal<uint>(0, result);
            Assert.Equal(expectedSettingKey, settingKey);
        }

        [Fact]
        public void GetNotifyMessageById_WhenCalled_ReturnsExpectedData()
        {
            //// Arrange
            //var notifyMessageID = fixture.Create<uint>();
            //var expectedNotifyMessage = fixture.Create<NotifyMessageModel>();
            //var criteria = fixture.Create<GetNotifyMessageByIDCriteria>();

            //var db = Substitute.For<IDatabase>();
            //db.QueryEntity<NotifyMessageModel>(Arg.Any<GetNotifyMessageByIDCriteria>()).Returns(expectedNotifyMessage);
            //ObjectDbFactory.CreateInstance(Arg.Any<string>()).Returns(db);

            //// Act
            //var actual = monitoringDataAccess.GetNotifyMessageById(notifyMessageID);

            //// Assert
            //var settingKey = criteria.GetSettingKey();
            //Assert.Equal(expectedNotifyMessage, actual);
            //Assert.Equal("DCS_DC_NotifyMessage_GetByID", settingKey);
        }

        [Fact]
        public void GetNotifyMessage_WhenCalled_ReturnsExpectedData()
        {
            //// Arrange
            //var notifyType = fixture.Create<string>();
            //var batchSize = fixture.Create<int>();
            //var expectedNotifyMessages = fixture.CreateMany<NotifyMessageModel>();
            //var criteria = fixture.Create<GetNotifyMessageByNotifyTypeCriteria>();

            //var db = Substitute.For<IDatabase>();
            //db.Query<NotifyMessageModel>(Arg.Any<GetNotifyMessageByNotifyTypeCriteria>()).Returns(expectedNotifyMessages);
            //ObjectDbFactory.CreateInstance(Arg.Any<string>()).Returns(db);

            //// Act
            //var actual = monitoringDataAccess.GetNotifyMessage(notifyType, batchSize);

            //// Assert
            //var settingKey = criteria.GetSettingKey();
            //Assert.Equal(expectedNotifyMessages, actual);
            //Assert.Equal("DCS_DC_NotifyMessage_GetByNotifyType", settingKey);
        }

        [Fact]
        public void UpdateNotifyMessagesToNotified_WhenCalled_ExecutesWithCorrectCriteria()
        {
            // Arrange
            var ids = fixture.Create<string>();
            var criteria = fixture.Create<UpdateNotifyMessagesToNotifiedCriteria>();

            // Act
            monitoringDataAccess.UpdateNotifyMessagesToNotified(ids);

            // Assert
            repository.Received(1).Execute(Arg.Is<UpdateNotifyMessagesToNotifiedCriteria>(c =>
                c.IDs == ids &&
                c.GetSettingKey() == "DCS_DC_NotifyMessage_UpdateNotified"));
        }
    }
}