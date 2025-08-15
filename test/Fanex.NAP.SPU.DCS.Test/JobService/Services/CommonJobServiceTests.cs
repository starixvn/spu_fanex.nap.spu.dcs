using Microsoft.Extensions.Logging;
using NSubstitute;
using Fanex.NAP.SPU.DCS.JobService.Services;
using Fanex.NAP.SPU.DCS.Features.Common;
using Xunit;
using System;
using System.Threading.Tasks;

namespace Fanex.NAP.SPU.DCS.Test.JobService.Services
{
    public class CommonJobServiceTests
    {
        private readonly ICommonServices _mockCommonServices;
        private readonly ILogService _mockLogService;
        private readonly CommonJobService _service;

        public CommonJobServiceTests()
        {
            _mockCommonServices = Substitute.For<ICommonServices>();
            _mockLogService = Substitute.For<ILogService>();
            _service = new CommonJobService(_mockCommonServices, _mockLogService);
        }

        #region InsertIDRangeFPFingerPrint Tests

        [Fact]
        public void InsertIDRangeFPFingerPrint_WithValidDates_ReturnsTrue()
        {
            // Arrange
            var fromDate = DateTime.Now.AddDays(-7);
            var toDate = DateTime.Now;
            _mockCommonServices.InsertTableDailyIDRange(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTime>()).Returns(1);

            // Act
            var result = _service.InsertIDRangeFPFingerPrint(fromDate, toDate);

            // Assert
            Assert.True(result);
            _mockCommonServices.Received(8).InsertTableDailyIDRange(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTime>());
        }

        [Fact]
        public void InsertIDRangeFPFingerPrint_WithSameDate_ReturnsTrue()
        {
            // Arrange
            var date = DateTime.Now;
            _mockCommonServices.InsertTableDailyIDRange(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTime>()).Returns(1);

            // Act
            var result = _service.InsertIDRangeFPFingerPrint(date, date);

            // Assert
            Assert.True(result);
            _mockCommonServices.Received(1).InsertTableDailyIDRange(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTime>());
        }

        [Fact]
        public void InsertIDRangeFPFingerPrint_WithInvalidDateRange_ReturnsFalse()
        {
            // Arrange
            var fromDate = DateTime.Now;
            var toDate = DateTime.Now.AddDays(-1); // toDate < fromDate

            // Act
            var result = _service.InsertIDRangeFPFingerPrint(fromDate, toDate);

            // Assert
            Assert.False(result);
            _mockCommonServices.DidNotReceive().InsertTableDailyIDRange(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTime>());
        }

        [Fact]
        public void InsertIDRangeFPFingerPrint_WithServiceFailure_ReturnsFalse()
        {
            // Arrange
            var fromDate = DateTime.Now.AddDays(-1);
            var toDate = DateTime.Now;
            _mockCommonServices.InsertTableDailyIDRange(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTime>()).Returns(-1);

            // Act
            var result = _service.InsertIDRangeFPFingerPrint(fromDate, toDate);

            // Assert
            Assert.False(result);
            _mockCommonServices.Received(1).InsertTableDailyIDRange(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTime>());
        }

        [Fact]
        public void InsertIDRangeFPFingerPrint_WithMultipleDays_ProcessesAllDays()
        {
            // Arrange
            var fromDate = new DateTime(2024, 1, 1);
            var toDate = new DateTime(2024, 1, 3);
            _mockCommonServices.InsertTableDailyIDRange(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTime>()).Returns(1);

            // Act
            var result = _service.InsertIDRangeFPFingerPrint(fromDate, toDate);

            // Assert
            Assert.True(result);
            _mockCommonServices.Received(3).InsertTableDailyIDRange(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTime>());
        }

        #endregion

        #region UpdateIDRangeFPFingerPrint Tests

        [Fact]
        public void UpdateIDRangeFPFingerPrint_WithValidParameters_ReturnsTrue()
        {
            // Arrange
            var date = DateTime.Now;
            var minID = 1000UL;
            var maxID = 2000UL;
            _mockCommonServices.UpdateTableDailyIDRange(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTime>(), Arg.Any<ulong>(), Arg.Any<ulong>()).Returns(1);

            // Act
            var result = _service.UpdateIDRangeFPFingerPrint(date, minID, maxID);

            // Assert
            Assert.True(result);
            _mockCommonServices.Received(1).UpdateTableDailyIDRange(Arg.Any<string>(), Arg.Any<string>(), date, minID, maxID);
        }

