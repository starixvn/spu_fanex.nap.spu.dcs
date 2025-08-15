using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Fanex.NAP.SPU.DCS.JobService.Controllers;
using Fanex.NAP.SPU.DCS.JobService.Services;
using System.Text;
using Xunit;
using System;
using System.Threading.Tasks;

namespace Fanex.NAP.SPU.DCS.Test.JobService.Controllers
{
    public class ELKJobControllerTests
    {
        private readonly IELKJobService _mockElkJobService;
        private readonly ELKJobController _controller;

        public ELKJobControllerTests()
        {
            _mockElkJobService = Substitute.For<IELKJobService>();
            _controller = new ELKJobController(_mockElkJobService);
        }

        #region GetBettingMessageCount Tests

        [Fact]
        public async Task GetBettingMessageCount_ReturnsTrue()
        {
            // Arrange
            var utcNow = DateTime.UtcNow;
            var lastMonth = utcNow.AddMonths(-1);
            var expectedStartDate = new DateTime(lastMonth.Year, lastMonth.Month, 1);
            var expectedEndDate = new DateTime(utcNow.Year, utcNow.Month, 1);
            
            _mockElkJobService.Execute(expectedStartDate, expectedEndDate).Returns(true);

            // Act
            var result = await _controller.GetBettingMessageCount();

            // Assert
            Assert.True(result);
            await _mockElkJobService.Received(1).Execute(expectedStartDate, expectedEndDate);
        }

        #endregion

        #region GetBettingMessageCountDaily Tests

        [Fact]
        public async Task GetBettingMessageCountDaily_ReturnsTrue()
        {
            // Arrange
            _mockElkJobService.Execute(Arg.Any<DateTime>(), Arg.Any<DateTime>()).Returns(true);

            // Act
            var result = await _controller.GetBettingMessageCountDaily();

            // Assert
            Assert.True(result);
            await _mockElkJobService.Received(1).Execute(Arg.Any<DateTime>(), Arg.Any<DateTime>());
        }

        #endregion

        #region GetBettingMessageCountDailyManual Tests

        [Fact]
        public async Task GetBettingMessageCountDailyManual_WithExportToFileTrue_ReturnsFileResult()
        {
            // Arrange
            var startTime = DateTime.Now.AddDays(-7);
            var endTime = DateTime.Now;
            var exportToFile = true;
            var expectedResponse = "{\"data\": \"test\"}";
            
            _mockElkJobService.ExecuteExportAsync(startTime, endTime).Returns(expectedResponse);

            // Act
            var result = await _controller.GetBettingMessageCountDaily(startTime, endTime, exportToFile);

            // Assert
            Assert.IsType<FileContentResult>(result);
            var fileResult = result as FileContentResult;
            Assert.Equal("application/json", fileResult.ContentType);
            Assert.Equal($"BettingMessageDaily_{startTime:yyyyMMdd}_{endTime:yyyyMMdd}.json", fileResult.FileDownloadName);
            Assert.Equal(Encoding.UTF8.GetBytes(expectedResponse), fileResult.FileContents);
            await _mockElkJobService.Received(1).ExecuteExportAsync(startTime, endTime);
        }

        [Fact]
        public async Task GetBettingMessageCountDailyManual_WithExportToFileFalse_ReturnsOkResult()
        {
            // Arrange
            var startTime = DateTime.Now.AddDays(-7);
            var endTime = DateTime.Now;
            var exportToFile = false;
            var expectedResult = true;
            
            _mockElkJobService.Execute(startTime, endTime).Returns(expectedResult);

            // Act
            var result = await _controller.GetBettingMessageCountDaily(startTime, endTime, exportToFile);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.Equal(expectedResult, okResult.Value);
            await _mockElkJobService.Received(1).Execute(startTime, endTime);
        }

        #endregion

        #region GetBettingMessageCountDailyByLeagueGroup Tests

        [Fact]
        public async Task GetBettingMessageCountDailyByLeagueGroup_ReturnsTrue()
        {
            // Arrange
            _mockElkJobService.Execute(Arg.Any<DateTime>(), Arg.Any<DateTime>(), Arg.Any<DateTime>()).Returns(Task.FromResult(true));
            _mockElkJobService.Execute(Arg.Any<DateTime>(), Arg.Any<DateTime>(), Arg.Any<DateTime>()).Returns(true);

            // Act
            var result = await _controller.GetBettingMessageCountDailyByLeagueGroup();

            // Assert
            Assert.True(result);
            await _mockElkJobService.Received(2).Execute(Arg.Any<DateTime>(), Arg.Any<DateTime>(), Arg.Any<DateTime>());
        }

        #endregion

        #region GetBettingMessageCountByLeagueGroup Tests

