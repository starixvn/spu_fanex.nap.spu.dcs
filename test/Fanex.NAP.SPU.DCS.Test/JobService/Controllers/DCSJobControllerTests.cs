using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Fanex.NAP.SPU.DCS.JobService.Controllers;
using Fanex.NAP.SPU.DCS.JobService.Services;
using Fanex.NAP.SPU.DCS.Features.SystemSetting;
using UAParser.FormFactor.Models;
using Xunit;
using System;
using System.Threading.Tasks;

namespace Fanex.NAP.SPU.DCS.Test.JobService.Controllers
{
    public class DCSJobControllerTests
    {
        private readonly DCSTransactionJobService _mockTransactionJobServices;
        private readonly IUserAgentJobService _mockUserAgentJobService;
        private readonly IDeviceJobService _mockDeviceJobService;
        private readonly IArchiveJobServices _mockArchiveJobService;
        private readonly ISystemSettingServices _mockSystemSettingServices;
        private readonly IMBDeviceJobService _mockMBDeviceJobService;
        private readonly ICommonJobService _mockCommonJobService;
        private readonly DCSJobController _controller;

        public DCSJobControllerTests()
        {
            var mockTransactionJobService = Substitute.For<ITransactionJobService>();
            var mockSumTransactionJobService = Substitute.For<ISumTransactionJobService>();
            var mockAbnormalTransactionJobService = Substitute.For<IAbnormalTransactionJobService>();
            var mockMBTransactionJobService = Substitute.For<IMBTransactionJobService>();
            
            _mockTransactionJobServices = new DCSTransactionJobService(
                mockTransactionJobService,
                mockSumTransactionJobService,
                mockAbnormalTransactionJobService,
                mockMBTransactionJobService);
                
            _mockUserAgentJobService = Substitute.For<IUserAgentJobService>();
            _mockDeviceJobService = Substitute.For<IDeviceJobService>();
            _mockArchiveJobService = Substitute.For<IArchiveJobServices>();
            _mockSystemSettingServices = Substitute.For<ISystemSettingServices>();
            _mockMBDeviceJobService = Substitute.For<IMBDeviceJobService>();
            _mockCommonJobService = Substitute.For<ICommonJobService>();

            _controller = new DCSJobController(
                _mockTransactionJobServices,
                _mockUserAgentJobService,
                _mockDeviceJobService,
                _mockArchiveJobService,
                _mockSystemSettingServices,
                _mockMBDeviceJobService,
                _mockCommonJobService);
        }

        #region UserAgent Tests

        [Fact]
        public async Task UserAgent_WithValidSize_ReturnsTrue()
        {
            // Arrange
            var size = 1000;
            _mockUserAgentJobService.Execute(size).Returns(true);

            // Act
            var result = await _controller.UserAgent(size);

            // Assert
            Assert.True(result);
            await _mockUserAgentJobService.Received(1).Execute(size);
        }

        [Fact]
        public async Task UserAgent_WithZeroSize_ReturnsFalse()
        {
            // Arrange
            var size = 0;
            _mockUserAgentJobService.Execute(size).Returns(false);

            // Act
            var result = await _controller.UserAgent(size);

            // Assert
            Assert.False(result);
            await _mockUserAgentJobService.Received(1).Execute(size);
        }

        #endregion

        #region ConvertUserAgent Tests

        [Fact]
        public void ConvertUserAgent_WithValidUserAgent_ReturnsClientInfo()
        {
            // Arrange
            var userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36";
            var expectedClientInfo = new ClientInfo("test", new OS("Windows", "10", "x64", "10.0", "19044"), new Device("Desktop", "Desktop", "Desktop", DeviceFormFactor.Desktop), new UserAgent("Chrome", "100", "100", "537.36"));
            _mockUserAgentJobService.ConvertUserAgent(userAgent).Returns(expectedClientInfo);

            // Act
            var result = _controller.ConvertUserAgent(userAgent);

            // Assert
            Assert.Equal(expectedClientInfo, result);
            _mockUserAgentJobService.Received(1).ConvertUserAgent(userAgent);
        }

        [Fact]
        public void ConvertUserAgent_WithEmptyUserAgent_ReturnsClientInfo()
        {
            // Arrange
            var userAgent = "";
            var expectedClientInfo = new ClientInfo("test", new OS("Windows", "10", "x64", "10.0", "19044"), new Device("Desktop", "Desktop", "Desktop", DeviceFormFactor.Desktop), new UserAgent("Chrome", "100", "100", "537.36"));
            _mockUserAgentJobService.ConvertUserAgent(userAgent).Returns(expectedClientInfo);

            // Act
            var result = _controller.ConvertUserAgent(userAgent);

            // Assert
            Assert.Equal(expectedClientInfo, result);
            _mockUserAgentJobService.Received(1).ConvertUserAgent(userAgent);
        }

        #endregion

