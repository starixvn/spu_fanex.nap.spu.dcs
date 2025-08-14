using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Fanex.NAP.SPU.DCS.JobService.Controllers;
using Fanex.NAP.SPU.DCS.JobService.Services;
using Fanex.NAP.SPU.DCS.Features.DataTrace.Model;
using System.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Fanex.NAP.SPU.DCS.Test.JobService.Controllers
{
    public class DataTraceJobControllerTests
    {
        private readonly IDataTraceJobService _mockDataTraceJobService;
        private readonly DataTraceJobController _controller;

        public DataTraceJobControllerTests()
        {
            _mockDataTraceJobService = Substitute.For<IDataTraceJobService>();
            _controller = new DataTraceJobController(_mockDataTraceJobService);
        }

        #region SummaryBetTransaction Tests

        [Fact]
        public async Task SummaryBetTransaction_WithValidParameters_ReturnsTrue()
        {
            // Arrange
            var fromDate = DateTime.Now.AddDays(-7);
            var toDate = DateTime.Now;
            var batchSize = 25000;
            _mockDataTraceJobService.SummaryBetTransactionAsync(fromDate, toDate, batchSize).Returns(true);

            // Act
            var result = await _controller.SummaryBetTransaction(fromDate, toDate, batchSize);

            // Assert
            Assert.True(result);
            await _mockDataTraceJobService.Received(1).SummaryBetTransactionAsync(fromDate, toDate, batchSize);
        }

        [Fact]
        public async Task SummaryBetTransaction_WithNullDates_ReturnsFalse()
        {
            // Arrange
            DateTime? fromDate = null;
            DateTime? toDate = null;
            var batchSize = 25000;
            _mockDataTraceJobService.SummaryBetTransactionAsync(fromDate, toDate, batchSize).Returns(false);

            // Act
            var result = await _controller.SummaryBetTransaction(fromDate, toDate, batchSize);

            // Assert
            Assert.False(result);
            await _mockDataTraceJobService.Received(1).SummaryBetTransactionAsync(fromDate, toDate, batchSize);
        }

        #endregion

        #region SummaryMissingBetTransaction Tests

        [Fact]
        public async Task SummaryMissingBetTransaction_WithValidParameters_ReturnsTrue()
        {
            // Arrange
            var fromDate = DateTime.Now.AddDays(-7);
            var toDate = DateTime.Now;
            var batchSize = 25000;
            _mockDataTraceJobService.SummaryMissingBetTransactionAsync(fromDate, toDate, batchSize).Returns(true);

            // Act
            var result = await _controller.SummaryMissingBetTransaction(fromDate, toDate, batchSize);

            // Assert
            Assert.True(result);
            await _mockDataTraceJobService.Received(1).SummaryMissingBetTransactionAsync(fromDate, toDate, batchSize);
        }

        [Fact]
        public async Task SummaryMissingBetTransaction_WithNullDates_ReturnsFalse()
        {
            // Arrange
            DateTime? fromDate = null;
            DateTime? toDate = null;
            var batchSize = 25000;
            _mockDataTraceJobService.SummaryMissingBetTransactionAsync(fromDate, toDate, batchSize).Returns(false);

            // Act
            var result = await _controller.SummaryMissingBetTransaction(fromDate, toDate, batchSize);

            // Assert
            Assert.False(result);
            await _mockDataTraceJobService.Received(1).SummaryMissingBetTransactionAsync(fromDate, toDate, batchSize);
        }

        #endregion

        #region SummaryMissingLoginTransaction Tests

        [Fact]
        public async Task SummaryMissingLoginTransaction_WithValidParameters_ReturnsTrue()
        {
            // Arrange
            var fromDate = DateTime.Now.AddDays(-7);
            var toDate = DateTime.Now;
            var batchSize = 25000;
            _mockDataTraceJobService.SummaryMissingLoginTransactionAsync(fromDate, toDate, batchSize).Returns(true);

            // Act
            var result = await _controller.SummaryMissingLoginTransaction(fromDate, toDate, batchSize);

            // Assert
            Assert.True(result);
            await _mockDataTraceJobService.Received(1).SummaryMissingLoginTransactionAsync(fromDate, toDate, batchSize);
        }

        [Fact]
        public async Task SummaryMissingLoginTransaction_WithNullDates_ReturnsFalse()
        {
            // Arrange
            DateTime? fromDate = null;
            DateTime? toDate = null;
            var batchSize = 25000;
            _mockDataTraceJobService.SummaryMissingLoginTransactionAsync(fromDate, toDate, batchSize).Returns(false);

            // Act
            var result = await _controller.SummaryMissingLoginTransaction(fromDate, toDate, batchSize);

            // Assert
            Assert.False(result);
            await _mockDataTraceJobService.Received(1).SummaryMissingLoginTransactionAsync(fromDate, toDate, batchSize);
        }

        #endregion

        #region CountDWBetTransactionByDate Tests

        [Fact]
        public async Task CountDWBetTransactionByDate_WithValidDate_ReturnsCount()
        {
            // Arrange
            var transDate = DateTime.Now;
            var expectedCount = 1000;
            _mockDataTraceJobService.CountDWBetTransactionByDate(transDate).Returns(expectedCount);

            // Act
            var result = await _controller.CountDWBetTransactionByDate(transDate);

            // Assert
            Assert.Equal(expectedCount, result);
            await _mockDataTraceJobService.Received(1).CountDWBetTransactionByDate(transDate);
        }

        [Fact]
        public async Task CountDWBetTransactionByDate_WithZeroCount_ReturnsZero()
        {
            // Arrange
            var transDate = DateTime.Now;
            var expectedCount = 0;
            _mockDataTraceJobService.CountDWBetTransactionByDate(transDate).Returns(expectedCount);

            // Act
            var result = await _controller.CountDWBetTransactionByDate(transDate);

            // Assert
            Assert.Equal(expectedCount, result);
            await _mockDataTraceJobService.Received(1).CountDWBetTransactionByDate(transDate);
        }

        #endregion

        #region UpdateSystemSetting Tests

        [Fact]
        public async Task UpdateSystemSetting_WithValidParameters_ReturnsTrue()
        {
            // Arrange
            var id = 1;
            var value = "test_value";
            _mockDataTraceJobService.UpdateSystemSetting(id, value).Returns(true);

            // Act
            var result = await _controller.UpdateSystemSetting(id, value);

            // Assert
            Assert.True(result);
            await _mockDataTraceJobService.Received(1).UpdateSystemSetting(id, value);
        }

        [Fact]
        public async Task UpdateSystemSetting_WithInvalidParameters_ReturnsFalse()
        {
            // Arrange
            var id = 0;
            var value = "";
            _mockDataTraceJobService.UpdateSystemSetting(id, value).Returns(false);

            // Act
            var result = await _controller.UpdateSystemSetting(id, value);

            // Assert
            Assert.False(result);
            await _mockDataTraceJobService.Received(1).UpdateSystemSetting(id, value);
        }

        #endregion

        #region CookDataLoginTransaction Tests

        [Fact]
        public async Task CookDataLoginTransaction_WithValidBatchSize_ReturnsTrue()
        {
            // Arrange
            var batchSize = 5000;
            _mockDataTraceJobService.CookDataLoginTransaction(batchSize).Returns(true);

            // Act
            var result = await _controller.CookDataLoginTransaction(batchSize);

            // Assert
            Assert.True(result);
            await _mockDataTraceJobService.Received(1).CookDataLoginTransaction(batchSize);
        }

        [Fact]
        public async Task CookDataLoginTransaction_WithDefaultBatchSize_ReturnsTrue()
        {
            // Arrange
            var batchSize = 5000; // Default value
            _mockDataTraceJobService.CookDataLoginTransaction(batchSize).Returns(true);

            // Act
            var result = await _controller.CookDataLoginTransaction();

            // Assert
            Assert.True(result);
            await _mockDataTraceJobService.Received(1).CookDataLoginTransaction(batchSize);
        }

        #endregion

        #region SendTransactionToLogstash Tests

        [Fact]
        public void SendTransactionToLogstash_WithValidBatchSize_ReturnsTrue()
        {
            // Arrange
            var batchSize = 5000;
            _mockDataTraceJobService.SendTransactionToLogstash(batchSize).Returns(true);

            // Act
            var result = _controller.SendTransactionToLogstash(batchSize);

            // Assert
            Assert.True(result);
            _mockDataTraceJobService.Received(1).SendTransactionToLogstash(batchSize);
        }

        [Fact]
        public void SendTransactionToLogstash_WithDefaultBatchSize_ReturnsTrue()
        {
            // Arrange
            var batchSize = 5000; // Default value
            _mockDataTraceJobService.SendTransactionToLogstash(batchSize).Returns(true);

            // Act
            var result = _controller.SendTransactionToLogstash();

            // Assert
            Assert.True(result);
            _mockDataTraceJobService.Received(1).SendTransactionToLogstash(batchSize);
        }

        #endregion

        #region SendMBTransactionToLogstash Tests

        [Fact]
        public void SendMBTransactionToLogstash_WithValidBatchSize_ReturnsTrue()
        {
            // Arrange
            var batchSize = 5000;
            _mockDataTraceJobService.SendMBTransactionToLogstash(batchSize).Returns(true);

            // Act
            var result = _controller.SendMBTransactionToLogstash(batchSize);

            // Assert
            Assert.True(result);
            _mockDataTraceJobService.Received(1).SendMBTransactionToLogstash(batchSize);
        }

        [Fact]
        public void SendMBTransactionToLogstash_WithDefaultBatchSize_ReturnsTrue()
        {
            // Arrange
            var batchSize = 5000; // Default value
            _mockDataTraceJobService.SendMBTransactionToLogstash(batchSize).Returns(true);

            // Act
            var result = _controller.SendMBTransactionToLogstash();

            // Assert
            Assert.True(result);
            _mockDataTraceJobService.Received(1).SendMBTransactionToLogstash(batchSize);
        }

        #endregion

        #region Report Tests

        [Fact]
        public async Task ReportMissingTransaction_WithValidDates_ReturnsData()
        {
            // Arrange
            var fromDate = DateTime.Now.AddDays(-7);
            var toDate = DateTime.Now;
            var expectedData = new List<ReportMissingTransactionModel>
            {
                new ReportMissingTransactionModel
                {
                    TransDate = DateTime.Now,
                    SubscriberID = 1,
                    SubscriberName = "Test Subscriber",
                    TotalTrans = 100,
                    CountCustWithoutTrans = 10,
                    CountCustMissingTrans = 5
                }
            };

            _mockDataTraceJobService.GetReportMissingTransaction(fromDate, toDate).Returns(expectedData);

            // Act
            var result = await _controller.ReportMissingTransaction(fromDate, toDate);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedData.Count, result.Count());
            await _mockDataTraceJobService.Received(1).GetReportMissingTransaction(fromDate, toDate);
        }

        [Fact]
        public async Task ReportMissingTransactionCSV_WithValidDates_ReturnsFileResult()
        {
            // Arrange
            var fromDate = DateTime.Now.AddDays(-7);
            var toDate = DateTime.Now;
            var testData = new List<ReportMissingTransactionModel>
            {
                new ReportMissingTransactionModel
                {
                    TransDate = new DateTime(2024, 1, 1),
                    SubscriberID = 1,
                    SubscriberName = "Test Subscriber",
                    TotalTrans = 100,
                    CountCustWithoutTrans = 10,
                    CountCustMissingTrans = 5
                }
            };

            _mockDataTraceJobService.GetReportMissingTransaction(fromDate, toDate).Returns(testData);

            // Act
            var result = await _controller.ReportMissingTransactionCSV(fromDate, toDate);

            // Assert
            Assert.IsType<FileContentResult>(result);
            var fileResult = result as FileContentResult;
            Assert.Equal("text/csv", fileResult.ContentType);
            Assert.Equal("ReportMissingTransaction.csv", fileResult.FileDownloadName);
            
            var content = Encoding.UTF8.GetString(fileResult.FileContents);
            Assert.Contains("TransDate,SubscriberID,SubscriberName,TotalTrans,CountCustWithoutTrans,CountCustMissingTrans", content);
            Assert.Contains("2024-01-01,1,Test Subscriber,100,10,5", content);
        }

        [Fact]
        public async Task ReportLoginTransactionBySubscriber_WithValidDates_ReturnsData()
        {
            // Arrange
            var fromDate = DateTime.Now.AddDays(-7);
            var toDate = DateTime.Now;
            var expectedData = new List<ReportLoginTransactionBySubscriberModel>
            {
                new ReportLoginTransactionBySubscriberModel
                {
                    TransDate = DateTime.Now,
                    SubscriberID = 1,
                    SubscriberName = "Test Subscriber",
                    TotalTrans = 100,
                    TotalInvalidBrowers = 5,
                    TotalBotTrans = 2,
                    TotalNoDeviceTrans = 3
                }
            };

            _mockDataTraceJobService.GetReportLoginTransactionBySubscriber(fromDate, toDate).Returns(expectedData);

            // Act
            var result = await _controller.ReportLoginTransactionBySubscriber(fromDate, toDate);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedData.Count, result.Count());
            await _mockDataTraceJobService.Received(1).GetReportLoginTransactionBySubscriber(fromDate, toDate);
        }

        [Fact]
        public async Task ReportLoginTransactionBySubscriberCSV_WithValidDates_ReturnsFileResult()
        {
            // Arrange
            var fromDate = DateTime.Now.AddDays(-7);
            var toDate = DateTime.Now;
            var testData = new List<ReportLoginTransactionBySubscriberModel>
            {
                new ReportLoginTransactionBySubscriberModel
                {
                    TransDate = new DateTime(2024, 1, 1),
                    SubscriberID = 1,
                    SubscriberName = "Test Subscriber",
                    TotalTrans = 100,
                    TotalInvalidBrowers = 5,
                    TotalBotTrans = 2,
                    TotalNoDeviceTrans = 3
                }
            };

            _mockDataTraceJobService.GetReportLoginTransactionBySubscriber(fromDate, toDate).Returns(testData);

            // Act
            var result = await _controller.ReportLoginTransactionBySubscriberCSV(fromDate, toDate);

            // Assert
            Assert.IsType<FileContentResult>(result);
            var fileResult = result as FileContentResult;
            Assert.Equal("text/csv", fileResult.ContentType);
            Assert.Equal("ReportLoginTransactionBySubscriber.csv", fileResult.FileDownloadName);
        }

        [Fact]
        public async Task ReportLoginTransactionBySite_WithValidDates_ReturnsData()
        {
            // Arrange
            var fromDate = DateTime.Now.AddDays(-7);
            var toDate = DateTime.Now;
            var expectedData = new List<ReportLoginTransactionBySiteModel>
            {
                new ReportLoginTransactionBySiteModel
                {
                    TransDate = DateTime.Now,
                    SubscriberID = 1,
                    SubscriberName = "Test Subscriber",
                    Site = "Test Site",
                    PaymentType = "Credit Card",
                    LicenseeType = "Premium",
                    TotalTrans = 100,
                    TotalInvalidBrowers = 5,
                    TotalBotTrans = 2,
                    TotalNoDeviceTrans = 3
                }
            };

            _mockDataTraceJobService.GetReportLoginTransactionBySite(fromDate, toDate).Returns(expectedData);

            // Act
            var result = await _controller.ReportLoginTransactionBySite(fromDate, toDate);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedData.Count, result.Count());
            await _mockDataTraceJobService.Received(1).GetReportLoginTransactionBySite(fromDate, toDate);
        }

        [Fact]
        public async Task ReportLoginTransactionBySiteCSV_WithValidDates_ReturnsFileResult()
        {
            // Arrange
            var fromDate = DateTime.Now.AddDays(-7);
            var toDate = DateTime.Now;
            var testData = new List<ReportLoginTransactionBySiteModel>
            {
                new ReportLoginTransactionBySiteModel
                {
                    TransDate = new DateTime(2024, 1, 1),
                    SubscriberID = 1,
                    SubscriberName = "Test Subscriber",
                    Site = "Test Site",
                    PaymentType = "Credit Card",
                    LicenseeType = "Premium",
                    TotalTrans = 100,
                    TotalInvalidBrowers = 5,
                    TotalBotTrans = 2,
                    TotalNoDeviceTrans = 3
                }
            };

            _mockDataTraceJobService.GetReportLoginTransactionBySite(fromDate, toDate).Returns(testData);

            // Act
            var result = await _controller.ReportLoginTransactionBySiteCSV(fromDate, toDate);

            // Assert
            Assert.IsType<FileContentResult>(result);
            var fileResult = result as FileContentResult;
            Assert.Equal("text/csv", fileResult.ContentType);
            Assert.Equal("ReportLoginTransactionBySite.csv", fileResult.FileDownloadName);
        }

        [Fact]
        public async Task ReportLoginTransactionByAccount_WithValidDates_ReturnsData()
        {
            // Arrange
            var fromDate = DateTime.Now.AddDays(-7);
            var toDate = DateTime.Now;
            var expectedData = new List<ReportLoginTransactionByAccountModel>
            {
                new ReportLoginTransactionByAccountModel
                {
                    TransDate = DateTime.Now,
                    SubscriberID = 1,
                    SubscriberName = "Test Subscriber",
                    AccountID = 1,
                    CustID = 1,
                    LoginName = "testuser",
                    Site = "Test Site",
                    PaymentType = "Credit Card",
                    LicenseeType = "Premium",
                    TotalTrans = 100,
                    TotalInvalidBrowers = 5,
                    TotalBotTrans = 2,
                    TotalNoDeviceTrans = 3
                }
            };

            _mockDataTraceJobService.GetReportLoginTransactionByAccount(fromDate, toDate).Returns(expectedData);

            // Act
            var result = await _controller.ReportLoginTransactionByAccount(fromDate, toDate);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedData.Count, result.Count());
            await _mockDataTraceJobService.Received(1).GetReportLoginTransactionByAccount(fromDate, toDate);
        }

        [Fact]
        public async Task ReportLoginTransactionByAccountCSV_WithValidDates_ReturnsFileResult()
        {
            // Arrange
            var fromDate = DateTime.Now.AddDays(-7);
            var toDate = DateTime.Now;
            var testData = new List<ReportLoginTransactionByAccountModel>
            {
                new ReportLoginTransactionByAccountModel
                {
                    TransDate = new DateTime(2024, 1, 1),
                    SubscriberID = 1,
                    SubscriberName = "Test Subscriber",
                    AccountID = 1,
                    CustID = 1,
                    LoginName = "testuser",
                    Site = "Test Site",
                    PaymentType = "Credit Card",
                    LicenseeType = "Premium",
                    TotalTrans = 100,
                    TotalInvalidBrowers = 5,
                    TotalBotTrans = 2,
                    TotalNoDeviceTrans = 3
                }
            };

            _mockDataTraceJobService.GetReportLoginTransactionByAccount(fromDate, toDate).Returns(testData);

            // Act
            var result = await _controller.ReportLoginTransactionByAccountCSV(fromDate, toDate);

            // Assert
            Assert.IsType<FileContentResult>(result);
            var fileResult = result as FileContentResult;
            Assert.Equal("text/csv", fileResult.ContentType);
            Assert.Equal("ReportLoginTransactionByAccount.csv", fileResult.FileDownloadName);
        }

        [Fact]
        public async Task ReportLoginTransactionByBrowser_WithValidDates_ReturnsData()
        {
            // Arrange
            var fromDate = DateTime.Now.AddDays(-7);
            var toDate = DateTime.Now;
            var expectedData = new List<ReportLoginTransactionByBrowserModel>
            {
                new ReportLoginTransactionByBrowserModel
                {
                    TransDate = DateTime.Now,
                    BrowserID = 1,
                    BrowserName = "Chrome",
                    OSID = 1,
                    OSName = "Windows",
                    DeviceTypeID = 1,
                    DeviceType = "Desktop",
                    TotalTrans = 100,
                    TotalInvalidBrowers = 5,
                    TotalBotTrans = 2
                }
            };

            _mockDataTraceJobService.GetReportLoginTransactionByBrowser(fromDate, toDate).Returns(expectedData);

            // Act
            var result = await _controller.ReportLoginTransactionByBrowser(fromDate, toDate);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedData.Count, result.Count());
            await _mockDataTraceJobService.Received(1).GetReportLoginTransactionByBrowser(fromDate, toDate);
        }

        [Fact]
        public async Task ReportLoginTransactionByBrowserCSV_WithValidDates_ReturnsFileResult()
        {
            // Arrange
            var fromDate = DateTime.Now.AddDays(-7);
            var toDate = DateTime.Now;
            var testData = new List<ReportLoginTransactionByBrowserModel>
            {
                new ReportLoginTransactionByBrowserModel
                {
                    TransDate = new DateTime(2024, 1, 1),
                    BrowserID = 1,
                    BrowserName = "Chrome",
                    OSID = 1,
                    OSName = "Windows",
                    DeviceTypeID = 1,
                    DeviceType = "Desktop",
                    TotalTrans = 100,
                    TotalInvalidBrowers = 5,
                    TotalBotTrans = 2
                }
            };

            _mockDataTraceJobService.GetReportLoginTransactionByBrowser(fromDate, toDate).Returns(testData);

            // Act
            var result = await _controller.ReportLoginTransactionByBrowserCSV(fromDate, toDate);

            // Assert
            Assert.IsType<FileContentResult>(result);
            var fileResult = result as FileContentResult;
            Assert.Equal("text/csv", fileResult.ContentType);
            Assert.Equal("ReportLoginTransactionByBrowser.csv", fileResult.FileDownloadName);
        }

        [Fact]
        public async Task ReportLoginTransactionByFlagged_WithValidDates_ReturnsData()
        {
            // Arrange
            var fromDate = DateTime.Now.AddDays(-7);
            var toDate = DateTime.Now;
            var expectedData = new List<ReportLoginTransactionByFlaggedModel>
            {
                new ReportLoginTransactionByFlaggedModel
                {
                    TransDate = DateTime.Now,
                    Flagged = 1,
                    FlaggedName = "Suspicious",
                    TotalTrans = 100
                }
            };

            _mockDataTraceJobService.GetReportLoginTransactionByFlagged(fromDate, toDate).Returns(expectedData);

            // Act
            var result = await _controller.ReportLoginTransactionByFlagged(fromDate, toDate);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedData.Count, result.Count());
            await _mockDataTraceJobService.Received(1).GetReportLoginTransactionByFlagged(fromDate, toDate);
        }

        [Fact]
        public async Task ReportLoginTransactionByFlaggedCSV_WithValidDates_ReturnsFileResult()
        {
            // Arrange
            var fromDate = DateTime.Now.AddDays(-7);
            var toDate = DateTime.Now;
            var testData = new List<ReportLoginTransactionByFlaggedModel>
            {
                new ReportLoginTransactionByFlaggedModel
                {
                    TransDate = new DateTime(2024, 1, 1),
                    Flagged = 1,
                    FlaggedName = "Suspicious",
                    TotalTrans = 100
                }
            };

            _mockDataTraceJobService.GetReportLoginTransactionByFlagged(fromDate, toDate).Returns(testData);

            // Act
            var result = await _controller.ReportLoginTransactionByFlaggedCSV(fromDate, toDate);

            // Assert
            Assert.IsType<FileContentResult>(result);
            var fileResult = result as FileContentResult;
            Assert.Equal("text/csv", fileResult.ContentType);
            Assert.Equal("ReportLoginTransactionByFlagged.csv", fileResult.FileDownloadName);
        }

        #endregion

        #region Error Handling Tests

        [Fact]
        public async Task SummaryBetTransaction_WhenServiceThrowsException_ReturnsFalse()
        {
            // Arrange
            var fromDate = DateTime.Now.AddDays(-7);
            var toDate = DateTime.Now;
            var batchSize = 25000;
            _mockDataTraceJobService.SummaryBetTransactionAsync(fromDate, toDate, batchSize).Returns(Task.FromException<bool>(new Exception("Test exception")));

            // Act
            var result = await _controller.SummaryBetTransaction(fromDate, toDate, batchSize);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task ReportMissingTransaction_WhenServiceThrowsException_ThrowsException()
        {
            // Arrange
            var fromDate = DateTime.Now.AddDays(-7);
            var toDate = DateTime.Now;
            _mockDataTraceJobService.GetReportMissingTransaction(fromDate, toDate).Returns(Task.FromException<IEnumerable<ReportMissingTransactionModel>>(new Exception("Test exception")));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _controller.ReportMissingTransaction(fromDate, toDate));
        }

        #endregion
    }
}
