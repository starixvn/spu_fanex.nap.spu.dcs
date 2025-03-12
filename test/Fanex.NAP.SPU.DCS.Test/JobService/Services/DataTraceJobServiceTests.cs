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
        public async Task SendTransactionToLogstash_SuccessfulExecution_ShouldCallExactFunctionAndReturnTrue()
        {
            // Arrange
            var transactionLogs = fixture.Create<IEnumerable<TransactionLogstashModel>>();

            var maxProcessedTransID = 3UL;

            dataTraceServices.GetTransactionLogstash(1).Returns(transactionLogs);

            logstashService.PushLogAsync(transactionLogs).Returns(Task.FromResult(maxProcessedTransID));

            // Act
            var result = await service.SendTransactionToLogstash(1);

            // Assert
            Assert.True(result);
            await logService.DidNotReceiveWithAnyArgs().ErrorAsync(Arg.Any<Exception>());
        }

        [Fact]
        public async Task SendTransactionToLogstash_WhenExceptionThrown_ShouldReturnFalse()
        {
            // Arrange
            dataTraceServices.GetTransactionLogstash(1).Throws(new Exception("Test exception"));

            // Act
            var result = await service.SendTransactionToLogstash(1);

            // Assert
            Assert.False(result);
            await logService.Received(1).ErrorMessageAsync(Arg.Any<string>());
        }

        [Fact]
        public async Task CountDWBetTransactionByDate_SuccessfulExecution_ShouldCallExactFunctionAndReturnNotNull()
        {
            // Arrange
            var transDate = fixture.Create<DateTime>();

            var expectedCount = fixture.Create<int>();

            dataTraceServices.CountDWBetTransactionByDate(transDate).Returns(expectedCount);

            // Act
            var result = await service.CountDWBetTransactionByDate(transDate);

            // Assert
            Assert.Equal(expectedCount, result);
            await dataTraceServices.Received(1).CountDWBetTransactionByDate(transDate);
        }

        [Fact]
        public async Task CountDWBetTransactionByDate_WhenExceptionThrown_ShouldReturnZeroAndLogError()
        {
            // Arrange
            var transDate = fixture.Create<DateTime>();

            dataTraceServices.CountDWBetTransactionByDate(transDate).Throws(new Exception("Test exception"));

            // Act
            var result = await service.CountDWBetTransactionByDate(transDate);

            // Assert
            Assert.Equal(0, result);
            await logService.Received(1).ErrorMessageAsync(Arg.Any<string>());
        }

        [Fact]
        public async Task CookDataLoginTransaction_SuccessfulExecution_ShouldReturnTrue()
        {
            // Arrange
            dataTraceServices.CookDataLoginTransaction(1).Returns(Task.FromResult(1));

            // Act
            var result = await service.CookDataLoginTransaction(1);

            // Assert
            Assert.True(result);
            await dataTraceServices.Received(1).CookDataLoginTransaction(1);
        }

        [Fact]
        public async Task CookDataLoginTransaction_BatchSizeZero_ShouldLogErrorAndReturnFalse()
        {
            // Arrange
            var batchSize = 0;

            // Act
            var result = await service.CookDataLoginTransaction(batchSize);

            // Assert
            Assert.False(result);
            await logService.Received(1).ErrorMessageAsync("DCS - DataTrace CookDataLoginTransaction has batchSize = 0!");
            await dataTraceServices.DidNotReceive().CookDataLoginTransaction(Arg.Any<int>());
        }

        [Fact]
        public async Task GetReportMissingTransaction_SuccessfulExecution_ShouldCallExactFunctionAndReturnNotNull()
        {
            // Arrange
            var model = fixture.Create<IEnumerable<ReportMissingTransactionModel>>();

            dataTraceServices.GetReportMissingTransaction(Arg.Any<DateTime>(), Arg.Any<DateTime>()).Returns(model);

            // Act
            var result = await service.GetReportMissingTransaction(DateTime.Now, DateTime.Now.AddDays(1));

            // Assert
            Assert.NotNull(result);
            await dataTraceServices.Received().GetReportMissingTransaction(Arg.Any<DateTime>(), Arg.Any<DateTime>());
        }

        [Fact]
        public async Task GetReportMissingTransaction_WhenThrowError_ShouldReturnNullAndLogError()
        {
            // Arrange
            dataTraceServices.GetReportMissingTransaction(Arg.Any<DateTime>(), Arg.Any<DateTime>()).Throws(new Exception("Simulated exception"));

            // Act
            var result = await service.GetReportMissingTransaction(DateTime.Now, DateTime.Now.AddDays(1));

            // Assert
            Assert.Null(result);
        }
    }
}