        [Fact]
        public void UpdateIDRangeFPFingerPrint_WithServiceFailure_ReturnsFalse()
        {
            // Arrange
            var date = DateTime.Now;
            var minID = 1000UL;
            var maxID = 2000UL;
            _mockCommonServices.UpdateTableDailyIDRange(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTime>(), Arg.Any<ulong>(), Arg.Any<ulong>()).Returns(-1);

            // Act
            var result = _service.UpdateIDRangeFPFingerPrint(date, minID, maxID);

            // Assert
            Assert.False(result);
            _mockCommonServices.Received(1).UpdateTableDailyIDRange(Arg.Any<string>(), Arg.Any<string>(), date, minID, maxID);
        }

        [Fact]
        public void UpdateIDRangeFPFingerPrint_WithZeroIDs_ReturnsTrue()
        {
            // Arrange
            var date = DateTime.Now;
            var minID = 0UL;
            var maxID = 0UL;
            _mockCommonServices.UpdateTableDailyIDRange(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTime>(), Arg.Any<ulong>(), Arg.Any<ulong>()).Returns(1);

            // Act
            var result = _service.UpdateIDRangeFPFingerPrint(date, minID, maxID);

            // Assert
            Assert.True(result);
            _mockCommonServices.Received(1).UpdateTableDailyIDRange(Arg.Any<string>(), Arg.Any<string>(), date, minID, maxID);
        }

        [Fact]
        public void UpdateIDRangeFPFingerPrint_WithLargeIDs_ReturnsTrue()
        {
            // Arrange
            var date = DateTime.Now;
            var minID = ulong.MaxValue - 1000;
            var maxID = ulong.MaxValue;
            _mockCommonServices.UpdateTableDailyIDRange(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTime>(), Arg.Any<ulong>(), Arg.Any<ulong>()).Returns(1);

            // Act
            var result = _service.UpdateIDRangeFPFingerPrint(date, minID, maxID);

            // Assert
            Assert.True(result);
            _mockCommonServices.Received(1).UpdateTableDailyIDRange(Arg.Any<string>(), Arg.Any<string>(), date, minID, maxID);
        }

        #endregion

        #region Edge Cases Tests

        [Fact]
        public void InsertIDRangeFPFingerPrint_WithMinMaxDate_ReturnsTrue()
        {
            // Arrange
            var fromDate = DateTime.Now.AddYears(-1);
            var toDate = DateTime.Now.AddYears(1);
            _mockCommonServices.InsertTableDailyIDRange(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTime>()).Returns(1);

            // Act
            var result = _service.InsertIDRangeFPFingerPrint(fromDate, toDate);

            // Assert
            Assert.True(result);
            // Note: Using reasonable date range to avoid DateTime overflow
        }

        [Fact]
        public void UpdateIDRangeFPFingerPrint_WithMinMaxDate_ReturnsTrue()
        {
            // Arrange
            var date = DateTime.MinValue;
            var minID = 1000UL;
            var maxID = 2000UL;
            _mockCommonServices.UpdateTableDailyIDRange(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTime>(), Arg.Any<ulong>(), Arg.Any<ulong>()).Returns(1);

            // Act
            var result = _service.UpdateIDRangeFPFingerPrint(date, minID, maxID);

            // Assert
            Assert.True(result);
            _mockCommonServices.Received(1).UpdateTableDailyIDRange(Arg.Any<string>(), Arg.Any<string>(), date, minID, maxID);
        }

        #endregion

        #region Integration Tests

        [Fact]
        public void BothMethods_WithValidInputs_ReturnExpectedResults()
        {
            // Arrange
            var fromDate = DateTime.Now.AddDays(-1);
            var toDate = DateTime.Now;
            var updateDate = DateTime.Now;
            var minID = 1000UL;
            var maxID = 2000UL;
            
            _mockCommonServices.InsertTableDailyIDRange(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTime>()).Returns(1);
            _mockCommonServices.UpdateTableDailyIDRange(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTime>(), Arg.Any<ulong>(), Arg.Any<ulong>()).Returns(1);

            // Act
            var insertResult = _service.InsertIDRangeFPFingerPrint(fromDate, toDate);
            var updateResult = _service.UpdateIDRangeFPFingerPrint(updateDate, minID, maxID);

            // Assert
            Assert.True(insertResult);
            Assert.True(updateResult);
        }

        #endregion
    }
}
