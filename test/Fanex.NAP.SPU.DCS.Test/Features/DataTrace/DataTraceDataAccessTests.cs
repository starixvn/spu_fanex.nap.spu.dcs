namespace Fanex.NAP.SPU.DCS.Tests.Features.DataTrace
{
    using AutoFixture;
    using Fanex.Data.Repository;
    using NSubstitute;
    using System.Threading.Tasks;
    using Xunit;
    using Fanex.NAP.SPU.DCS.Features.DataTrace;
    using Fanex.NAP.SPU.DCS.Features.DataTrace.Criteria;
    using Fanex.NAP.SPU.DCS.Features.DataTrace.Model;
    using System;

    public class DataTraceDataAccessTests
    {
        private IDynamicRepository repository;
        private IDataTraceDataAccess dataTraceDataAccess;
        private IFixture fixture = new Fixture();

        public DataTraceDataAccessTests()
        {
            repository = Substitute.For<IDynamicRepository>();
            dataTraceDataAccess = new DataTraceDataAccess(repository);
        }

        [Fact]
        public async Task IsDailyTaskReadyAsync_WhenCall_ReturnExactData()
        {
            // Arrange
            var criteria = fixture.Create<CheckDailyTaskReadyCriteria>();

            var expectedSettingKey = "Acc_CheckDailyTaskReadiness";

            repository.GetAsync<bool>(Arg.Do<CheckDailyTaskReadyCriteria>(x => criteria = x)).Returns(true);

            // Act
            var actual = await dataTraceDataAccess.IsDailyTaskReadyAsync(1);

            // Assert
            var settingKey = criteria.GetSettingKey();
            var isValid = criteria.IsInvalid();

            Assert.True(actual);
            Assert.Equal(expectedSettingKey, settingKey);
        }

        [Fact]
        public async Task GetBetTransactionAsync_WhenCall_ReturnExactData()
        {
            // Arrange
            var betTransactions = fixture.CreateMany<BetTransactionInfo>();

            var criteria = fixture.Create<GetBetTransactionCriteria>();

            var expectedSettingKey = "Acc_Rpt_FPS_GetCustBettingInfo";

            repository.FetchAsync<BetTransactionInfo>(Arg.Do<GetBetTransactionCriteria>(x => criteria = x)).Returns(betTransactions);

            // Act
            var actual = await dataTraceDataAccess.GetBetTransactionAsync(DateTime.Now);

            // Assert
            var settingKey = criteria.GetSettingKey();
            var isValid = criteria.IsInvalid();

            Assert.Equal(betTransactions, actual);
            Assert.Equal(expectedSettingKey, settingKey);
        }

        [Fact]
        public async Task InsertSummaryBetTransactionAsync_WhenCall_ReturnExactData()
        {
            // Arrange
            var criteria = fixture.Create<InsertSummaryBetTransactionCriteria>();

            var expectedSettingKey = "DCS_DT_Summary_BetTransaction_Insert";

            repository.ExecuteAsync(Arg.Do<InsertSummaryBetTransactionCriteria>(x => criteria = x)).Returns(1);

            // Act
            var actual = await dataTraceDataAccess.InsertSummaryBetTransactionAsync("rawTransJson");

            // Assert
            var settingKey = criteria.GetSettingKey();
            var isValid = criteria.IsInvalid();

            Assert.Equal(1, actual);
            Assert.Equal(expectedSettingKey, settingKey);
        }

        [Fact]
        public async Task DeleteBetTransactionSummaryByDateAsync_WhenCall_ReturnExactData()
        {
            // Arrange
            var criteria = fixture.Create<DeleteBetTransactionSummaryByDateCriteria>();

            var expectedSettingKey = "DCS_DT_Summary_BetTransaction_Delete";

            repository.ExecuteAsync(Arg.Do<DeleteBetTransactionSummaryByDateCriteria>(x => criteria = x)).Returns(1);

            // Act
            var actual = await dataTraceDataAccess.DeleteBetTransactionSummaryByDateAsync(DateTime.Now);

            // Assert
            var settingKey = criteria.GetSettingKey();
            var isValid = criteria.IsInvalid();

            Assert.Equal(1, actual);
            Assert.Equal(expectedSettingKey, settingKey);
        }

        [Fact]
        public async Task UpdateSystemSettingParameterByID_WhenCall_ReturnExactData()
        {
            // Arrange
            var criteria = fixture.Create<UpdateSystemSettingParameterCriteria>();

            var expectedSettingKey = "DCS_DT_SystemSetting_UpdateParameterByID";

            repository.ExecuteAsync(Arg.Do<UpdateSystemSettingParameterCriteria>(x => criteria = x)).Returns(1);

            // Act
            var actual = await dataTraceDataAccess.UpdateSystemSettingParameterByID(1, "value");

            // Assert
            var settingKey = criteria.GetSettingKey();
            var isValid = criteria.IsInvalid();

            Assert.Equal(1, actual);
            Assert.Equal(expectedSettingKey, settingKey);
        }

        [Fact]
        public async Task GetMissingBetTransactionPackageAsync_WhenCall_ReturnExactData()
        {
            // Arrange
            var missingBets = fixture.CreateMany<string>();

            var criteria = fixture.Create<GetMissingBetTransactionPackageCriteria>();

            var expectedSettingKey = "DCS_DT_Summary_MissingBetTransaction_GetPackage";

            repository.FetchAsync<string>(Arg.Do<GetMissingBetTransactionPackageCriteria>(x => criteria = x)).Returns(missingBets);

            // Act
            var actual = await dataTraceDataAccess.GetMissingBetTransactionPackageAsync(DateTime.Now, 1);

            // Assert
            var settingKey = criteria.GetSettingKey();
            var isValid = criteria.IsInvalid();

            Assert.Equal(missingBets, actual);
            Assert.Equal(expectedSettingKey, settingKey);
        }

        [Fact]
        public async Task GetMissingLoginTranPackageAsync_WhenCall_ReturnExactData()
        {
            // Arrange
            var rangeIDPackages = fixture.CreateMany<RangeIDPackage>();

            var criteria = fixture.Create<GetMissingLoginTransactionPackageCriteria>();

            var expectedSettingKey = "DCS_DT_Summary_MissingLoginTransaction_GetPackage";

            repository.FetchAsync<RangeIDPackage>(Arg.Do<GetMissingLoginTransactionPackageCriteria>(x => criteria = x)).Returns(rangeIDPackages);

            // Act
            var actual = await dataTraceDataAccess.GetMissingLoginTranPackageAsync(DateTime.Now, 1);

            // Assert
            var settingKey = criteria.GetSettingKey();
            var isValid = criteria.IsInvalid();

            Assert.Equal(rangeIDPackages, actual);
            Assert.Equal(expectedSettingKey, settingKey);
        }

        [Fact]
        public async Task InsertMissingLoginTransactionAsync_WhenCall_ReturnExactData()
        {
            // Arrange
            var criteria = fixture.Create<InsertMissingLoginTransactionCriteria>();

            var expectedSettingKey = "DCS_DT_Summary_MissingLoginTransaction_Insert";

            repository.ExecuteAsync(Arg.Do<InsertMissingLoginTransactionCriteria>(x => criteria = x)).Returns(1);

            // Act
            var actual = await dataTraceDataAccess.InsertMissingLoginTransactionAsync(DateTime.Now, 1, 2);

            // Assert
            var settingKey = criteria.GetSettingKey();
            var isValid = criteria.IsInvalid();

            Assert.Equal(1, actual);
            Assert.Equal(expectedSettingKey, settingKey);
        }

        [Fact]
        public async Task DeleteMissingTransactionByDate_WhenCall_ReturnExactData()
        {
            // Arrange
            var criteria = fixture.Create<DeleteMissingTransactionByDateCriteria>();

            var expectedSettingKey = "DCS_DT_Summary_MemberMissingTransaction_DeleteByDate";

            repository.ExecuteAsync(Arg.Do<DeleteMissingTransactionByDateCriteria>(x => criteria = x)).Returns(1);

            // Act
            var actual = await dataTraceDataAccess.DeleteMissingTransactionByDate(DateTime.Now, 1);

            // Assert
            var settingKey = criteria.GetSettingKey();
            var isValid = criteria.IsInvalid();

            Assert.Equal(1, actual);
            Assert.Equal(expectedSettingKey, settingKey);
        }

        [Fact]
        public async Task GetReportMissingTransaction_WhenCall_ReturnExactData()
        {
            // Arrange
            var reportMissingTransactions = fixture.CreateMany<ReportMissingTransactionModel>();

            var criteria = fixture.Create<GetReportMissingTransactionCriteria>();

            var expectedSettingKey = "DCS_DT_Report_MissingTransaction_Get";

            repository.FetchAsync<ReportMissingTransactionModel>(Arg.Do<GetReportMissingTransactionCriteria>(x => criteria = x)).Returns(reportMissingTransactions);

            // Act
            var actual = await dataTraceDataAccess.GetReportMissingTransaction(DateTime.Now, DateTime.Now);

            // Assert
            var settingKey = criteria.GetSettingKey();
            var isValid = criteria.IsInvalid();

            Assert.Equal(reportMissingTransactions, actual);
            Assert.Equal(expectedSettingKey, settingKey);
        }

        [Fact]
        public async Task GetTranPackageAsync_WhenCalled_ReturnsExpectedData()
        {
            // Arrange
            var expectedTransactions = fixture.CreateMany<TransactionLogstashModel>();

            var criteria = fixture.Create<GetTranPackageCriteria>();
            var expectedSettingKey = "DCS_DC_OpenSearch_Transaction_Get";

            repository
                .FetchAsync<TransactionLogstashModel>(Arg.Do<GetTranPackageCriteria>(x => criteria = x))
                .Returns(expectedTransactions);

            // Act
            var actual = await dataTraceDataAccess.GetTranPackageAsync(1);
            var settingKey = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expectedTransactions, actual);
            Assert.Equal(expectedSettingKey, settingKey);
        }

        [Fact]
        public async Task InsertMissingBetTransactionAsync_WhenCall_ReturnExactData()
        {
            // Arrange
            var criteria = fixture.Create<InsertMissingBetTransactionCriteria>();

            var expectedSettingKey = "DCS_DT_Summary_MissingBetTransaction_Insert";

            repository.ExecuteAsync(Arg.Do<InsertMissingBetTransactionCriteria>(x => criteria = x)).Returns(1);

            // Act
            var actual = await dataTraceDataAccess.InsertMissingBetTransactionAsync(DateTime.Now, "value");

            // Assert
            var settingKey = criteria.GetSettingKey();
            var isValid = criteria.IsInvalid();

            Assert.Equal(1, actual);
            Assert.Equal(expectedSettingKey, settingKey);
        }

        [Fact]
        public async Task GetReportLoginTransactionBySubscriber_WhenCalled_ReturnsExactData()
        {
            // Arrange
            var expectedData = fixture.CreateMany<ReportLoginTransactionBySubscriberModel>();

            var criteria = fixture.Create<GetReportLoginTransactionBySubscriberCriteria>();

            var expectedKey = "DCS_DT_Report_LoginTransactionBySubscriber_GetByDateRange";

            repository.FetchAsync<ReportLoginTransactionBySubscriberModel>(Arg.Do<GetReportLoginTransactionBySubscriberCriteria>(x => criteria = x))
                      .Returns(expectedData);

            // Act
            var actualData = await dataTraceDataAccess.GetReportLoginTransactionBySubscriber(DateTime.Now, DateTime.Now);

            // Assert
            Assert.Equal(expectedData, actualData);
            Assert.Equal(expectedKey, criteria.GetSettingKey());
        }

        [Fact]
        public async Task GetReportLoginTransactionBySite_WhenCalled_ReturnsExactData()
        {
            // Arrange
            var expectedData = fixture.CreateMany<ReportLoginTransactionBySiteModel>();

            var criteria = fixture.Create<GetReportLoginTransactionBySiteCriteria>();

            var expectedKey = "DCS_DT_Report_LoginTransactionBySite_GetByDateRange";

            repository.FetchAsync<ReportLoginTransactionBySiteModel>(Arg.Do<GetReportLoginTransactionBySiteCriteria>(x => criteria = x))
                      .Returns(expectedData);

            // Act
            var actualData = await dataTraceDataAccess.GetReportLoginTransactionBySite(DateTime.Now, DateTime.Now);

            // Assert
            Assert.Equal(expectedData, actualData);
            Assert.Equal(expectedKey, criteria.GetSettingKey());
        }

        [Fact]
        public async Task GetReportLoginTransactionByAccount_WhenCalled_ReturnsExactData()
        {
            // Arrange
            var expectedData = fixture.CreateMany<ReportLoginTransactionByAccountModel>();

            var criteria = fixture.Create<GetReportLoginTransactionByAccountCriteria>();

            var expectedKey = "DCS_DT_Report_LoginTransactionByAccount_GetByDateRange";

            repository.FetchAsync<ReportLoginTransactionByAccountModel>(Arg.Do<GetReportLoginTransactionByAccountCriteria>(x => criteria = x))
                      .Returns(expectedData);

            // Act
            var actualData = await dataTraceDataAccess.GetReportLoginTransactionByAccount(DateTime.Now, DateTime.Now);

            // Assert
            Assert.Equal(expectedData, actualData);
            Assert.Equal(expectedKey, criteria.GetSettingKey());
        }

        [Fact]
        public async Task GetReportLoginTransactionByBrowser_WhenCalled_ReturnsExactData()
        {
            // Arrange
            var expectedData = fixture.CreateMany<ReportLoginTransactionByBrowserModel>();

            var criteria = fixture.Create<GetReportLoginTransactionByBrowserCriteria>();

            var expectedKey = "DCS_DT_Report_LoginTransactionByBrowser_GetByDateRange";

            repository.FetchAsync<ReportLoginTransactionByBrowserModel>(Arg.Do<GetReportLoginTransactionByBrowserCriteria>(x => criteria = x))
                      .Returns(expectedData);

            // Act
            var actualData = await dataTraceDataAccess.GetReportLoginTransactionByBrowser(DateTime.Now, DateTime.Now);

            // Assert
            Assert.Equal(expectedData, actualData);
            Assert.Equal(expectedKey, criteria.GetSettingKey());
        }

        [Fact]
        public async Task GetReportLoginTransactionByFlagged_WhenCalled_ReturnsExactData()
        {
            // Arrange
            var expectedData = fixture.CreateMany<ReportLoginTransactionByFlaggedModel>();

            var criteria = fixture.Create<GetReportLoginTransactionByFlaggedCriteria>();

            var expectedKey = "DCS_DT_Report_LoginTransactionByFlagged_GetByDateRange";

            repository.FetchAsync<ReportLoginTransactionByFlaggedModel>(Arg.Do<GetReportLoginTransactionByFlaggedCriteria>(x => criteria = x))
                      .Returns(expectedData);

            // Act
            var actualData = await dataTraceDataAccess.GetReportLoginTransactionByFlagged(DateTime.Now, DateTime.Now);

            // Assert
            Assert.Equal(expectedData, actualData);
            Assert.Equal(expectedKey, criteria.GetSettingKey());
        }

        [Fact]
        public async Task CookDataLoginTransaction_WhenCall_ReturnExactData()
        {
            // Arrange
            var criteria = fixture.Create<CookDataLoginTransactionCriteria>();

            var expectedSettingKey = "DCS_DT_Summary_LoginTransaction_CookData";

            repository.ExecuteAsync(Arg.Do<CookDataLoginTransactionCriteria>(x => criteria = x)).Returns(1);

            // Act
            var actual = await dataTraceDataAccess.CookDataLoginTransaction(1);

            // Assert
            var settingKey = criteria.GetSettingKey();
            var isValid = criteria.IsInvalid();

            Assert.Equal(1, actual);
            Assert.Equal(expectedSettingKey, settingKey);
        }


    }
}
