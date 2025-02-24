
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