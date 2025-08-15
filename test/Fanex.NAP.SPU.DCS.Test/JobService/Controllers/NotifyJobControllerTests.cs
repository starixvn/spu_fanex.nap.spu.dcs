using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Fanex.NAP.SPU.DCS.JobService.Controllers;
using Fanex.NAP.SPU.DCS.JobService.Services;
using Xunit;
using System;
using System.Threading.Tasks;

namespace Fanex.NAP.SPU.DCS.Test.JobService.Controllers
{
    public class NotifyJobControllerTests
    {
        private readonly INotifyTransactionJobService _mockNotifyTransactionJobService;
        private readonly NotifyJobController _controller;

        public NotifyJobControllerTests()
        {
            _mockNotifyTransactionJobService = Substitute.For<INotifyTransactionJobService>();
            _controller = new NotifyJobController(_mockNotifyTransactionJobService);
        }

        #region Transaction Tests

        [Fact]
        public async Task Transaction_ReturnsTrue()
        {
            // Arrange
            _mockNotifyTransactionJobService.Execute().Returns(true);

            // Act
            var result = await _controller.Transaction();

            // Assert
            Assert.True(result);
            await _mockNotifyTransactionJobService.Received(1).Execute();
        }

        [Fact]
        public async Task Transaction_ReturnsFalse()
        {
            // Arrange
            _mockNotifyTransactionJobService.Execute().Returns(false);

            // Act
            var result = await _controller.Transaction();

            // Assert
            Assert.False(result);
            await _mockNotifyTransactionJobService.Received(1).Execute();
        }

        #endregion

        #region MonitorTransaction Tests

        [Fact]
        public async Task MonitorTransaction_WithValidParameters_ReturnsTrue()
        {
            // Arrange
            var rptDateStr = "2024-01-01";
            var batchSize = 1000;
            var limitShowCust = 100;
            _mockNotifyTransactionJobService.MonitorTransactionAndNotify(rptDateStr, batchSize, limitShowCust).Returns(true);

            // Act
            var result = await _controller.MonitorTransaction(rptDateStr, batchSize, limitShowCust);

            // Assert
            Assert.True(result);
            await _mockNotifyTransactionJobService.Received(1).MonitorTransactionAndNotify(rptDateStr, batchSize, limitShowCust);
        }

        [Fact]
        public async Task MonitorTransaction_WithEmptyDateString_ReturnsFalse()
        {
            // Arrange
            var rptDateStr = "";
            var batchSize = 1000;
            var limitShowCust = 100;
            _mockNotifyTransactionJobService.MonitorTransactionAndNotify(rptDateStr, batchSize, limitShowCust).Returns(false);

            // Act
            var result = await _controller.MonitorTransaction(rptDateStr, batchSize, limitShowCust);

            // Assert
            Assert.False(result);
            await _mockNotifyTransactionJobService.Received(1).MonitorTransactionAndNotify(rptDateStr, batchSize, limitShowCust);
        }

        [Fact]
        public async Task MonitorTransaction_WithZeroBatchSize_ReturnsFalse()
        {
            // Arrange
            var rptDateStr = "2024-01-01";
            var batchSize = 0;
            var limitShowCust = 100;
            _mockNotifyTransactionJobService.MonitorTransactionAndNotify(rptDateStr, batchSize, limitShowCust).Returns(false);

            // Act
            var result = await _controller.MonitorTransaction(rptDateStr, batchSize, limitShowCust);

            // Assert
            Assert.False(result);
            await _mockNotifyTransactionJobService.Received(1).MonitorTransactionAndNotify(rptDateStr, batchSize, limitShowCust);
        }

        [Fact]
        public async Task MonitorTransaction_WithZeroLimitShowCust_ReturnsFalse()
        {
            // Arrange
            var rptDateStr = "2024-01-01";
            var batchSize = 1000;
            var limitShowCust = 0;
            _mockNotifyTransactionJobService.MonitorTransactionAndNotify(rptDateStr, batchSize, limitShowCust).Returns(false);

            // Act
            var result = await _controller.MonitorTransaction(rptDateStr, batchSize, limitShowCust);

            // Assert
            Assert.False(result);
            await _mockNotifyTransactionJobService.Received(1).MonitorTransactionAndNotify(rptDateStr, batchSize, limitShowCust);
        }

        #endregion

        #region MessageByID Tests

        [Fact]
        public async Task MessageByID_WithValidNotifyMessageID_ReturnsTrue()
        {
            // Arrange
            var notifyMessageID = 123u;
            _mockNotifyTransactionJobService.NotifyMessageByID(notifyMessageID).Returns(true);

            // Act
            var result = await _controller.MessageByID(notifyMessageID);

            // Assert
            Assert.True(result);
            await _mockNotifyTransactionJobService.Received(1).NotifyMessageByID(notifyMessageID);
        }

        [Fact]
        public async Task MessageByID_WithZeroNotifyMessageID_ReturnsFalse()
        {
            // Arrange
            var notifyMessageID = 0u;
            _mockNotifyTransactionJobService.NotifyMessageByID(notifyMessageID).Returns(false);

            // Act
            var result = await _controller.MessageByID(notifyMessageID);

            // Assert
            Assert.False(result);
            await _mockNotifyTransactionJobService.Received(1).NotifyMessageByID(notifyMessageID);
        }