        [Fact]
        public async Task GetBettingMessageCountByLeagueGroup_WithExportToFileTrue_ReturnsFileResult()
        {
            // Arrange
            var startTime = DateTime.Now.AddDays(-7);
            var endTime = DateTime.Now;
            var exportToFile = true;
            var expectedResponse = "{\"data\": \"test\"}";
            
            _mockElkJobService.ExecuteExportAsync(startTime, endTime, startTime).Returns(expectedResponse);

            // Act
            var result = await _controller.GetBettingMessageCountByLeagueGroup(startTime, endTime, exportToFile);

            // Assert
            Assert.IsType<FileContentResult>(result);
            var fileResult = result as FileContentResult;
            Assert.Equal("application/json", fileResult.ContentType);
            Assert.Equal($"BettingMessageDailyLeagueGroup_{startTime:yyyyMMdd}_{endTime:yyyyMMdd}.json", fileResult.FileDownloadName);
            Assert.Equal(Encoding.UTF8.GetBytes(expectedResponse), fileResult.FileContents);
            await _mockElkJobService.Received(1).ExecuteExportAsync(startTime, endTime, startTime);
        }

        [Fact]
        public async Task GetBettingMessageCountByLeagueGroup_WithExportToFileFalse_ReturnsOkResult()
        {
            // Arrange
            var startTime = DateTime.Now.AddDays(-7);
            var endTime = DateTime.Now;
            var exportToFile = false;
            var expectedResult = true;
            
            _mockElkJobService.Execute(startTime, endTime, startTime).Returns(expectedResult);

            // Act
            var result = await _controller.GetBettingMessageCountByLeagueGroup(startTime, endTime, exportToFile);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.Equal(expectedResult, okResult.Value);
            await _mockElkJobService.Received(1).Execute(startTime, endTime, startTime);
        }

        #endregion

        #region GetBettingMessageCount Tests

        [Fact]
        public async Task GetBettingMessageCount_WithExportToFileTrue_ReturnsFileResult()
        {
            // Arrange
            var startTime = DateTime.Now.AddDays(-7);
            var endTime = DateTime.Now;
            var exportToFile = true;
            var expectedResponse = "{\"data\": \"test\"}";
            
            _mockElkJobService.ExecuteExportAsync(startTime, endTime).Returns(expectedResponse);

            // Act
            var result = await _controller.GetBettingMessageCount(startTime, endTime, exportToFile);

            // Assert
            Assert.IsType<FileContentResult>(result);
            var fileResult = result as FileContentResult;
            Assert.Equal("application/json", fileResult.ContentType);
            Assert.Equal($"BettingMessage_{startTime:yyyyMMdd}_{endTime:yyyyMMdd}.json", fileResult.FileDownloadName);
            Assert.Equal(Encoding.UTF8.GetBytes(expectedResponse), fileResult.FileContents);
            await _mockElkJobService.Received(1).ExecuteExportAsync(startTime, endTime);
        }

        [Fact]
        public async Task GetBettingMessageCount_WithExportToFileFalse_ReturnsOkResult()
        {
            // Arrange
            var startTime = DateTime.Now.AddDays(-7);
            var endTime = DateTime.Now;
            var exportToFile = false;
            var expectedResult = true;
            
            _mockElkJobService.Execute(startTime, endTime).Returns(expectedResult);

            // Act
            var result = await _controller.GetBettingMessageCount(startTime, endTime, exportToFile);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.Equal(expectedResult, okResult.Value);
            await _mockElkJobService.Received(1).Execute(startTime, endTime);
        }

        #endregion

        #region Edge Cases Tests

        [Fact]
        public async Task GetBettingMessageCountDailyManual_WithEmptyResponse_ReturnsEmptyFile()
        {
            // Arrange
            var startTime = DateTime.Now.AddDays(-7);
            var endTime = DateTime.Now;
            var exportToFile = true;
            var expectedResponse = "";
            
            _mockElkJobService.ExecuteExportAsync(startTime, endTime).Returns(expectedResponse);

            // Act
            var result = await _controller.GetBettingMessageCountDaily(startTime, endTime, exportToFile);

            // Assert
            Assert.IsType<FileContentResult>(result);
            var fileResult = result as FileContentResult;
            Assert.Equal(Encoding.UTF8.GetBytes(expectedResponse), fileResult.FileContents);
        }

        [Fact]
        public async Task GetBettingMessageCountDailyManual_WithNullResponse_ThrowsException()
        {
            // Arrange
            var startTime = DateTime.Now.AddDays(-7);
            var endTime = DateTime.Now;
            var exportToFile = true;
            string expectedResponse = null;
            
            _mockElkJobService.ExecuteExportAsync(startTime, endTime).Returns(expectedResponse);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => 
                _controller.GetBettingMessageCountDaily(startTime, endTime, exportToFile));
        }

        [Fact]
        public async Task GetBettingMessageCountDailyManual_WithSameStartAndEndTime_ReturnsCorrectResult()
        {
            // Arrange
            var startTime = DateTime.Now;
            var endTime = startTime;
            var exportToFile = false;
            var expectedResult = true;
            
            _mockElkJobService.Execute(startTime, endTime).Returns(expectedResult);

            // Act
            var result = await _controller.GetBettingMessageCountDaily(startTime, endTime, exportToFile);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.Equal(expectedResult, okResult.Value);
        }

        #endregion
    }
}
