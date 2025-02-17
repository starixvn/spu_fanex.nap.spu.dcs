namespace Fanex.NAP.SPU.DCS.Tests.Features.DataTrace
{
    using AutoFixture;
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.DCS.Features.DataTrace;
    using Fanex.NAP.SPU.DCS.JobService.Services;
    using NSubstitute;
    using Xunit;
    using System;
    using Fanex.NAP.SPU.DCS.Features.DataTrace.Model;
    using NSubstitute.ExceptionExtensions;
    using System.Collections.Generic;
    using Fanex.NAP.SPU.DCS.Features.DataTrace.Model.Dto;

    public class DataTraceServicesTests
    {
        private IDataTraceDataAccess dataAccess;
        private ILogService logService;
        private IDataTraceServices dataTraceServices;
        private IFixture fixture = new Fixture();

        public DataTraceServicesTests()
        {
            dataAccess = Substitute.For<IDataTraceDataAccess>();
            logService = Substitute.For<ILogService>();

            dataTraceServices = new DataTraceServices(dataAccess, logService);
        }

        [Fact]
        public async Task ProcessSummaryBetTransactionAsync_IsTaskDailyNotReady_ReturnCallExactFunction()
        {
            // Arrange
            dataAccess.IsDailyTaskReadyAsync(Arg.Any<int>()).Returns(false);

            // Act
            var actual = await dataTraceServices.ProcessSummaryBetTransactionAsync(null, null, 1);

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public async Task ProcessSummaryBetTransactionAsync_ValidParamAndTaskDailyReady_ReturnTrueAndCallExactFunction()
        {
            // Arrange
            var betTransactionInfos = fixture.CreateMany<BetTransactionInfo>();

            dataAccess.IsDailyTaskReadyAsync(Arg.Any<int>()).Returns(true);
            dataAccess.GetBetTransactionAsync(Arg.Any<DateTime>()).Returns(betTransactionInfos);
            dataAccess.DeleteBetTransactionSummaryByDateAsync(Arg.Any<DateTime>()).Returns(1);
            dataAccess.InsertSummaryBetTransactionAsync(Arg.Any<string>()).Returns(1);

            // Act
            var actual = await dataTraceServices.ProcessSummaryBetTransactionAsync(DateTime.Now, DateTime.Now, 1);

            // Assert
            Assert.True(actual);
            await dataAccess.Received(3).InsertSummaryBetTransactionAsync(Arg.Any<string>());
        }

        [Fact]
        public async Task ProcessSummaryBetTransactionAsync_NullParam_ShouldCallExactFunction()
        {
            // Arrange
            var betTransactionInfos = fixture.CreateMany<BetTransactionInfo>();

            dataAccess.IsDailyTaskReadyAsync(Arg.Any<int>()).Returns(true);
            dataAccess.GetSystemSettingParameterByID(Arg.Any<int>()).Returns(DateTimeOffset.Now.Date.AddDays(-2).ToString("yyyy-MM-dd"));
            dataAccess.GetBetTransactionAsync(Arg.Any<DateTime>()).Returns(betTransactionInfos);
            dataAccess.DeleteBetTransactionSummaryByDateAsync(Arg.Any<DateTime>()).Returns(1);
            dataAccess.InsertSummaryBetTransactionAsync(Arg.Any<string>()).Returns(1);

            // Act
            var actual = await dataTraceServices.ProcessSummaryBetTransactionAsync(null, null, 1);

            // Assert
            await dataAccess.Received(3).InsertSummaryBetTransactionAsync(Arg.Any<string>());
            await dataAccess.Received().UpdateSystemSettingParameterByID(Arg.Any<int>(), Arg.Any<string>());
            Assert.True(actual);
        }

        [Fact]
        public async Task ProcessSummaryMissingBetTransactionAsync_NullParamAndToDateGreaterThanFromDate_ShouldCallExactFunction()
        {
            // Arrange
            var listAccountIDByBatch = fixture.CreateMany<string>();

            dataAccess.GetSystemSettingParameterByID(4).Returns(DateTimeOffset.Now.Date.ToString("yyyy-MM-dd"));
            dataAccess.GetSystemSettingParameterByID(2).Returns(DateTimeOffset.Now.Date.AddDays(1).ToString("yyyy-MM-dd"));

            dataAccess.GetMissingBetTransactionPackageAsync(Arg.Any<DateTime>(), Arg.Any<int>()).Returns(listAccountIDByBatch);
            dataAccess.DeleteMissingTransactionByDate(Arg.Any<DateTime>(), Arg.Any<int>()).Returns(1);

            // Act
            var actual = await dataTraceServices.ProcessSummaryMissingBetTransactionAsync(null, null, 1);

            // Assert
            await dataAccess.Received().DeleteMissingTransactionByDate(Arg.Any<DateTime>(), Arg.Any<int>());
            await dataAccess.Received(3).InsertMissingBetTransactionAsync(Arg.Any<DateTime>(), Arg.Any<string>());
            Assert.True(actual);
        }

        [Fact]
        public async Task ProcessSummaryMissingBetTransactionAsync_ParamIsNotNullAndToDateGreaterThanFromDate_ShouldCallExactFunction()
        {
            // Arrange

            dataAccess.GetSystemSettingParameterByID(4).Returns(DateTimeOffset.Now.Date.ToString("yyyy-MM-dd"));
            dataAccess.GetSystemSettingParameterByID(2).Returns(DateTimeOffset.Now.Date.AddDays(1).ToString("yyyy-MM-dd"));

            dataAccess.GetMissingBetTransactionPackageAsync(Arg.Any<DateTime>(), Arg.Any<int>()).Returns(new List<string>());
            dataAccess.DeleteMissingTransactionByDate(Arg.Any<DateTime>(), Arg.Any<int>()).Returns(1);

            // Act
            await dataTraceServices.ProcessSummaryMissingBetTransactionAsync(DateTime.Now, DateTime.Now.AddDays(1), 1);

            // Assert
            await logService.Received().ErrorMessageAsync(Arg.Any<string>());
        }

        [Fact]
        public async Task ProcessSummaryMissingLoginTransactionAsync_NullParamAndToDateGreaterThanFromDate_ShouldCallExactFunction()
        {
            // Arrange
            var missingLoginIDPackages = fixture.CreateMany<RangeIDPackage>();

            dataAccess.GetSystemSettingParameterByID(3).Returns(DateTimeOffset.Now.Date.ToString("yyyy-MM-dd"));
            dataAccess.GetSystemSettingParameterByID(2).Returns(DateTimeOffset.Now.Date.AddDays(1).ToString("yyyy-MM-dd"));

            dataAccess.GetMissingLoginTranPackageAsync(Arg.Any<DateTime>(), Arg.Any<int>()).Returns(missingLoginIDPackages);
            dataAccess.DeleteMissingTransactionByDate(Arg.Any<DateTime>(), Arg.Any<int>()).Returns(1);

            // Act
            var actual = await dataTraceServices.ProcessSummaryMissingLoginTransactionAsync(null, null, 1);

            // Assert
            await dataAccess.Received().DeleteMissingTransactionByDate(Arg.Any<DateTime>(), Arg.Any<int>());
            await dataAccess.Received(3).InsertMissingLoginTransactionAsync(Arg.Any<DateTime>(), Arg.Any<int>(), Arg.Any<int>());
            Assert.True(actual);
        }

        [Fact]
        public async Task ProcessSummaryMissingLoginTransactionAsync_ParamIsNotNullAndToDateGreaterThanFromDate_ShouldCallExactFunction()
        {
            // Arrange

            dataAccess.GetSystemSettingParameterByID(3).Returns(DateTimeOffset.Now.Date.ToString("yyyy-MM-dd"));
            dataAccess.GetSystemSettingParameterByID(2).Returns(DateTimeOffset.Now.Date.AddDays(1).ToString("yyyy-MM-dd"));

            dataAccess.GetMissingLoginTranPackageAsync(Arg.Any<DateTime>(), Arg.Any<int>()).Returns(new List<RangeIDPackage>());
            dataAccess.DeleteMissingTransactionByDate(Arg.Any<DateTime>(), Arg.Any<int>()).Returns(1);

            // Act
            await dataTraceServices.ProcessSummaryMissingBetTransactionAsync(DateTime.Now, DateTime.Now.AddDays(1), 1);

            // Assert
            await logService.Received().ErrorMessageAsync(Arg.Any<string>());
        }

        [Fact]
        public async Task UpdateSystemSetting_WhenCall_ShouldCallExactFunction()
        {
            // Act
            var actual = await dataTraceServices.UpdateSystemSetting(1, "SystemSettingValue");

            // Assert
            await dataAccess.Received().UpdateSystemSettingParameterByID(1, "SystemSettingValue");

            Assert.True(actual);
        }

        [Fact]
        public async Task GetReportMissingTransaction_WhenCall_ShouldCallExactFunction()
        {
            // Arrange
            var reportMissingTransactions = fixture.CreateMany<ReportMissingTransactionModel>();
            dataAccess.GetReportMissingTransaction(Arg.Any<DateTime>(), Arg.Any<DateTime>()).Returns(reportMissingTransactions);

            // Act
            var actual = await dataTraceServices.GetReportMissingTransaction(DateTime.Now, DateTime.Now);

            // Assert

            Assert.Equal(reportMissingTransactions, actual);
        }

        [Fact]
        public async Task CountDWBetTransactionByDate_WhenDailyTaskNotReady_ShouldReturnExactData()
        {
            // Arrange
            dataAccess.IsDailyTaskReadyAsync(Arg.Any<int>()).Returns(false);

            // Act
            var actual = await dataTraceServices.CountDWBetTransactionByDate(DateTime.Now);

            // Assert
            Assert.Equal(0, actual);
        }

        [Fact]
        public async Task CountDWBetTransactionByDate_WhenDailyTaskReadyAndHasData_ShouldReturnExactData()
        {
            // Arrange
            var betTransactions = fixture.CreateMany<BetTransactionInfo>();
            dataAccess.IsDailyTaskReadyAsync(Arg.Any<int>()).Returns(true);
            dataAccess.GetBetTransactionAsync(Arg.Any<DateTime>()).Returns(betTransactions);

            // Act
            var actual = await dataTraceServices.CountDWBetTransactionByDate(DateTime.Now);

            // Assert
            Assert.Equal(3, actual);
        }

        [Fact]
        public async Task GetTransactionLogstash_WhenCalled_ShouldCallExactFunction()
        {
            // Arrange
            var expectedTransactions = fixture.CreateMany<TransactionLogsTashModel>();

            dataAccess.GetTranPackageAsync(Arg.Any<int>()).Returns(expectedTransactions);

            // Act

            var actual = await dataTraceServices.GetTransactionLogstash(1);

            // Assert
            Assert.Equal(expectedTransactions, actual);
        }
    }
}