        #region TransformTransaction Tests

        [Fact]
        public async Task TransformTransaction_WithValidParameters_ReturnsTrue()
        {
            // Arrange
            var size = 1000;
            var numberOfBatch = 5;
            _mockTransactionJobServices.TransactionJobService.Execute(size, numberOfBatch).Returns(true);

            // Act
            var result = await _controller.TransformTransaction(size, numberOfBatch);

            // Assert
            Assert.True(result);
            await _mockTransactionJobServices.TransactionJobService.Received(1).Execute(size, numberOfBatch);
        }

        [Fact]
        public async Task TransformTransaction_WithZeroParameters_ReturnsFalse()
        {
            // Arrange
            var size = 0;
            var numberOfBatch = 0;
            _mockTransactionJobServices.TransactionJobService.Execute(size, numberOfBatch).Returns(false);

            // Act
            var result = await _controller.TransformTransaction(size, numberOfBatch);

            // Assert
            Assert.False(result);
            await _mockTransactionJobServices.TransactionJobService.Received(1).Execute(size, numberOfBatch);
        }

        #endregion

        #region TransformDevice Tests

        [Fact]
        public async Task TransformDevice_WithValidParameters_ReturnsTrue()
        {
            // Arrange
            var size = 1000;
            var numberOfBatch = 5;
            _mockDeviceJobService.Execute(size, numberOfBatch).Returns(true);

            // Act
            var result = await _controller.TransformDevice(size, numberOfBatch);

            // Assert
            Assert.True(result);
            await _mockDeviceJobService.Received(1).Execute(size, numberOfBatch);
        }

        #endregion

        #region SumTransaction Tests

        [Fact]
        public async Task SumTransaction_ReturnsTrue()
        {
            // Arrange
            _mockTransactionJobServices.SumTransactionJobService.Execute().Returns(true);

            // Act
            var result = await _controller.SumTransaction();

            // Assert
            Assert.True(result);
            await _mockTransactionJobServices.SumTransactionJobService.Received(1).Execute();
        }

        #endregion

        #region PushAbnormalTransaction Tests

        [Fact]
        public async Task PushAbnormalTransaction_WithValidSize_ReturnsTrue()
        {
            // Arrange
            var size = 1000;
            _mockTransactionJobServices.AbnormalTransactionJobService.PushAbnormalTransaction(size).Returns(true);

            // Act
            var result = await _controller.PushAbnormalTransaction(size);

            // Assert
            Assert.True(result);
            await _mockTransactionJobServices.AbnormalTransactionJobService.Received(1).PushAbnormalTransaction(size);
        }

        #endregion

        #region UpdateModifiedDate Tests

        [Fact]
        public async Task UpdateModifiedDate_WithValidBatchSize_ReturnsTrue()
        {
            // Arrange
            var batchSize = 1000;
            _mockArchiveJobService.Execute(batchSize).Returns(true);

            // Act
            var result = await _controller.UpdateModifiedDate(batchSize);

            // Assert
            Assert.True(result);
            await _mockArchiveJobService.Received(1).Execute(batchSize);
        }

        #endregion

        #region UpdateDCSystemSettingByID Tests

        [Fact]
        public void UpdateDCSystemSettingByID_WithValidParameters_ReturnsTrue()
        {
            // Arrange
            var id = 1;
            var value = "test_value";
            _mockSystemSettingServices.UpdateDCSystemSettingByID(id, value).Returns(true);

            // Act
            var result = _controller.UpdateDCSystemSettingByID(id, value);

            // Assert
            Assert.True(result);
            _mockSystemSettingServices.Received(1).UpdateDCSystemSettingByID(id, value);
        }

        [Fact]
        public void UpdateDCSystemSettingByID_WithInvalidId_ReturnsFalse()
        {
            // Arrange
            var id = -1;
            var value = "test_value";
            _mockSystemSettingServices.UpdateDCSystemSettingByID(id, value).Returns(false);

            // Act
            var result = _controller.UpdateDCSystemSettingByID(id, value);

            // Assert
            Assert.False(result);
            _mockSystemSettingServices.Received(1).UpdateDCSystemSettingByID(id, value);
        }

        #endregion

        #region DeleteBotComponent Tests

        [Fact]
        public async Task DeleteBotComponent_WithValidRetentionDays_ReturnsTrue()
        {
            // Arrange
            var retentionDays = 30;
            _mockArchiveJobService.DeleteBotComponent(retentionDays).Returns(true);

            // Act
            var result = await _controller.DeleteBotComponent(retentionDays);

            // Assert
            Assert.True(result);
            await _mockArchiveJobService.Received(1).DeleteBotComponent(retentionDays);
        }

        #endregion

        #region InsertIDRangeFPFingerPrint Tests

