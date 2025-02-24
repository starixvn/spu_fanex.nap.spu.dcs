namespace Fanex.NAP.SPU.DCS.Tests.Features.DataTrace
{
    using AutoFixture;
    using System.Threading.Tasks;
    using Fanex.NAP.SPU.DCS.Features.DataTrace;
    using Fanex.NAP.SPU.DCS.JobService.Services;
    using NSubstitute;
    using Xunit;
    using System;
    using Fanex.NAP.SPU.DCS.Features.DataTrace.Model;
    using System.Collections.Generic;

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
        public async Task GetReportMissingTransaction_WhenCalledWithEmptyData_ShouldReturnEmptyList()
        {
            // Arrange
            var reportMissingTransactions = new List<ReportMissingTransactionModel>();

            dataAccess.GetReportMissingTransaction(Arg.Any<DateTime>(), Arg.Any<DateTime>()).Returns(reportMissingTransactions);

            // Act
            var actual = await dataTraceServices.GetReportMissingTransaction(DateTime.Now, DateTime.Now);

            // Assert
            Assert.Empty(actual);
            await dataAccess.Received(1).GetReportMissingTransaction(Arg.Any<DateTime>(), Arg.Any<DateTime>());
        }

        [Fact]
        public async Task GetReportMissingTransaction_WhenCalledWithNullData_ShouldReturnNull()
        {
            // Arrange
            List<ReportMissingTransactionModel> reportMissingTransactions = null;

            dataAccess.GetReportMissingTransaction(Arg.Any<DateTime>(), Arg.Any<DateTime>()).Returns(reportMissingTransactions);

            // Act
            var actual = await dataTraceServices.GetReportMissingTransaction(DateTime.Now, DateTime.Now);

            // Assert
            Assert.Null(actual);
            await dataAccess.Received(1).GetReportMissingTransaction(Arg.Any<DateTime>(), Arg.Any<DateTime>());
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
        public async Task CountDWBetTransactionByDate_WhenDailyTaskReadyAndNoData_ShouldReturnZero()
        {
            // Arrange
            var betTransactions = new List<BetTransactionInfo>();

            dataAccess.IsDailyTaskReadyAsync(Arg.Any<int>()).Returns(true);

            dataAccess.GetBetTransactionAsync(Arg.Any<DateTime>()).Returns(betTransactions);

            // Act
            var actual = await dataTraceServices.CountDWBetTransactionByDate(DateTime.Now);

            // Assert
            Assert.Equal(0, actual);
            await dataAccess.Received(1).GetBetTransactionAsync(Arg.Any<DateTime>());
        }

        [Fact]
        public async Task CountDWBetTransactionByDate_WhenDailyTaskReadyAndNullData_ShouldReturnZero()
        {
            // Arrange
            List<BetTransactionInfo> betTransactions = null;

            dataAccess.IsDailyTaskReadyAsync(Arg.Any<int>()).Returns(true);

            dataAccess.GetBetTransactionAsync(Arg.Any<DateTime>()).Returns(betTransactions);

            // Act
            var actual = await dataTraceServices.CountDWBetTransactionByDate(DateTime.Now);

            // Assert
            Assert.Equal(0, actual);
            await dataAccess.Received(1).GetBetTransactionAsync(Arg.Any<DateTime>());
        }

        [Fact]
        public async Task GetTransactionLogstash_WhenCalled_ShouldCallExactFunction()
        {
            // Arrange
            var expectedTransactions = fixture.CreateMany<TransactionLogstashModel>();

            dataAccess.GetTranPackageAsync(Arg.Any<int>()).Returns(expectedTransactions);

            // Act

            var actual = await dataTraceServices.GetTransactionLogstash(1);

            // Assert
            Assert.Equal(expectedTransactions, actual);
        }

        [Fact]
        public async Task GetTransactionLogstash_WhenCalledWithEmptyData_ShouldReturnEmptyList()
        {
            // Arrange
            var expectedTransactions = new List<TransactionLogstashModel>();

            dataAccess.GetTranPackageAsync(Arg.Any<int>()).Returns(expectedTransactions);

            // Act
            var actual = await dataTraceServices.GetTransactionLogstash(1);

            // Assert
            Assert.Empty(actual);
            await dataAccess.Received(1).GetTranPackageAsync(Arg.Any<int>());
        }

        [Fact]
        public async Task GetTransactionLogstash_WhenCalledWithNullData_ShouldReturnNull()
        {
            // Arrange
            List<TransactionLogstashModel> expectedTransactions = null;

            dataAccess.GetTranPackageAsync(Arg.Any<int>()).Returns(expectedTransactions);

            // Act
            var actual = await dataTraceServices.GetTransactionLogstash(1);

            // Assert
            Assert.Null(actual);
            await dataAccess.Received(1).GetTranPackageAsync(Arg.Any<int>());
        }

        [Fact]
        public async Task GetReportLoginTransactionBySubscriber_WhenCalled_ShouldReturnExactData()
        {
            // Arrange
            var expectedData = fixture.CreateMany<ReportLoginTransactionBySubscriberModel>();

            dataAccess.GetReportLoginTransactionBySubscriber(Arg.Any<DateTime>(), Arg.Any<DateTime>())
                      .Returns(expectedData);

            // Act
            var actual = await dataTraceServices.GetReportLoginTransactionBySubscriber(DateTime.Now, DateTime.Now);

            // Assert
            Assert.Equal(expectedData, actual);
        }

        [Fact]
        public async Task GetReportLoginTransactionBySubscriber_WhenCalledWithEmptyData_ShouldReturnEmptyList()
        {
            // Arrange
            var expectedData = new List<ReportLoginTransactionBySubscriberModel>();

            dataAccess.GetReportLoginTransactionBySubscriber(Arg.Any<DateTime>(), Arg.Any<DateTime>())
                      .Returns(expectedData);

            // Act
            var actual = await dataTraceServices.GetReportLoginTransactionBySubscriber(DateTime.Now, DateTime.Now);

            // Assert
            Assert.Empty(actual);
            await dataAccess.Received(1).GetReportLoginTransactionBySubscriber(Arg.Any<DateTime>(), Arg.Any<DateTime>());
        }

        [Fact]
        public async Task GetReportLoginTransactionBySubscriber_WhenCalledWithNullData_ShouldReturnNull()
        {
            // Arrange
            List<ReportLoginTransactionBySubscriberModel> expectedData = null;

            dataAccess.GetReportLoginTransactionBySubscriber(Arg.Any<DateTime>(), Arg.Any<DateTime>())
                      .Returns(expectedData);

            // Act
            var actual = await dataTraceServices.GetReportLoginTransactionBySubscriber(DateTime.Now, DateTime.Now);

            // Assert
            Assert.Null(actual);
            await dataAccess.Received(1).GetReportLoginTransactionBySubscriber(Arg.Any<DateTime>(), Arg.Any<DateTime>());
        }

        [Fact]
        public async Task GetReportLoginTransactionBySite_WhenCalled_ShouldReturnExactData()
        {
            // Arrange
            var expectedData = fixture.CreateMany<ReportLoginTransactionBySiteModel>();

            dataAccess.GetReportLoginTransactionBySite(Arg.Any<DateTime>(), Arg.Any<DateTime>())
                      .Returns(expectedData);

            // Act
            var actual = await dataTraceServices.GetReportLoginTransactionBySite(DateTime.Now, DateTime.Now);

            // Assert
            Assert.Equal(expectedData, actual);
        }

        [Fact]
        public async Task GetReportLoginTransactionBySite_WhenCalledWithEmptyData_ShouldReturnEmptyList()
        {
            // Arrange
            var expectedData = new List<ReportLoginTransactionBySiteModel>();

            dataAccess.GetReportLoginTransactionBySite(Arg.Any<DateTime>(), Arg.Any<DateTime>())
                      .Returns(expectedData);

            // Act
            var actual = await dataTraceServices.GetReportLoginTransactionBySite(DateTime.Now, DateTime.Now);

            // Assert
            Assert.Empty(actual);
            await dataAccess.Received(1).GetReportLoginTransactionBySite(Arg.Any<DateTime>(), Arg.Any<DateTime>());
        }

        [Fact]
        public async Task GetReportLoginTransactionBySite_WhenCalledWithNullData_ShouldReturnNull()
        {
            // Arrange
            List<ReportLoginTransactionBySiteModel> expectedData = null;

            dataAccess.GetReportLoginTransactionBySite(Arg.Any<DateTime>(), Arg.Any<DateTime>())
                      .Returns(expectedData);

            // Act
            var actual = await dataTraceServices.GetReportLoginTransactionBySite(DateTime.Now, DateTime.Now);

            // Assert
            Assert.Null(actual);
            await dataAccess.Received(1).GetReportLoginTransactionBySite(Arg.Any<DateTime>(), Arg.Any<DateTime>());
        }

        [Fact]
        public async Task GetReportLoginTransactionByAccount_WhenCalled_ShouldReturnExactData()
        {
            // Arrange
            var expectedData = fixture.CreateMany<ReportLoginTransactionByAccountModel>();

            dataAccess.GetReportLoginTransactionByAccount(Arg.Any<DateTime>(), Arg.Any<DateTime>())
                      .Returns(expectedData);

            // Act
            var actual = await dataTraceServices.GetReportLoginTransactionByAccount(DateTime.Now, DateTime.Now);

            // Assert
            Assert.Equal(expectedData, actual);
        }

        [Fact]
        public async Task GetReportLoginTransactionByAccount_WhenCalledWithEmptyData_ShouldReturnEmptyList()
        {
            // Arrange
            var expectedData = new List<ReportLoginTransactionByAccountModel>();

            dataAccess.GetReportLoginTransactionByAccount(Arg.Any<DateTime>(), Arg.Any<DateTime>())
                      .Returns(expectedData);

            // Act
            var actual = await dataTraceServices.GetReportLoginTransactionByAccount(DateTime.Now, DateTime.Now);

            // Assert
            Assert.Empty(actual);
            await dataAccess.Received(1).GetReportLoginTransactionByAccount(Arg.Any<DateTime>(), Arg.Any<DateTime>());
        }

        [Fact]
        public async Task GetReportLoginTransactionByAccount_WhenCalledWithNullData_ShouldReturnNull()
        {
            // Arrange
            List<ReportLoginTransactionByAccountModel> expectedData = null;

            dataAccess.GetReportLoginTransactionByAccount(Arg.Any<DateTime>(), Arg.Any<DateTime>())
                      .Returns(expectedData);

            // Act
            var actual = await dataTraceServices.GetReportLoginTransactionByAccount(DateTime.Now, DateTime.Now);

            // Assert
            Assert.Null(actual);
            await dataAccess.Received(1).GetReportLoginTransactionByAccount(Arg.Any<DateTime>(), Arg.Any<DateTime>());
        }

        [Fact]
        public async Task GetReportLoginTransactionByBrowser_WhenCalled_ShouldReturnExactData()
        {
            // Arrange
            var expectedData = fixture.CreateMany<ReportLoginTransactionByBrowserModel>();

            dataAccess.GetReportLoginTransactionByBrowser(Arg.Any<DateTime>(), Arg.Any<DateTime>())
                      .Returns(expectedData);

            // Act
            var actual = await dataTraceServices.GetReportLoginTransactionByBrowser(DateTime.Now, DateTime.Now);

            // Assert
            Assert.Equal(expectedData, actual);
        }

        [Fact]
        public async Task GetReportLoginTransactionByBrowser_WhenCalledWithEmptyData_ShouldReturnEmptyList()
        {
            // Arrange
            var expectedData = new List<ReportLoginTransactionByBrowserModel>();

            dataAccess.GetReportLoginTransactionByBrowser(Arg.Any<DateTime>(), Arg.Any<DateTime>())
                      .Returns(expectedData);

            // Act
            var actual = await dataTraceServices.GetReportLoginTransactionByBrowser(DateTime.Now, DateTime.Now);

            // Assert
            Assert.Empty(actual);
            await dataAccess.Received(1).GetReportLoginTransactionByBrowser(Arg.Any<DateTime>(), Arg.Any<DateTime>());
        }

        [Fact]
        public async Task GetReportLoginTransactionByBrowser_WhenCalledWithNullData_ShouldReturnNull()
        {
            // Arrange
            List<ReportLoginTransactionByBrowserModel> expectedData = null;

            dataAccess.GetReportLoginTransactionByBrowser(Arg.Any<DateTime>(), Arg.Any<DateTime>())
                      .Returns(expectedData);

            // Act
            var actual = await dataTraceServices.GetReportLoginTransactionByBrowser(DateTime.Now, DateTime.Now);

            // Assert
            Assert.Null(actual);
            await dataAccess.Received(1).GetReportLoginTransactionByBrowser(Arg.Any<DateTime>(), Arg.Any<DateTime>());
        }

        [Fact]
        public async Task GetReportLoginTransactionByFlagged_WhenCalled_ShouldReturnExactData()
        {
            // Arrange
            var expectedData = fixture.CreateMany<ReportLoginTransactionByFlaggedModel>();

            dataAccess.GetReportLoginTransactionByFlagged(Arg.Any<DateTime>(), Arg.Any<DateTime>())
                      .Returns(expectedData);

            // Act
            var actual = await dataTraceServices.GetReportLoginTransactionByFlagged(DateTime.Now, DateTime.Now);

            // Assert
            Assert.Equal(expectedData, actual);
        }

        [Fact]
        public async Task GetReportLoginTransactionByFlagged_WhenCalledWithEmptyData_ShouldReturnEmptyList()
        {
            // Arrange
            var expectedData = new List<ReportLoginTransactionByFlaggedModel>();

            dataAccess.GetReportLoginTransactionByFlagged(Arg.Any<DateTime>(), Arg.Any<DateTime>())
                      .Returns(expectedData);

            // Act
            var actual = await dataTraceServices.GetReportLoginTransactionByFlagged(DateTime.Now, DateTime.Now);

            // Assert
            Assert.Empty(actual);
            await dataAccess.Received(1).GetReportLoginTransactionByFlagged(Arg.Any<DateTime>(), Arg.Any<DateTime>());
        }

        [Fact]
        public async Task GetReportLoginTransactionByFlagged_WhenCalledWithNullData_ShouldReturnNull()
        {
            // Arrange
            List<ReportLoginTransactionByFlaggedModel> expectedData = null;

            dataAccess.GetReportLoginTransactionByFlagged(Arg.Any<DateTime>(), Arg.Any<DateTime>())
                      .Returns(expectedData);

            // Act
            var actual = await dataTraceServices.GetReportLoginTransactionByFlagged(DateTime.Now, DateTime.Now);

            // Assert
            Assert.Null(actual);
            await dataAccess.Received(1).GetReportLoginTransactionByFlagged(Arg.Any<DateTime>(), Arg.Any<DateTime>());
        }
    }
}
