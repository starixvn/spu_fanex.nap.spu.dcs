using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Fanex.NAP.SPU.DCS.JobService.Controllers;
using Fanex.NAP.SPU.DCS.JobService.Services;
using Xunit;
using System;
using System.Threading.Tasks;

namespace Fanex.NAP.SPU.DCS.Test.JobService.Controllers
{
    public class DCSExtraJobControllerTests
    {
        private readonly IUserAgentJobService _mockUserAgentJobService;
        private readonly ITransactionJobService _mockTransactionJobService;
        private readonly IDeviceJobService _mockDeviceJobService;
        private readonly IArchiveJobServices _mockArchiveJobServices;
        private readonly DCSExtraJobController _controller;

        public DCSExtraJobControllerTests()
        {
            _mockUserAgentJobService = Substitute.For<IUserAgentJobService>();
            _mockTransactionJobService = Substitute.For<ITransactionJobService>();
            _mockDeviceJobService = Substitute.For<IDeviceJobService>();
            _mockArchiveJobServices = Substitute.For<IArchiveJobServices>();

            _controller = new DCSExtraJobController(
                _mockUserAgentJobService,
                _mockTransactionJobService,
                _mockDeviceJobService,
                _mockArchiveJobServices);
        }

        #region UserAgent Tests

        [Fact]
        public async Task UserAgent_WithValidSize_ReturnsTrue()
        {
            // Arrange
            var size = 1000;
            _mockUserAgentJobService.Execute(size, true).Returns(true);

            // Act
            var result = await _controller.UserAgent(size);

            // Assert
            Assert.True(result);
            await _mockUserAgentJobService.Received(1).Execute(size, true);
        }

        [Fact]
        public async Task UserAgent_WithZeroSize_ReturnsFalse()
        {
            // Arrange
            var size = 0;
            _mockUserAgentJobService.Execute(size, true).Returns(false);

            // Act
            var result = await _controller.UserAgent(size);

            // Assert
            Assert.False(result);
            await _mockUserAgentJobService.Received(1).Execute(size, true);
        }

        [Fact]
        public async Task UserAgent_WithNegativeSize_ReturnsFalse()
        {
            // Arrange
            var size = -100;
            _mockUserAgentJobService.Execute(size, true).Returns(false);

            // Act
            var result = await _controller.UserAgent(size);

            // Assert
            Assert.False(result);
            await _mockUserAgentJobService.Received(1).Execute(size, true);
        }

        #endregion

        #region TransformTransaction Tests

        [Fact]
        public async Task TransformTransaction_WithValidParameters_ReturnsTrue()
        {
            // Arrange
            var size = 1000;
            var numberOfBatch = 5;
            _mockTransactionJobService.Execute(size, numberOfBatch, true).Returns(true);

            // Act
            var result = await _controller.TransformTransaction(size, numberOfBatch);

            // Assert
            Assert.True(result);
            await _mockTransactionJobService.Received(1).Execute(size, numberOfBatch, true);
        }

        [Fact]
        public async Task TransformTransaction_WithZeroParameters_ReturnsFalse()
        {
            // Arrange
            var size = 0;
            var numberOfBatch = 0;
            _mockTransactionJobService.Execute(size, numberOfBatch, true).Returns(false);

            // Act
            var result = await _controller.TransformTransaction(size, numberOfBatch);

            // Assert
            Assert.False(result);
            await _mockTransactionJobService.Received(1).Execute(size, numberOfBatch, true);
        }

        [Fact]
        public async Task TransformTransaction_WithLargeParameters_ReturnsTrue()
        {
            // Arrange
            var size = 100000;
            var numberOfBatch = 100;
            _mockTransactionJobService.Execute(size, numberOfBatch, true).Returns(true);

            // Act
            var result = await _controller.TransformTransaction(size, numberOfBatch);

            // Assert
            Assert.True(result);
            await _mockTransactionJobService.Received(1).Execute(size, numberOfBatch, true);
        }

        #endregion

        #region TransformDevice Tests

        [Fact]
        public async Task TransformDevice_WithValidParameters_ReturnsTrue()
        {
            // Arrange
            var size = 1000;
            var numberOfBatch = 5;
            _mockDeviceJobService.Execute(size, numberOfBatch, true).Returns(true);

            // Act
            var result = await _controller.TransformDevice(size, numberOfBatch);

            // Assert
            Assert.True(result);
            await _mockDeviceJobService.Received(1).Execute(size, numberOfBatch, true);
        }

