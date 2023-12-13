namespace Fanex.NAP.SPU.DCS.Tests.Features.DataTrace
{
    using AutoFixture;
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.DCS.Features.Devices.Entities;
    using Fanex.NAP.SPU.DCS.Features.Devices;
    using NSubstitute;
    using System.Threading.Tasks;
    using Xunit;
    using Fanex.NAP.SPU.DCS.Features.DataTrace;
    using Fanex.NAP.SPU.DCS.Features.DataTrace.Criteria;
    using Fanex.NAP.SPU.DCS.Features.DataTrace.Model;
    using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
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
        public async Task InsertSummaryLoginTransactionAsync_WhenCall_ReturnExactData()
        {
            // Arrange
            var criteria = fixture.Create<InsertSumaryLoginTransactionCriteria>();
            var expectedSettingKey = "DCS_DT_Summary_LoginTransaction_Insert";

            repository.ExecuteAsync(Arg.Do<InsertSumaryLoginTransactionCriteria>(x => criteria = x)).Returns(1);

            // Act
            var actual = await dataTraceDataAccess.InsertSummaryLoginTransactionAsync(1);

            // Assert
            var settingKey = criteria.GetSettingKey();
            Assert.Equal(1, actual);
            Assert.Equal(expectedSettingKey, settingKey);
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
        public async Task GetInitBetTransactionAsync_WhenCall_ReturnExactData()
        {
            // Arrange
            var betTransactionInfos = fixture.CreateMany<BetTransactionInfo>();
            var criteria = fixture.Create<GetInitBetTransactionCriteria>();
            var expectedSettingKey = "Ned_Acc_Rpt_FPS_GetCustBettingInfo";

            repository.FetchAsync<BetTransactionInfo>(Arg.Do<GetInitBetTransactionCriteria>(x => criteria = x)).Returns(betTransactionInfos);

            // Act
            var actual = await dataTraceDataAccess.GetInitBetTransactionAsync(DateTime.Now);

            // Assert
            var settingKey = criteria.GetSettingKey();
            var isValid = criteria.IsInvalid();

            Assert.Equal(betTransactionInfos, actual);
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
        public async Task GetReportLoginTransaction_WhenCall_ReturnExactData()
        {
            // Arrange
            var loginTransactionModels = fixture.CreateMany<ReportLoginTransactionModel>();
            var criteria = fixture.Create<GetReportLoginTransactionCriteria>();
            var expectedSettingKey = "DCS_DT_Report_LoginTransaction_Get";

            repository.FetchAsync<ReportLoginTransactionModel>(Arg.Do<GetReportLoginTransactionCriteria>(x => criteria = x)).Returns(loginTransactionModels);

            // Act
            var actual = await dataTraceDataAccess.GetReportLoginTransaction(DateTime.Now, DateTime.Now);

            // Assert
            var settingKey = criteria.GetSettingKey();
            var isValid = criteria.IsInvalid();

            Assert.Equal(loginTransactionModels, actual);
            Assert.Equal(expectedSettingKey, settingKey);
        }

        [Fact]
        public async Task GetReportBotDTransaction_WhenCall_ReturnExactData()
        {
            // Arrange
            var reportBotDTransactions = fixture.CreateMany<ReportBotDTransactionModel>();
            var criteria = fixture.Create<GetReportBotDTransactionCriteria>();
            var expectedSettingKey = "DCS_DT_Report_BotDTransaction_Get";

            repository.FetchAsync<ReportBotDTransactionModel>(Arg.Do<GetReportBotDTransactionCriteria>(x => criteria = x)).Returns(reportBotDTransactions);

            // Act
            var actual = await dataTraceDataAccess.GetReportBotDTransaction(DateTime.Now, DateTime.Now);

            // Assert
            var settingKey = criteria.GetSettingKey();
            var isValid = criteria.IsInvalid();

            Assert.Equal(reportBotDTransactions, actual);
            Assert.Equal(expectedSettingKey, settingKey);
        }

        [Fact]
        public async Task GetReportLoginTransactionDetail_WhenCall_ReturnExactData()
        {
            // Arrange
            var reportLoginTransactions = fixture.CreateMany<ReportLoginTransactionDetailModel>();
            var criteria = fixture.Create<GetReportLoginTransactionDetailCriteria>();
            var expectedSettingKey = "DCS_DT_Report_LoginTransaction_GetDetailByDate";

            repository.FetchAsync<ReportLoginTransactionDetailModel>(Arg.Do<GetReportLoginTransactionDetailCriteria>(x => criteria = x)).Returns(reportLoginTransactions);

            // Act
            var actual = await dataTraceDataAccess.GetReportLoginTransactionDetail(DateTime.Now);

            // Assert
            var settingKey = criteria.GetSettingKey();
            var isValid = criteria.IsInvalid();

            Assert.Equal(reportLoginTransactions, actual);
            Assert.Equal(expectedSettingKey, settingKey);
        }

        [Fact]
        public async Task GetReportMissingTransaction_WhenCall_ReturnExactData()
        {
            // Arrange
            var reportMissingTransactions = fixture.CreateMany<ReportMissingTransactionModel>();
            var criteria = fixture.Create<GetReportMissingTransactionCriteria>();
            var expectedSettingKey = "DCS_DT_Report_LoginTransaction_GetDetailByDate";

            repository.FetchAsync<ReportMissingTransactionModel>(Arg.Do<GetReportMissingTransactionCriteria>(x => criteria = x)).Returns(reportMissingTransactions);

            // Act
            var actual = await dataTraceDataAccess.GetReportMissingTransaction(DateTime.Now, DateTime.Now);

            // Assert
            var settingKey = criteria.GetSettingKey();
            var isValid = criteria.IsInvalid();

            Assert.Equal(reportMissingTransactions, actual);
            Assert.Equal(expectedSettingKey, settingKey);
        }
    }
}