        [Fact]
        public void InsertIDRangeFPFingerPrint_WithValidDates_ReturnsTrue()
        {
            // Arrange
            var fromDate = DateTime.Now.AddDays(-1);
            var toDate = DateTime.Now;
            _mockCommonJobService.InsertIDRangeFPFingerPrint(fromDate, toDate).Returns(true);

            // Act
            var result = _controller.InsertIDRangeFPFingerPrint(fromDate, toDate);

            // Assert
            Assert.True(result);
            _mockCommonJobService.Received(1).InsertIDRangeFPFingerPrint(fromDate, toDate);
        }

        [Fact]
        public void InsertIDRangeFPFingerPrint_WithNullDates_UsesDefaultDates()
        {
            // Arrange
            DateTime? fromDate = null;
            DateTime? toDate = null;
            _mockCommonJobService.InsertIDRangeFPFingerPrint(Arg.Any<DateTime>(), Arg.Any<DateTime>()).Returns(true);

            // Act
            var result = _controller.InsertIDRangeFPFingerPrint(fromDate, toDate);

            // Assert
            Assert.True(result);
            _mockCommonJobService.Received(1).InsertIDRangeFPFingerPrint(Arg.Any<DateTime>(), Arg.Any<DateTime>());
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
            _mockCommonJobService.UpdateIDRangeFPFingerPrint(date, minID, maxID).Returns(true);

            // Act
            var result = _controller.UpdateIDRangeFPFingerPrint(date, minID, maxID);

            // Assert
            Assert.True(result);
            _mockCommonJobService.Received(1).UpdateIDRangeFPFingerPrint(date, minID, maxID);
        }

        #endregion

        #region TransformRawFingerprintQueue Tests

        [Fact]
        public void TransformRawFingerprintQueue_WithValidBatchSize_ReturnsTrue()
        {
            // Arrange
            var batchSize = 1000;
            _mockTransactionJobServices.TransactionJobService.ProcessRawFingerprintQueue(batchSize).Returns(true);

            // Act
            var result = _controller.TransformRawFingerprintQueue(batchSize);

            // Assert
            Assert.True(result);
            _mockTransactionJobServices.TransactionJobService.Received(1).ProcessRawFingerprintQueue(batchSize);
        }

        [Fact]
        public void TransformRawFingerprintQueue_WithZeroBatchSize_ReturnsFalse()
        {
            // Arrange
            var batchSize = 0;
            _mockTransactionJobServices.TransactionJobService.ProcessRawFingerprintQueue(batchSize).Returns(false);

            // Act
            var result = _controller.TransformRawFingerprintQueue(batchSize);

            // Assert
            Assert.False(result);
            _mockTransactionJobServices.TransactionJobService.Received(1).ProcessRawFingerprintQueue(batchSize);
        }

        [Fact]
        public void TransformRawFingerprintQueue_WithNegativeBatchSize_ReturnsFalse()
        {
            // Arrange
            var batchSize = -1;
            _mockTransactionJobServices.TransactionJobService.ProcessRawFingerprintQueue(batchSize).Returns(false);

            // Act
            var result = _controller.TransformRawFingerprintQueue(batchSize);

            // Assert
            Assert.False(result);
            _mockTransactionJobServices.TransactionJobService.Received(1).ProcessRawFingerprintQueue(batchSize);
        }

        #endregion

        #region Mobile App Tests

        [Fact]
        public async Task MBTransformTransaction_WithValidParameters_ReturnsTrue()
        {
            // Arrange
            var batchSize = 1000;
            var numberOfBatch = 5;
            _mockTransactionJobServices.MBTransactionJobService.Execute(batchSize, numberOfBatch).Returns(true);

            // Act
            var result = await _controller.MBTransformTransaction(batchSize, numberOfBatch);

            // Assert
            Assert.True(result);
            await _mockTransactionJobServices.MBTransactionJobService.Received(1).Execute(batchSize, numberOfBatch);
        }

        [Fact]
        public async Task MBTransformDevice_WithValidParameters_ReturnsTrue()
        {
            // Arrange
            var limitTotalRecord = 1000;
            var numberOfBatch = 5;
            _mockMBDeviceJobService.Execute(limitTotalRecord, numberOfBatch).Returns(true);

            // Act
            var result = await _controller.MBTransformDevice(limitTotalRecord, numberOfBatch);

            // Assert
            Assert.True(result);
            await _mockMBDeviceJobService.Received(1).Execute(limitTotalRecord, numberOfBatch);
        }

        [Fact]
        public async Task UpdateMBLastAccessedDate_WithValidBatchSize_ReturnsTrue()
        {
            // Arrange
            var batchSize = 1000;
            _mockArchiveJobService.UpdateMBLastAccessedDate(batchSize).Returns(true);

            // Act
            var result = await _controller.UpdateMBLastAccessedDate(batchSize);

            // Assert
            Assert.True(result);
            await _mockArchiveJobService.Received(1).UpdateMBLastAccessedDate(batchSize);
        }

        #endregion
    }
}