        [Fact]
        public async Task TransformDevice_WithZeroParameters_ReturnsFalse()
        {
            // Arrange
            var size = 0;
            var numberOfBatch = 0;
            _mockDeviceJobService.Execute(size, numberOfBatch, true).Returns(false);

            // Act
            var result = await _controller.TransformDevice(size, numberOfBatch);

            // Assert
            Assert.False(result);
            await _mockDeviceJobService.Received(1).Execute(size, numberOfBatch, true);
        }

        [Fact]
        public async Task TransformDevice_WithSingleBatch_ReturnsTrue()
        {
            // Arrange
            var size = 1000;
            var numberOfBatch = 1;
            _mockDeviceJobService.Execute(size, numberOfBatch, true).Returns(true);

            // Act
            var result = await _controller.TransformDevice(size, numberOfBatch);

            // Assert
            Assert.True(result);
            await _mockDeviceJobService.Received(1).Execute(size, numberOfBatch, true);
        }

        #endregion

        #region UpdateModifiedDate Tests

        [Fact]
        public async Task UpdateModifiedDate_WithValidBatchSize_ReturnsTrue()
        {
            // Arrange
            var batchSize = 1000;
            _mockArchiveJobServices.Execute(batchSize, true).Returns(true);

            // Act
            var result = await _controller.UpdateModifiedDate(batchSize);

            // Assert
            Assert.True(result);
            await _mockArchiveJobServices.Received(1).Execute(batchSize, true);
        }

        [Fact]
        public async Task UpdateModifiedDate_WithZeroBatchSize_ReturnsFalse()
        {
            // Arrange
            var batchSize = 0;
            _mockArchiveJobServices.Execute(batchSize, true).Returns(false);

            // Act
            var result = await _controller.UpdateModifiedDate(batchSize);

            // Assert
            Assert.False(result);
            await _mockArchiveJobServices.Received(1).Execute(batchSize, true);
        }

        [Fact]
        public async Task UpdateModifiedDate_WithLargeBatchSize_ReturnsTrue()
        {
            // Arrange
            var batchSize = 100000;
            _mockArchiveJobServices.Execute(batchSize, true).Returns(true);

            // Act
            var result = await _controller.UpdateModifiedDate(batchSize);

            // Assert
            Assert.True(result);
            await _mockArchiveJobServices.Received(1).Execute(batchSize, true);
        }

        #endregion

        #region DeleteBotComponent Tests

        [Fact]
        public async Task DeleteBotComponent_WithValidRetentionDays_ReturnsTrue()
        {
            // Arrange
            var retentionDays = 30;
            _mockArchiveJobServices.DeleteBotComponent(retentionDays, true).Returns(true);

            // Act
            var result = await _controller.DeleteBotComponent(retentionDays);

            // Assert
            Assert.True(result);
            await _mockArchiveJobServices.Received(1).DeleteBotComponent(retentionDays, true);
        }

        [Fact]
        public async Task DeleteBotComponent_WithZeroRetentionDays_ReturnsFalse()
        {
            // Arrange
            var retentionDays = 0;
            _mockArchiveJobServices.DeleteBotComponent(retentionDays, true).Returns(false);

            // Act
            var result = await _controller.DeleteBotComponent(retentionDays);

            // Assert
            Assert.False(result);
            await _mockArchiveJobServices.Received(1).DeleteBotComponent(retentionDays, true);
        }

        [Fact]
        public async Task DeleteBotComponent_WithNegativeRetentionDays_ReturnsFalse()
        {
            // Arrange
            var retentionDays = -10;
            _mockArchiveJobServices.DeleteBotComponent(retentionDays, true).Returns(false);

            // Act
            var result = await _controller.DeleteBotComponent(retentionDays);

            // Assert
            Assert.False(result);
            await _mockArchiveJobServices.Received(1).DeleteBotComponent(retentionDays, true);
        }

        [Fact]
        public async Task DeleteBotComponent_WithLargeRetentionDays_ReturnsTrue()
        {
            // Arrange
            var retentionDays = 365;
            _mockArchiveJobServices.DeleteBotComponent(retentionDays, true).Returns(true);

            // Act
            var result = await _controller.DeleteBotComponent(retentionDays);

            // Assert
            Assert.True(result);
            await _mockArchiveJobServices.Received(1).DeleteBotComponent(retentionDays, true);
        }

        #endregion

        #region Edge Cases Tests

