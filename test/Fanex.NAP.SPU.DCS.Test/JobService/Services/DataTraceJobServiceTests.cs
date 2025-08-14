namespace Fanex.NAP.SPU.DCS.Tests.JobService.Services
{
    using AutoFixture;
    using Fanex.NAP.SPU.DCS.Features.DataTrace;
    using Fanex.NAP.SPU.DCS.Features.DataTrace.Model;
    using Fanex.NAP.SPU.DCS.Features.SystemSetting;
    using Fanex.NAP.SPU.DCS.JobService;
    using Fanex.NAP.SPU.DCS.JobService.Services;
    using NSubstitute;
    using NSubstitute.ExceptionExtensions;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Xunit;

    public class DataTraceJobServiceTests
    {
        private IDataTraceServices dataTraceServices;
        private ILogService logService;
        private ILogstashService logstashService;
        private ISystemSettingServices systemSettingServices;
        private readonly DataTraceJobService service;
        private readonly IFixture fixture;

        public DataTraceJobServiceTests()
        {
            dataTraceServices = Substitute.For<IDataTraceServices>();
            logService = Substitute.For<ILogService>();
            logstashService = Substitute.For<ILogstashService>();
            systemSettingServices = Substitute.For<ISystemSettingServices>();
            fixture = new Fixture();

            service = new DataTraceJobService(dataTraceServices, logService, logstashService, systemSettingServices);
        }

        [Fact]
        public async Task SummaryBetTransactionAsync_SuccessfulExecution_ReturnsTrue()
        {
            // Arrange
            dataTraceServices.ProcessSummaryBetTransactionAsync(null, null, 1).Returns(true);


            // Act
            var result = await service.SummaryBetTransactionAsync(null, null, 1);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task SummaryBetTransactionAsync_WhenThrowException_CapturesExceptionAndReturnsFalse()
        {
            // Arrange
            dataTraceServices.ProcessSummaryBetTransactionAsync(null, null, 1).Throws(new Exception("Simulated exception"));

            // Act
            var result = await service.SummaryBetTransactionAsync(null, null, 1);

            // Assert
            Assert.False(result);
            await logService.Received().ErrorMessageAsync(Arg.Any<string>());
        }

        [Fact]
        public async Task SummaryMissingBetTransactionAsync_SuccessfulExecution_ReturnsTrue()
        {
            // Arrange
            dataTraceServices.ProcessSummaryMissingBetTransactionAsync(null, null, 1).Returns(true);


            // Act
            var result = await service.SummaryMissingBetTransactionAsync(null, null, 1);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task SummaryMissingBetTransactionAsync_WhenThrowException_CapturesExceptionAndReturnsFalse()
        {
            // Arrange
            dataTraceServices.ProcessSummaryMissingBetTransactionAsync(null, null, 1).Throws(new Exception("Simulated exception"));

            // Act
            var result = await service.SummaryMissingBetTransactionAsync(null, null, 1);

            // Assert
            Assert.False(result);
            await logService.Received().ErrorMessageAsync(Arg.Any<string>());
        }

        [Fact]
        public async Task SummaryMissingLoginTransactionAsync_SuccessfulExecution_ReturnsTrue()
        {
            // Arrange
            dataTraceServices.ProcessSummaryMissingLoginTransactionAsync(null, null, 1).Returns(true);


            // Act
            var result = await service.SummaryMissingLoginTransactionAsync(null, null, 1);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task SummaryMissingLoginTransactionAsync_WhenThrowException_CapturesExceptionAndReturnsFalse()
        {
            // Arrange
            dataTraceServices.ProcessSummaryMissingLoginTransactionAsync(null, null, 1).Throws(new Exception("Simulated exception"));

            // Act
            var result = await service.SummaryMissingLoginTransactionAsync(null, null, 1);

            // Assert
            Assert.False(result);
            await logService.Received().ErrorMessageAsync(Arg.Any<string>());
        }

        [Fact]
        public async Task UpdateSystemSetting_SuccessfulExecution_ReturnsTrue()
        {
            // Arrange
            dataTraceServices.UpdateSystemSetting(Arg.Any<int>(), Arg.Any<string>()).Returns(true);

            // Act
            var result = await service.UpdateSystemSetting(1, "NewValue");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task UpdateSystemSetting_WhenThrowException_CapturesExceptionAndReturnsFalse()
        {
            // Arrange
            dataTraceServices.UpdateSystemSetting(Arg.Any<int>(), Arg.Any<string>()).Throws(new Exception("Simulated exception"));

            // Act
            var result = await service.UpdateSystemSetting(1, "NewValue");

            // Assert
            Assert.False(result);
            await logService.Received().ErrorMessageAsync(Arg.Any<string>());
        }

        [Fact]
        public async Task GetReportLoginTransactionBySubscriber_SuccessfulExecution_ShouldCallExactFunctionAndReturnNotNull()
        {
            // Arrange
            var model = fixture.Create<IEnumerable<ReportLoginTransactionBySubscriberModel>>();

            dataTraceServices.GetReportLoginTransactionBySubscriber(Arg.Any<DateTime>(), Arg.Any<DateTime>()).Returns(model);

            // Act
            var result = await service.GetReportLoginTransactionBySubscriber(DateTime.Now, DateTime.Now.AddDays(1));

            // Assert
            Assert.NotNull(result);
            await dataTraceServices.Received(1).GetReportLoginTransactionBySubscriber(Arg.Any<DateTime>(), Arg.Any<DateTime>());
        }

        [Fact]
        public async Task GetReportLoginTransactionBySubscriber_WhenThrowError_ShouldReturnNullAndLogError()
        {
            // Arrange
            dataTraceServices.GetReportLoginTransactionBySubscriber(Arg.Any<DateTime>(), Arg.Any<DateTime>()).Throws(new Exception("Simulated exception"));

            // Act
            var result = await service.GetReportLoginTransactionBySubscriber(DateTime.Now, DateTime.Now.AddDays(1));

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetReportLoginTransactionBySite_SuccessfulExecution_ShouldCallExactFunctionAndReturnNotNull()
        {
            // Arrange
            var model = fixture.Create<IEnumerable<ReportLoginTransactionBySiteModel>>();

            dataTraceServices.GetReportLoginTransactionBySite(Arg.Any<DateTime>(), Arg.Any<DateTime>()).Returns(model);

            // Act
            var result = await service.GetReportLoginTransactionBySite(DateTime.Now, DateTime.Now.AddDays(1));

            // Assert
            Assert.NotNull(result);
            await dataTraceServices.Received(1).GetReportLoginTransactionBySite(Arg.Any<DateTime>(), Arg.Any<DateTime>());
        }

        [Fact]
        public async Task GetReportLoginTransactionBySite_WhenThrowError_ShouldReturnNullAndLogError()
        {
            // Arrange
            dataTraceServices.GetReportLoginTransactionBySite(Arg.Any<DateTime>(), Arg.Any<DateTime>()).Throws(new Exception("Simulated exception"));

            // Act
            var result = await service.GetReportLoginTransactionBySite(DateTime.Now, DateTime.Now.AddDays(1));

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetReportLoginTransactionByAccount_SuccessfulExecution_ShouldCallExactFunctionAndReturnNotNull()
        {
            // Arrange
            var model = fixture.Create<IEnumerable<ReportLoginTransactionByAccountModel>>();

            dataTraceServices.GetReportLoginTransactionByAccount(Arg.Any<DateTime>(), Arg.Any<DateTime>()).Returns(model);

            // Act
            var result = await service.GetReportLoginTransactionByAccount(DateTime.Now, DateTime.Now.AddDays(1));

            // Assert
            Assert.NotNull(result);
            await dataTraceServices.Received(1).GetReportLoginTransactionByAccount(Arg.Any<DateTime>(), Arg.Any<DateTime>());
        }

        [Fact]
        public async Task GetReportLoginTransactionByAccount_WhenThrowError_ShouldReturnNullAndLogError()
        {
            // Arrange
            dataTraceServices.GetReportLoginTransactionByAccount(Arg.Any<DateTime>(), Arg.Any<DateTime>()).Throws(new Exception("Simulated exception"));

            // Act
            var result = await service.GetReportLoginTransactionByAccount(DateTime.Now, DateTime.Now.AddDays(1));

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetReportLoginTransactionByBrowser_SuccessfulExecution_ShouldCallExactFunctionAndReturnNotNull()
        {
            // Arrange
            var model = fixture.Create<IEnumerable<ReportLoginTransactionByBrowserModel>>();

            dataTraceServices.GetReportLoginTransactionByBrowser(Arg.Any<DateTime>(), Arg.Any<DateTime>()).Returns(model);

            // Act
            var result = await service.GetReportLoginTransactionByBrowser(DateTime.Now, DateTime.Now.AddDays(1));

            // Assert
            Assert.NotNull(result);
            await dataTraceServices.Received(1).GetReportLoginTransactionByBrowser(Arg.Any<DateTime>(), Arg.Any<DateTime>());
        }

        [Fact]
        public async Task GetReportLoginTransactionByBrowser_WhenThrowError_ShouldReturnNullAndLogError()
        {
            // Arrange
            dataTraceServices.GetReportLoginTransactionByBrowser(Arg.Any<DateTime>(), Arg.Any<DateTime>()).Throws(new Exception("Simulated exception"));

            // Act
            var result = await service.GetReportLoginTransactionByBrowser(DateTime.Now, DateTime.Now.AddDays(1));

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetReportLoginTransactionByFlagged_SuccessfulExecution_ShouldCallExactFunctionAndReturnNotNull()
        {
            // Arrange
            var model = fixture.Create<IEnumerable<ReportLoginTransactionByFlaggedModel>>();

            dataTraceServices.GetReportLoginTransactionByFlagged(Arg.Any<DateTime>(), Arg.Any<DateTime>()).Returns(model);

            // Act
            var result = await service.GetReportLoginTransactionByFlagged(DateTime.Now, DateTime.Now.AddDays(1));

            // Assert
            Assert.NotNull(result);
            await dataTraceServices.Received(1).GetReportLoginTransactionByFlagged(Arg.Any<DateTime>(), Arg.Any<DateTime>());
        }

        [Fact]
        public async Task GetReportLoginTransactionByFlagged_WhenThrowError_ShouldReturnNullAndLogError()
        {
            // Arrange
            dataTraceServices.GetReportLoginTransactionByFlagged(Arg.Any<DateTime>(), Arg.Any<DateTime>()).Throws(new Exception("Simulated exception"));

            // Act
            var result = await service.GetReportLoginTransactionByFlagged(DateTime.Now, DateTime.Now.AddDays(1));

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void SendTransactionToLogstash_SuccessfulExecution_ShouldCallExactFunctionAndReturnTrue()
        {
            // Arrange
            var transactionLogs = fixture.Create<IEnumerable<TransactionLogstashModel>>();

            var maxProcessedTransID = 3UL;

            dataTraceServices.GetTransactionLogstash(1).Returns(transactionLogs);

            logstashService.PushLog(transactionLogs).Returns(maxProcessedTransID);
            
            // Mock the systemSettingServices call that happens in the implementation
            systemSettingServices.UpdateDCSystemSettingByID(Arg.Any<int>(), Arg.Any<string>()).Returns(true);

            // Act
            var result = service.SendTransactionToLogstash(1);

            // Assert
            Assert.True(result);
            
            // Verify that systemSettingServices.UpdateDCSystemSettingByID was called with correct parameters
            systemSettingServices.Received(1).UpdateDCSystemSettingByID(101, maxProcessedTransID.ToString());
            
            logService.DidNotReceiveWithAnyArgs().ErrorAsync(Arg.Any<Exception>());
        }

        [Fact]
        public async Task SendTransactionToLogstash_WhenExceptionThrown_ShouldReturnFalse()
        {
            // Arrange
            dataTraceServices.GetTransactionLogstash(1).Throws(new Exception("Test exception"));

            // Act
            var result = service.SendTransactionToLogstash(1);

            // Assert
            Assert.False(result);
            await logService.Received(1).ErrorMessageAsync(Arg.Any<string>());
        }

        [Fact]
        public void SendTransactionToLogstash_WhenNoTransactions_ShouldReturnTrue()
        {
            // Arrange
            dataTraceServices.GetTransactionLogstash(1).Returns((IEnumerable<TransactionLogstashModel>)null);

            // Act
            var result = service.SendTransactionToLogstash(1);

            // Assert
            Assert.True(result);
            
            // Verify that logstashService.PushLog was not called
            logstashService.DidNotReceive().PushLog(Arg.Any<IEnumerable<TransactionLogstashModel>>());
            
            // Verify that systemSettingServices.UpdateDCSystemSettingByID was not called
            systemSettingServices.DidNotReceive().UpdateDCSystemSettingByID(Arg.Any<int>(), Arg.Any<string>());
        }

        [Fact]
        public void SendTransactionToLogstash_WhenEmptyTransactions_ShouldReturnTrue()
        {
            // Arrange
            var emptyTransactionLogs = new List<TransactionLogstashModel>();
            
            dataTraceServices.GetTransactionLogstash(1).Returns(emptyTransactionLogs);

            // Act
            var result = service.SendTransactionToLogstash(1);

            // Assert
            Assert.True(result);
            
            // Verify that logstashService.PushLog was not called
            logstashService.DidNotReceive().PushLog(Arg.Any<IEnumerable<TransactionLogstashModel>>());
            
            // Verify that systemSettingServices.UpdateDCSystemSettingByID was not called
            systemSettingServices.DidNotReceive().UpdateDCSystemSettingByID(Arg.Any<int>(), Arg.Any<string>());
        }

        [Fact]
        public void SendTransactionToLogstash_WhenProcessedTransIDIsZero_ShouldReturnTrue()
        {
            // Arrange
            var transactionLogs = fixture.Create<IEnumerable<TransactionLogstashModel>>();

            dataTraceServices.GetTransactionLogstash(1).Returns(transactionLogs);
            
            logstashService.PushLog(transactionLogs).Returns(0UL); // Return 0 to simulate no processed transactions

            // Act
            var result = service.SendTransactionToLogstash(1);

            // Assert
            Assert.True(result);
            
            // Verify that systemSettingServices.UpdateDCSystemSettingByID was not called when processedTransID is 0
            systemSettingServices.DidNotReceive().UpdateDCSystemSettingByID(Arg.Any<int>(), Arg.Any<string>());
        }

        [Fact]
        public void SendTransactionToLogstash_WhenSystemSettingUpdateFails_ShouldReturnFalse()
        {
            // Arrange
            var transactionLogs = fixture.Create<IEnumerable<TransactionLogstashModel>>();
            var maxProcessedTransID = 3UL;

            dataTraceServices.GetTransactionLogstash(1).Returns(transactionLogs);
            logstashService.PushLog(transactionLogs).Returns(maxProcessedTransID);
            
            // Mock systemSettingServices to return false (indicating update failure)
            systemSettingServices.UpdateDCSystemSettingByID(Arg.Any<int>(), Arg.Any<string>()).Returns(false);

            // Act
            var result = service.SendTransactionToLogstash(1);

            // Assert
            Assert.False(result);
            
            // Verify that systemSettingServices.UpdateDCSystemSettingByID was called
            systemSettingServices.Received(1).UpdateDCSystemSettingByID(101, maxProcessedTransID.ToString());
        }

        [Fact]
        public void SendMBTransactionToLogstash_SuccessfulExecution_ShouldCallExactFunctionAndReturnTrue()
        {
            // Arrange
            var mbTransactionLogs = fixture.Create<IEnumerable<MBTransactionLogstashModel>>();
            var maxProcessedTransID = 5UL;

            dataTraceServices.GetMBTransactionLogtash(1).Returns(mbTransactionLogs);
            logstashService.PushMBLog(mbTransactionLogs).Returns(maxProcessedTransID);
            systemSettingServices.UpdateDCSystemSettingByID(Arg.Any<int>(), Arg.Any<string>()).Returns(true);

            // Act
            var result = service.SendMBTransactionToLogstash(1);

            // Assert
            Assert.True(result);
            
            // Verify that systemSettingServices.UpdateDCSystemSettingByID was called with correct parameters
            systemSettingServices.Received(1).UpdateDCSystemSettingByID(102, maxProcessedTransID.ToString());
        }

        [Fact]
        public void SendMBTransactionToLogstash_WhenExceptionThrown_ShouldReturnFalse()
        {
            // Arrange
            dataTraceServices.GetMBTransactionLogtash(1).Throws(new Exception("Test exception"));

            // Act
            var result = service.SendMBTransactionToLogstash(1);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void SendMBTransactionToLogstash_WhenNoTransactions_ShouldReturnTrue()
        {
            // Arrange
            dataTraceServices.GetMBTransactionLogtash(1).Returns((IEnumerable<MBTransactionLogstashModel>)null);

            // Act
            var result = service.SendMBTransactionToLogstash(1);

            // Assert
            Assert.True(result);
            
            // Verify that logstashService.PushMBLog was not called
            logstashService.DidNotReceive().PushMBLog(Arg.Any<IEnumerable<MBTransactionLogstashModel>>());
            
            // Verify that systemSettingServices.UpdateDCSystemSettingByID was not called
            systemSettingServices.DidNotReceive().UpdateDCSystemSettingByID(Arg.Any<int>(), Arg.Any<string>());
        }

        [Fact]
        public void SendMBTransactionToLogstash_WhenEmptyTransactions_ShouldReturnTrue()
        {
            // Arrange
            var emptyTransactionLogs = new List<MBTransactionLogstashModel>();
            
            dataTraceServices.GetMBTransactionLogtash(1).Returns(emptyTransactionLogs);

            // Act
            var result = service.SendMBTransactionToLogstash(1);

            // Assert
            Assert.True(result);
            
            // Verify that logstashService.PushMBLog was not called
            logstashService.DidNotReceive().PushMBLog(Arg.Any<IEnumerable<MBTransactionLogstashModel>>());
            
            // Verify that systemSettingServices.UpdateDCSystemSettingByID was not called
            systemSettingServices.DidNotReceive().UpdateDCSystemSettingByID(Arg.Any<int>(), Arg.Any<string>());
        }

        [Fact]
        public void SendMBTransactionToLogstash_WhenProcessedTransIDIsZero_ShouldReturnTrue()
        {
            // Arrange
            var mbTransactionLogs = fixture.Create<IEnumerable<MBTransactionLogstashModel>>();

            dataTraceServices.GetMBTransactionLogtash(1).Returns(mbTransactionLogs);
            
            logstashService.PushMBLog(mbTransactionLogs).Returns(0UL); // Return 0 to simulate no processed transactions

            // Act
            var result = service.SendMBTransactionToLogstash(1);

            // Assert
            Assert.True(result);
            
            // Verify that systemSettingServices.UpdateDCSystemSettingByID was not called when processedTransID is 0
            systemSettingServices.DidNotReceive().UpdateDCSystemSettingByID(Arg.Any<int>(), Arg.Any<string>());
        }

        [Fact]
        public void SendMBTransactionToLogstash_WhenSystemSettingUpdateFails_ShouldReturnFalse()
        {
            // Arrange
            var mbTransactionLogs = fixture.Create<IEnumerable<MBTransactionLogstashModel>>();
            var maxProcessedTransID = 5UL;

            dataTraceServices.GetMBTransactionLogtash(1).Returns(mbTransactionLogs);
            logstashService.PushMBLog(mbTransactionLogs).Returns(maxProcessedTransID);
            
            // Mock systemSettingServices to return false (indicating update failure)
            systemSettingServices.UpdateDCSystemSettingByID(Arg.Any<int>(), Arg.Any<string>()).Returns(false);

            // Act
            var result = service.SendMBTransactionToLogstash(1);

            // Assert
            Assert.False(result);
            
            // Verify that systemSettingServices.UpdateDCSystemSettingByID was called
            systemSettingServices.Received(1).UpdateDCSystemSettingByID(102, maxProcessedTransID.ToString());
        }

        [Fact]
        public async Task CookDataLoginTransaction_WhenExceptionThrown_ShouldReturnFalse()
        {
            // Arrange
            dataTraceServices.CookDataLoginTransaction(1).Throws(new Exception("Test exception"));

            // Act
            var result = await service.CookDataLoginTransaction(1);

            // Assert
            Assert.False(result);
            await logService.Received(1).ErrorMessageAsync(Arg.Any<string>());
        }

        [Fact]
        public async Task GetReportMissingTransaction_WhenCalledWithEmptyData_ShouldReturnEmptyList()
        {
            // Arrange
            var emptyData = new List<ReportMissingTransactionModel>();

            dataTraceServices.GetReportMissingTransaction(Arg.Any<DateTime>(), Arg.Any<DateTime>()).Returns(emptyData);

            // Act
            var result = await service.GetReportMissingTransaction(DateTime.Now, DateTime.Now.AddDays(1));

            // Assert
            Assert.Empty(result);
            await dataTraceServices.Received(1).GetReportMissingTransaction(Arg.Any<DateTime>(), Arg.Any<DateTime>());
        }

        [Fact]
        public async Task GetReportLoginTransactionBySubscriber_WhenCalledWithEmptyData_ShouldReturnEmptyList()
        {
            // Arrange
            var emptyData = new List<ReportLoginTransactionBySubscriberModel>();

            dataTraceServices.GetReportLoginTransactionBySubscriber(Arg.Any<DateTime>(), Arg.Any<DateTime>()).Returns(emptyData);

            // Act
            var result = await service.GetReportLoginTransactionBySubscriber(DateTime.Now, DateTime.Now.AddDays(1));

            // Assert
            Assert.Empty(result);
            await dataTraceServices.Received(1).GetReportLoginTransactionBySubscriber(Arg.Any<DateTime>(), Arg.Any<DateTime>());
        }

        [Fact]
        public async Task GetReportLoginTransactionBySite_WhenCalledWithEmptyData_ShouldReturnEmptyList()
        {
            // Arrange
            var emptyData = new List<ReportLoginTransactionBySiteModel>();

            dataTraceServices.GetReportLoginTransactionBySite(Arg.Any<DateTime>(), Arg.Any<DateTime>()).Returns(emptyData);

            // Act
            var result = await service.GetReportLoginTransactionBySite(DateTime.Now, DateTime.Now.AddDays(1));

            // Assert
            Assert.Empty(result);
            await dataTraceServices.Received(1).GetReportLoginTransactionBySite(Arg.Any<DateTime>(), Arg.Any<DateTime>());
        }

        [Fact]
        public async Task GetReportLoginTransactionByAccount_WhenCalledWithEmptyData_ShouldReturnEmptyList()
        {
            // Arrange
            var emptyData = new List<ReportLoginTransactionByAccountModel>();

            dataTraceServices.GetReportLoginTransactionByAccount(Arg.Any<DateTime>(), Arg.Any<DateTime>()).Returns(emptyData);

            // Act
            var result = await service.GetReportLoginTransactionByAccount(DateTime.Now, DateTime.Now.AddDays(1));

            // Assert
            Assert.Empty(result);
            await dataTraceServices.Received(1).GetReportLoginTransactionByAccount(Arg.Any<DateTime>(), Arg.Any<DateTime>());
        }

        [Fact]
        public async Task GetReportLoginTransactionByBrowser_WhenCalledWithEmptyData_ShouldReturnEmptyList()
        {
            // Arrange
            var emptyData = new List<ReportLoginTransactionByBrowserModel>();

            dataTraceServices.GetReportLoginTransactionByBrowser(Arg.Any<DateTime>(), Arg.Any<DateTime>()).Returns(emptyData);

            // Act
            var result = await service.GetReportLoginTransactionByBrowser(DateTime.Now, DateTime.Now.AddDays(1));

            // Assert
            Assert.Empty(result);
            await dataTraceServices.Received(1).GetReportLoginTransactionByBrowser(Arg.Any<DateTime>(), Arg.Any<DateTime>());
        }

        [Fact]
        public async Task GetReportLoginTransactionByFlagged_WhenCalledWithEmptyData_ShouldReturnEmptyList()
        {
            // Arrange
            var emptyData = new List<ReportLoginTransactionByFlaggedModel>();

            dataTraceServices.GetReportLoginTransactionByFlagged(Arg.Any<DateTime>(), Arg.Any<DateTime>()).Returns(emptyData);

            // Act
            var result = await service.GetReportLoginTransactionByFlagged(DateTime.Now, DateTime.Now.AddDays(1));

            // Assert
            Assert.Empty(result);
            await dataTraceServices.Received(1).GetReportLoginTransactionByFlagged(Arg.Any<DateTime>(), Arg.Any<DateTime>());
        }

        [Fact]
        public async Task GetReportLoginTransactionBySubscriber_WhenCalledWithNullData_ShouldReturnNull()
        {
            // Arrange
            dataTraceServices.GetReportLoginTransactionBySubscriber(Arg.Any<DateTime>(), Arg.Any<DateTime>()).Returns((IEnumerable<ReportLoginTransactionBySubscriberModel>)null);

            // Act
            var result = await service.GetReportLoginTransactionBySubscriber(DateTime.Now, DateTime.Now.AddDays(1));

            // Assert
            Assert.Null(result);
            await dataTraceServices.Received(1).GetReportLoginTransactionBySubscriber(Arg.Any<DateTime>(), Arg.Any<DateTime>());
        }

        [Fact]
        public async Task GetReportLoginTransactionBySite_WhenCalledWithNullData_ShouldReturnNull()
        {
            // Arrange
            dataTraceServices.GetReportLoginTransactionBySite(Arg.Any<DateTime>(), Arg.Any<DateTime>()).Returns((IEnumerable<ReportLoginTransactionBySiteModel>)null);

            // Act
            var result = await service.GetReportLoginTransactionBySite(DateTime.Now, DateTime.Now.AddDays(1));

            // Assert
            Assert.Null(result);
            await dataTraceServices.Received(1).GetReportLoginTransactionBySite(Arg.Any<DateTime>(), Arg.Any<DateTime>());
        }

        [Fact]
        public async Task GetReportLoginTransactionByAccount_WhenCalledWithNullData_ShouldReturnNull()
        {
            // Arrange
            dataTraceServices.GetReportLoginTransactionByAccount(Arg.Any<DateTime>(), Arg.Any<DateTime>()).Returns((IEnumerable<ReportLoginTransactionByAccountModel>)null);

            // Act
            var result = await service.GetReportLoginTransactionByAccount(DateTime.Now, DateTime.Now.AddDays(1));

            // Assert
            Assert.Null(result);
            await dataTraceServices.Received(1).GetReportLoginTransactionByAccount(Arg.Any<DateTime>(), Arg.Any<DateTime>());
        }

        [Fact]
        public async Task GetReportLoginTransactionByBrowser_WhenCalledWithNullData_ShouldReturnNull()
        {
            // Arrange
            dataTraceServices.GetReportLoginTransactionByBrowser(Arg.Any<DateTime>(), Arg.Any<DateTime>()).Returns((IEnumerable<ReportLoginTransactionByBrowserModel>)null);

            // Act
            var result = await service.GetReportLoginTransactionByBrowser(DateTime.Now, DateTime.Now.AddDays(1));

            // Assert
            Assert.Null(result);
            await dataTraceServices.Received(1).GetReportLoginTransactionByBrowser(Arg.Any<DateTime>(), Arg.Any<DateTime>());
        }

        [Fact]
        public async Task GetReportLoginTransactionByFlagged_WhenCalledWithNullData_ShouldReturnNull()
        {
            // Arrange
            dataTraceServices.GetReportLoginTransactionByFlagged(Arg.Any<DateTime>(), Arg.Any<DateTime>()).Returns((IEnumerable<ReportLoginTransactionByFlaggedModel>)null);

            // Act
            var result = await service.GetReportLoginTransactionByFlagged(DateTime.Now, DateTime.Now.AddDays(1));

            // Assert
            Assert.Null(result);
            await dataTraceServices.Received(1).GetReportLoginTransactionByFlagged(Arg.Any<DateTime>(), Arg.Any<DateTime>());
        }

        [Fact]
        public async Task GetReportMissingTransaction_WhenCalledWithNullData_ShouldReturnNull()
        {
            // Arrange
            dataTraceServices.GetReportMissingTransaction(Arg.Any<DateTime>(), Arg.Any<DateTime>()).Returns((IEnumerable<ReportMissingTransactionModel>)null);

            // Act
            var result = await service.GetReportMissingTransaction(DateTime.Now, DateTime.Now.AddDays(1));

            // Assert
            Assert.Null(result);
            await dataTraceServices.Received(1).GetReportMissingTransaction(Arg.Any<DateTime>(), Arg.Any<DateTime>());
        }
    }
}
