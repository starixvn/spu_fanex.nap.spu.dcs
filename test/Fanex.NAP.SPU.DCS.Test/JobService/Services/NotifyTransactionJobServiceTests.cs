namespace Fanex.NAP.SPU.DCS.Tests.JobService.Services
{
    using System.Threading.Tasks;
    using System;
    using Fanex.NAP.SPU.DCS.Features.Monitoring;
    using Fanex.NAP.SPU.DCS.JobService;
    using Fanex.NAP.SPU.DCS.JobService.Services;
    using NSubstitute;
    using Xunit;
    using AutoFixture;
    using Fanex.NAP.SPU.DCS.Features.Monitoring.Model;
    using NSubstitute.ExceptionExtensions;
    using System.Collections.Generic;
    using Fanex.NAP.SPU.Notification.Services;

    public class NotifyTransactionJobServiceTests
    {
        private ILogService logService;
        private IMonitoringServices monitoringServices;
        private IAppSettings appSettings;
        private INotificationService notificationService;

        private readonly IFixture fixture = new Fixture();

        [Fact]
        public async Task Execute_HasNotifyTransaction_ShouldReturnTrue()
        {
            // Arrange
            var transactions = fixture.Create<NotifyTransactionModel>();
            var service = GetService();

            monitoringServices.GetNotificationTransaction().Returns(transactions);

            // Act
            var result = await service.Execute();

            // Assert
            Assert.True(result);
            await logService.DidNotReceiveWithAnyArgs().ErrorAsync(Arg.Any<Exception>());
        }

        [Fact]
        public async Task Execute_WhenThrowError_ShouldReturnFalseAndCallExactFunction()
        {
            // Arrange
            var service = GetService();

            monitoringServices.GetNotificationTransaction().Throws(new Exception("Simulate exception"));

            // Act
            var result = await service.Execute();

            // Assert
            Assert.False(result);
            await logService.Received().ErrorAsync(Arg.Any<Exception>());
        }

        [Fact]
        public async Task TestTelegramNotifyMessage_ShouldReturnTrue()
        {
            // Arrange
            var service = GetService();

            // Act
            var result = await service.TestTelegramNotifyMessage();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task MonitorTransactionAndNotify_WhenDoNotHaveMainCust_ShouldReturnTrueAndCallExactFunction()
        {
            // Arrange
            var mainCust = new List<CustModel>();
            var rptDateStr = "2023-01-01";
            var batchSize = 1;
            var limitShowCust = 1;

            var service = GetService();

            monitoringServices.GetMainCustByDate(Arg.Any<DateTime>(), limitShowCust, batchSize).Returns(mainCust);

            // Act
            var result = await service.MonitorTransactionAndNotify(rptDateStr, batchSize, limitShowCust);

            // Assert
            Assert.True(result);

            monitoringServices.DidNotReceiveWithAnyArgs().InsertMonitorTransaction(Arg.Any<DateTime>(), Arg.Any<string>());
            monitoringServices.Received().InsertNotifyMonitorTransaction(Arg.Any<DateTime>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<int>());
        }

        [Fact]
        public async Task MonitorTransactionAndNotify_WhenThrowException_ShouldReturnFalseAndCallExactFunction()
        {
            // Arrange
            var rptDateStr = "2023-01-01";
            var batchSize = 1;
            var limitShowCust = 1;

            var service = GetService();

            monitoringServices.GetMainCustByDate(Arg.Any<DateTime>(), Arg.Any<int>(), Arg.Any<int>()).Throws(new Exception("Simulate exception"));

            // Act
            var result = await service.MonitorTransactionAndNotify(rptDateStr, batchSize, limitShowCust);

            // Assert
            Assert.False(result);

            await logService.Received().ErrorMessageAsync(Arg.Any<string>());
        }

        [Fact]
        public async Task NotifyMessageByID_WhenNotifyMessageIDGreaterThanZero_ShouldReturnTrueAndCallExactFunction()
        {
            // Arrange
            var notifyMessage = fixture.Create<NotifyMessageModel>();
            uint notifyMessageID = 1;
            var service = GetService();

            monitoringServices.GetNotifyMessageById(notifyMessageID).Returns(notifyMessage);

            // Act
            var result = await service.NotifyMessageByID(notifyMessageID);

            // Assert
            Assert.True(result);

            monitoringServices.Received().UpdateNotifyMessagesToNotified(Arg.Any<string>());
        }

        [Fact]
        public async Task NotifyMessageByID_WhenThrowException_ShouldReturnFalseAndCallExactFunction()
        {
            uint notifyMessageID = 1;

            var service = GetService();

            monitoringServices.GetNotifyMessageById(notifyMessageID).Throws(new Exception("Simulate Exception"));

            // Act
            var result = await service.NotifyMessageByID(notifyMessageID);

            // Assert
            Assert.False(result);

            await logService.Received().ErrorMessageAsync(Arg.Any<string>());
        }

        [Fact]
        public async Task NotifyMessageByNotifyType_WhenHaveDatas_ShouldReturnTrueAndCallExactFunction()
        {
            // Arrange
            var notifyMessages = fixture.CreateMany<NotifyMessageModel>();
            var notifyType = "notifyType";
            var batchSize = 1;
            var service = GetService();

            monitoringServices.GetNotifyMessage(notifyType, batchSize).Returns(notifyMessages);

            // Act
            var result = await service.NotifyMessageByNotifyType(notifyType, batchSize);

            // Assert
            Assert.True(result);

            monitoringServices.Received().UpdateNotifyMessagesToNotified(Arg.Any<string>());
        }

        [Fact]
        public async Task NotifyMessageByNotifyType_WhenThrowException_ShouldReturnFalseAndCallExactFunction()
        {
            var notifyType = "notifyType";
            var batchSize = 1;
            var service = GetService();

            monitoringServices.GetNotifyMessage(Arg.Any<string>(), Arg.Any<int>()).Throws(new Exception("Simulate Exception"));

            // Act
            var result = await service.NotifyMessageByNotifyType(notifyType, batchSize);

            // Assert
            Assert.False(result);

            await logService.Received().ErrorMessageAsync(Arg.Any<string>());
        }

        [Fact]
        public async Task TestSentryCaptureMessage_WhenCalled_LogsInfoMessage()
        {
            // Arrange
            var message = fixture.Create<string>();
            var service = GetService();

            // Act
            var result = await service.TestSentryCaptureMessage(message);

            // Assert
            await logService.Received(1).InfoMessageAsync(message);
            Assert.True(result);
        }

        public NotifyTransactionJobService GetService()
        {

            logService = Substitute.For<ILogService>();
            monitoringServices = Substitute.For<IMonitoringServices>();
            appSettings = Substitute.For<IAppSettings>();
            notificationService = Substitute.For<INotificationService>();
            return new NotifyTransactionJobService(monitoringServices, logService, appSettings, notificationService);
        }
    }
}