        [Fact]
        public async Task MessageByID_WithMaxUintNotifyMessageID_ReturnsTrue()
        {
            // Arrange
            var notifyMessageID = uint.MaxValue;
            _mockNotifyTransactionJobService.NotifyMessageByID(notifyMessageID).Returns(true);

            // Act
            var result = await _controller.MessageByID(notifyMessageID);

            // Assert
            Assert.True(result);
            await _mockNotifyTransactionJobService.Received(1).NotifyMessageByID(notifyMessageID);
        }

        #endregion

        #region MessageByNotifyType Tests

        [Fact]
        public async Task MessageByNotifyType_WithValidParameters_ReturnsTrue()
        {
            // Arrange
            var notifyType = "email";
            var batchSize = 1000;
            _mockNotifyTransactionJobService.NotifyMessageByNotifyType(notifyType, batchSize).Returns(true);

            // Act
            var result = await _controller.MessageByNotifyType(notifyType, batchSize);

            // Assert
            Assert.True(result);
            await _mockNotifyTransactionJobService.Received(1).NotifyMessageByNotifyType(notifyType, batchSize);
        }

        [Fact]
        public async Task MessageByNotifyType_WithEmptyNotifyType_ReturnsFalse()
        {
            // Arrange
            var notifyType = "";
            var batchSize = 1000;
            _mockNotifyTransactionJobService.NotifyMessageByNotifyType(notifyType, batchSize).Returns(false);

            // Act
            var result = await _controller.MessageByNotifyType(notifyType, batchSize);

            // Assert
            Assert.False(result);
            await _mockNotifyTransactionJobService.Received(1).NotifyMessageByNotifyType(notifyType, batchSize);
        }

        [Fact]
        public async Task MessageByNotifyType_WithNullNotifyType_ReturnsFalse()
        {
            // Arrange
            string notifyType = null;
            var batchSize = 1000;
            _mockNotifyTransactionJobService.NotifyMessageByNotifyType(notifyType, batchSize).Returns(false);

            // Act
            var result = await _controller.MessageByNotifyType(notifyType, batchSize);

            // Assert
            Assert.False(result);
            await _mockNotifyTransactionJobService.Received(1).NotifyMessageByNotifyType(notifyType, batchSize);
        }

        [Fact]
        public async Task MessageByNotifyType_WithZeroBatchSize_ReturnsFalse()
        {
            // Arrange
            var notifyType = "email";
            var batchSize = 0;
            _mockNotifyTransactionJobService.NotifyMessageByNotifyType(notifyType, batchSize).Returns(false);

            // Act
            var result = await _controller.MessageByNotifyType(notifyType, batchSize);

            // Assert
            Assert.False(result);
            await _mockNotifyTransactionJobService.Received(1).NotifyMessageByNotifyType(notifyType, batchSize);
        }

        [Fact]
        public async Task MessageByNotifyType_WithDifferentNotifyTypes_ReturnsCorrectResults()
        {
            // Arrange
            var notifyTypes = new[] { "email", "sms", "telegram", "push" };
            var batchSize = 1000;

            foreach (var notifyType in notifyTypes)
            {
                _mockNotifyTransactionJobService.NotifyMessageByNotifyType(notifyType, batchSize).Returns(true);

                // Act
                var result = await _controller.MessageByNotifyType(notifyType, batchSize);

                // Assert
                Assert.True(result);
                await _mockNotifyTransactionJobService.Received(1).NotifyMessageByNotifyType(notifyType, batchSize);
            }
        }

        #endregion

        #region Edge Cases Tests

        [Fact]
        public async Task Transaction_WithException_ThrowsException()
        {
            // Arrange
            _mockNotifyTransactionJobService.Execute().Returns(Task.FromException<bool>(new Exception("Test exception")));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _controller.Transaction());
            await _mockNotifyTransactionJobService.Received(1).Execute();
        }

        [Fact]
        public async Task MonitorTransaction_WithInvalidDateFormat_ReturnsFalse()
        {
            // Arrange
            var rptDateStr = "invalid-date";
            var batchSize = 1000;
            var limitShowCust = 100;
            _mockNotifyTransactionJobService.MonitorTransactionAndNotify(rptDateStr, batchSize, limitShowCust).Returns(false);

            // Act
            var result = await _controller.MonitorTransaction(rptDateStr, batchSize, limitShowCust);

            // Assert
            Assert.False(result);
            await _mockNotifyTransactionJobService.Received(1).MonitorTransactionAndNotify(rptDateStr, batchSize, limitShowCust);
        }

        [Fact]
        public async Task MessageByNotifyType_WithSpecialCharacters_ReturnsTrue()
        {
            // Arrange
            var notifyType = "email@domain.com";
            var batchSize = 1000;
            _mockNotifyTransactionJobService.NotifyMessageByNotifyType(notifyType, batchSize).Returns(true);

            // Act
            var result = await _controller.MessageByNotifyType(notifyType, batchSize);

            // Assert
            Assert.True(result);
            await _mockNotifyTransactionJobService.Received(1).NotifyMessageByNotifyType(notifyType, batchSize);
        }

        #endregion
    }
}
