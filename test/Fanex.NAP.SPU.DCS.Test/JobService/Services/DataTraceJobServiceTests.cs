namespace Fanex.NAP.SPU.DCS.Tests.JobService.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoFixture;
    using Fanex.NAP.SPU.DCS.Features.DataTrace;
    using Fanex.NAP.SPU.DCS.Features.DataTrace.Model;
    using Fanex.NAP.SPU.DCS.JobService;
    using Fanex.NAP.SPU.DCS.JobService.Services;
    using NSubstitute;
    using NSubstitute.ExceptionExtensions;
    using Xunit;

    public class DataTraceJobServiceTests
    {
        private IDataTraceServices dataTraceServices;
        private ILogService logService;
        private IAppSettings appSettings;
        private ILogstashService logstashService;


        [Fact]
        public async Task SummaryLoginTransactionsAsync_SuccessfulExecution_ReturnsTrue()
        {
            /*
            // Arrange
            var service = GetService();
            int batchSize = 10;

            dataTraceServices.InsertSummaryLoginTransactionAsync(batchSize).Returns(1);

            // Act
            var result = await service.SummaryLoginTransactionsAsync(batchSize);

            // Assert
            Assert.True(result);
            await dataTraceServices.Received(1).InsertSummaryLoginTransactionAsync(batchSize);
            */
        }

        [Fact]
        public async Task SummaryLoginTransactionsAsync_WhenThrowException_CapturesExceptionAndReturnsFalse()
        {
            /*
            // Arrange
            var service = GetService();
            int batchSize = 10;

            dataTraceServices.InsertSummaryLoginTransactionAsync(batchSize).Throws(new Exception("Simulated exception"));

            // Act
            var result = await service.SummaryLoginTransactionsAsync(batchSize);

            // Assert
            Assert.False(result);
            await logService.Received().ErrorMessageAsync(Arg.Any<string>());
            */
        }

        [Fact]
        public async Task SummaryBetTransactionAsync_SuccessfulExecution_ReturnsTrue()
        {
            // Arrange
            var service = GetService();

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
            var service = GetService();

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
            var service = GetService();

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
            var service = GetService();

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
            var service = GetService();

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
            var service = GetService();

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
            var service = GetService();

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
            var service = GetService();

            dataTraceServices.UpdateSystemSetting(Arg.Any<int>(), Arg.Any<string>()).Throws(new Exception("Simulated exception"));

            // Act
            var result = await service.UpdateSystemSetting(1, "NewValue");

            // Assert
            Assert.False(result);
            await logService.Received().ErrorMessageAsync(Arg.Any<string>());
        }

        [Fact]
        public async Task ReportLoginTransaction_SuccessfulExecution_ShouldCallExactFunctionAndReturnNotNull()
        {
            /*
            // Arrange
            var fixture = new Fixture();
            var model = fixture.Create<IEnumerable<ReportLoginTransactionModel>>();
            var service = GetService();

            dataTraceServices.GetReportLoginTransaction(DateTime.Now, DateTime.Now.AddDays(1)).Returns(model);

            // Act
            var result = await service.ReportLoginTransaction(DateTime.Now, DateTime.Now.AddDays(1));

            // Assert
            Assert.NotNull(result);
            await dataTraceServices.Received().GetReportLoginTransaction(Arg.Any<DateTime>(), Arg.Any<DateTime>());
            */
        }

        [Fact]
        public async Task ReportLoginTransaction_WhenThrowError_ShouldReturnNullAndLogError()
        {
            /*
            // Arrange
            var service = GetService();

            dataTraceServices.GetReportLoginTransaction(Arg.Any<DateTime>(), Arg.Any<DateTime>()).Throws(new Exception("Simulated exception"));

            // Act
            var result = await service.ReportLoginTransaction(DateTime.Now, DateTime.Now.AddDays(1));

            // Assert
            Assert.Null(result);
            */
        }

        [Fact]
        public async Task ReportBotDTransaction_SuccessfulExecution_ShouldCallExactFunctionAndReturnNotNull()
        {
            /*
            // Arrange
            var fixture = new Fixture();
            var model = fixture.Create<IEnumerable<ReportBotDTransactionModel>>();
            var service = GetService();

            dataTraceServices.GetReportBotDTransaction(DateTime.Now, DateTime.Now.AddDays(1)).Returns(model);

            // Act
            var result = await service.ReportBotDTransaction(DateTime.Now, DateTime.Now.AddDays(1));

            // Assert
            Assert.NotNull(result);
            await dataTraceServices.Received().GetReportBotDTransaction(Arg.Any<DateTime>(), Arg.Any<DateTime>());
            */
        }

        [Fact]
        public async Task ReportMissingTransaction_SuccessfulExecution_ShouldCallExactFunctionAndReturnNotNull()
        {
            // Arrange
            var fixture = new Fixture();
            var model = fixture.Create<IEnumerable<ReportMissingTransactionModel>>();
            var service = GetService();

            dataTraceServices.GetReportMissingTransaction(DateTime.Now, DateTime.Now.AddDays(1)).Returns(model);

            // Act
            var result = await service.GetReportMissingTransaction(DateTime.Now, DateTime.Now.AddDays(1));

            // Assert
            Assert.NotNull(result);
            await dataTraceServices.Received().GetReportMissingTransaction(Arg.Any<DateTime>(), Arg.Any<DateTime>());
        }

        [Fact]
        public async Task ReportLoginTransactionDetail_SuccessfulExecution_ShouldCallExactFunctionAndReturnNotNull()
        {


            /*
            // Arrange
            var fixture = new Fixture();
            var model = fixture.Create<IEnumerable<ReportLoginTransactionDetailModel>>();
            var service = GetService();

            dataTraceServices.GetReportLoginTransactionDetail(DateTime.Now).Returns(model);

            // Act
            var result = await service.ReportLoginTransactionDetail(DateTime.Now);

            // Assert
            Assert.NotNull(result);
            await dataTraceServices.Received().GetReportLoginTransactionDetail(Arg.Any<DateTime>());
            */
        }

        [Fact]
        public async Task SendTransactionToLogstash_SuccessfulExecution_ShouldCallExactFunctionAndReturnTrue()
        {
            // Arrange
            var fixture = new Fixture();
            var service = GetService();
            var transactionLogs = fixture.Create<IEnumerable<TransactionLogsTashModel>>();
            dataTraceServices.GetTransactionLogstash(1).Returns(transactionLogs);
            appSettings.IsEnableLogstash.Returns(true);
            logstashService.PushLogAsync(transactionLogs).Returns(Task.CompletedTask);

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
            var service = GetService();
            dataTraceServices.GetTransactionLogstash(1).Throws(new Exception("Test exception"));

            // Act
            var result = await service.SendTransactionToLogstash(1);

            // Assert
            Assert.False(result);
            await logService.Received(1).ErrorMessageAsync(Arg.Any<string>());
        }

        [Fact]
        public async Task InsertLogstash_WhenLogstashDisabled_ShouldNotCallPushLogAsync()
        {
            // Arrange
            var service = GetService();
            var fixture = new Fixture();
            var transactionLogs = fixture.CreateMany<TransactionLogsTashModel>();
            appSettings.IsEnableLogstash.Returns(false);

            // Act
            await service.InsertLogstash(transactionLogs);

            // Assert
            await logstashService.DidNotReceive().PushLogAsync(transactionLogs);
        }

        public DataTraceJobService GetService()
        {
            dataTraceServices = Substitute.For<IDataTraceServices>();
            appSettings = Substitute.For<IAppSettings>();
            logstashService = Substitute.For<ILogstashService>();
            logService = Substitute.For<ILogService>();
            return new DataTraceJobService(dataTraceServices, logService, appSettings, logstashService);
        }
    }
}
