
namespace Fanex.NAP.SPU.DCS.Tests.JobService.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.NAP.SPU.DCS.Features.MBDevice;
    using Fanex.NAP.SPU.DCS.Features.MBDevice.Entities;
    using Fanex.NAP.SPU.DCS.JobService.Services;
    using NSubstitute;
    using Xunit;

    public class MBDeviceJobServiceTest
    {
        private readonly ILogService logService;
        private readonly IMBDeviceServices deviceService;
        private readonly MBDeviceJobService mbDeviceJobService;

        public MBDeviceJobServiceTest()
        {
            logService = Substitute.For<ILogService>();
            deviceService = Substitute.For<IMBDeviceServices>();
            mbDeviceJobService = new MBDeviceJobService(logService, deviceService);
        }

        [Fact]
        public async Task Execute_WithValidParameters_ReturnsTrue()
        {
            // Arrange
            int limitTotalRecord = 10;
            int numberOfBatch = 2;

            // Act
            var result = await mbDeviceJobService.Execute(limitTotalRecord, numberOfBatch);

            // Assert
            Assert.True(result);
            await logService.DidNotReceiveWithAnyArgs().ErrorAsync(Arg.Any<Exception>());
        }

        [Fact]
        public async Task Execute_WithInvalidParameters_ReturnsFalseAndLogsError()
        {
            // Arrange
            int limitTotalRecord = 0;
            int numberOfBatch = 0;

            // Act
            var result = await mbDeviceJobService.Execute(limitTotalRecord, numberOfBatch);

            // Assert
            Assert.False(result);
            await logService.Received(1).ErrorAsync(Arg.Any<Exception>());
        }

        [Fact]
        public async Task Execute_WithException_ReturnsFalseAndLogsError()
        {
            // Arrange
            int limitTotalRecord = 10;
            int numberOfBatch = 2;

            deviceService.When(x => x.GetMBTransaction(limitTotalRecord, numberOfBatch))
                         .Do(x => { throw new Exception("Test exception"); });

            // Act
            var result = await mbDeviceJobService.Execute(limitTotalRecord, numberOfBatch);

            // Assert
            Assert.False(result);
            await logService.Received(1).ErrorAsync(Arg.Any<Exception>());
        }

        [Fact]
        public async Task MBTransformDevice_NullTransactionList_ReturnsWithoutProcessing()
        {
            // Arrange
            deviceService.GetMBTransaction(100, 2).Returns((List<MBTransactionEntity>)null);

            // Act
            await mbDeviceJobService.Execute(100, 2);

            // Assert
            deviceService.DidNotReceive().TransformDeviceInsert(Arg.Any<string>());
        }
    }
}