        [Fact]
        public async Task UserAgent_WithException_ThrowsException()
        {
            // Arrange
            var size = 1000;
            _mockUserAgentJobService.Execute(size, true).Returns(Task.FromException<bool>(new Exception("Test exception")));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _controller.UserAgent(size));
            await _mockUserAgentJobService.Received(1).Execute(size, true);
        }

        [Fact]
        public async Task TransformTransaction_WithException_ThrowsException()
        {
            // Arrange
            var size = 1000;
            var numberOfBatch = 5;
            _mockTransactionJobService.Execute(size, numberOfBatch, true).Returns(Task.FromException<bool>(new Exception("Test exception")));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _controller.TransformTransaction(size, numberOfBatch));
            await _mockTransactionJobService.Received(1).Execute(size, numberOfBatch, true);
        }

        [Fact]
        public async Task TransformDevice_WithException_ThrowsException()
        {
            // Arrange
            var size = 1000;
            var numberOfBatch = 5;
            _mockDeviceJobService.Execute(size, numberOfBatch, true).Returns(Task.FromException<bool>(new Exception("Test exception")));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _controller.TransformDevice(size, numberOfBatch));
            await _mockDeviceJobService.Received(1).Execute(size, numberOfBatch, true);
        }

        [Fact]
        public async Task UpdateModifiedDate_WithException_ThrowsException()
        {
            // Arrange
            var batchSize = 1000;
            _mockArchiveJobServices.Execute(batchSize, true).Returns(Task.FromException<bool>(new Exception("Test exception")));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _controller.UpdateModifiedDate(batchSize));
            await _mockArchiveJobServices.Received(1).Execute(batchSize, true);
        }

        [Fact]
        public async Task DeleteBotComponent_WithException_ThrowsException()
        {
            // Arrange
            var retentionDays = 30;
            _mockArchiveJobServices.DeleteBotComponent(retentionDays, true).Returns(Task.FromException<bool>(new Exception("Test exception")));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _controller.DeleteBotComponent(retentionDays));
            await _mockArchiveJobServices.Received(1).DeleteBotComponent(retentionDays, true);
        }

        #endregion

        #region Boundary Tests

        [Fact]
        public async Task UserAgent_WithMaxIntSize_ReturnsTrue()
        {
            // Arrange
            var size = int.MaxValue;
            _mockUserAgentJobService.Execute(size, true).Returns(true);

            // Act
            var result = await _controller.UserAgent(size);

            // Assert
            Assert.True(result);
            await _mockUserAgentJobService.Received(1).Execute(size, true);
        }

        [Fact]
        public async Task TransformTransaction_WithMaxIntParameters_ReturnsTrue()
        {
            // Arrange
            var size = int.MaxValue;
            var numberOfBatch = int.MaxValue;
            _mockTransactionJobService.Execute(size, numberOfBatch, true).Returns(true);

            // Act
            var result = await _controller.TransformTransaction(size, numberOfBatch);

            // Assert
            Assert.True(result);
            await _mockTransactionJobService.Received(1).Execute(size, numberOfBatch, true);
        }

        [Fact]
        public async Task TransformDevice_WithMaxIntParameters_ReturnsTrue()
        {
            // Arrange
            var size = int.MaxValue;
            var numberOfBatch = int.MaxValue;
            _mockDeviceJobService.Execute(size, numberOfBatch, true).Returns(true);

            // Act
            var result = await _controller.TransformDevice(size, numberOfBatch);

            // Assert
            Assert.True(result);
            await _mockDeviceJobService.Received(1).Execute(size, numberOfBatch, true);
        }

        [Fact]
        public async Task UpdateModifiedDate_WithMaxIntBatchSize_ReturnsTrue()
        {
            // Arrange
            var batchSize = int.MaxValue;
            _mockArchiveJobServices.Execute(batchSize, true).Returns(true);

            // Act
            var result = await _controller.UpdateModifiedDate(batchSize);

            // Assert
            Assert.True(result);
            await _mockArchiveJobServices.Received(1).Execute(batchSize, true);
        }

        [Fact]
        public async Task DeleteBotComponent_WithMaxIntRetentionDays_ReturnsTrue()
        {
            // Arrange
            var retentionDays = int.MaxValue;
            _mockArchiveJobServices.DeleteBotComponent(retentionDays, true).Returns(true);

            // Act
            var result = await _controller.DeleteBotComponent(retentionDays);

            // Assert
            Assert.True(result);
            await _mockArchiveJobServices.Received(1).DeleteBotComponent(retentionDays, true);
        }

        #endregion
    }
}
