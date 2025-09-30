namespace Fanex.NAP.SPU.DCS.Tests.JobService.Services
{
    using Fanex.NAP.SPU.DCS.Features.Devices;
    using Fanex.NAP.SPU.DCS.Features.Devices.Entities;
    using Fanex.NAP.SPU.DCS.JobService.Services;
    using NSubstitute;
    using NSubstitute.ReceivedExtensions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Xunit;

    public class DeviceJobServiceTests
    {
        private ILogService logService;
        private IDeviceServices deviceService;
        private HttpClient httpClient;

        [Fact]
        public async Task TransformDevice_WhenSuccessAndIsDeviceRecover_ShouldCallExactFunctionAndExactParams()
        {
            // Arrange
            var stagingTransIds = GetStagingTrans();
            var deviceJobService = GetService();

            deviceService.GetStagingTransIdForDeviceAsync(Arg.Any<int>(), Arg.Any<int>()).Returns(stagingTransIds);

            // Act
            var result = await deviceJobService.Execute(1, 1);

            // Assert
            foreach (var group in stagingTransIds.GroupBy(x => x.BatchId))
            {
                await deviceService.Received(1).TransformDeviceInsertAsync(Arg.Is<IEnumerable<StagingTransId>>(transIds => transIds.SequenceEqual(group)));
            }
            Assert.True(result);
        }

        [Fact]
        public async void TransformDevice_WhenGetStagingTransThrowError_ShouldCallExactSentry()
        {
            //Arrange
            var deviceJobService = GetService();

            deviceService.When(x => x.GetStagingTransIdForDeviceAsync(1, 1))
                            .Do(x => { throw new Exception("Sql exception"); });

            //Act
            var result = await deviceJobService.Execute(1, 1);

            //Assert
            await logService.Received().ErrorAsync(Arg.Any<Exception>());

            Assert.False(result);
        }

        [Fact]
        public async void TransformDevice_WhenTransformDeviceInsertAsyncThrowError_ShouldCallExactSentryWithTimes()
        {
            //Arrange
            var stagingTransIds = new List<StagingTransId>
            {
                new() {
                    BatchId = 1,
                    TransId = 1
                },
                new() {
                    BatchId = 1,
                    TransId = 2
                },
                new() {
                    BatchId = 2,
                    TransId = 2
                },
                new() {
                    BatchId = 2,
                    TransId = 3
                }
            };
            var deviceJobService = GetService();

            deviceService.GetStagingTransIdForDeviceAsync(Arg.Any<int>(), Arg.Any<int>()).Returns(stagingTransIds);

            deviceService.When(x => x.TransformDeviceInsertAsync(Arg.Any<IEnumerable<StagingTransId>>()))
                            .Do(x => { throw new Exception("Sql exception"); });

            //Act
            var result = await deviceJobService.Execute(1, 1);

            //Assert
            await logService.Received(3).ErrorAsync(Arg.Any<Exception>());

            Assert.False(result);
        }

        [Fact]
        public async Task TransformDevice_WhenSizeAndNumberOfBatchAreGreaterThanZero_ShouldCallTransformDevice()
        {
            // Arrange
            var deviceJobService = GetService();

            int size = 1;

            int numberOfBatch = 1;

            var stagingTransIds = GetStagingTrans();

            deviceService.GetStagingTransIdForDeviceAsync(Arg.Any<int>(), Arg.Any<int>()).Returns(stagingTransIds);

            // Act
            var result = await deviceJobService.Execute(size, numberOfBatch);

            // Assert
            await deviceService.Received(1).GetStagingTransIdForDeviceAsync(size, numberOfBatch);
            foreach (var group in stagingTransIds.GroupBy(x => x.BatchId))
            {
                await deviceService.Received(1).TransformDeviceInsertAsync(Arg.Is<IEnumerable<StagingTransId>>(transIds => transIds.SequenceEqual(group)));
            }
            Assert.True(result);
        }

        [Fact]
        public async Task Execute_WhenExceptionIsThrown_ShouldLogErrorAndReturnFalse()
        {
            // Arrange
            var deviceJobService = GetService();

            deviceService.When(x => x.GetStagingTransIdForDeviceAsync(Arg.Any<int>(), Arg.Any<int>()))
                         .Do(x => { throw new Exception("Test exception"); });

            // Act
            var result = await deviceJobService.Execute(1, 1);

            // Assert
            await logService.Received(1).ErrorAsync(Arg.Is<Exception>(ex => ex.Message == "Test exception"));
            Assert.False(result);
        }

        private static List<StagingTransId> GetStagingTrans()
        {
            return
            [
                new StagingTransId
                {
                    BatchId = 1,
                    TransId = 1
                },
                new StagingTransId
                {
                    BatchId = 1,
                    TransId = 2
                },
                new StagingTransId
                {
                    BatchId = 2,
                    TransId = 2
                },
                new StagingTransId
                {
                    BatchId = 2,
                    TransId = 3
                }
            ];
        }

        private DeviceJobService GetService()
        {
            logService = Substitute.For<ILogService>();
            deviceService = Substitute.For<IDeviceServices>();
            httpClient = Substitute.For<HttpClient>();
            return new DeviceJobService(logService, deviceService, httpClient);
        }

    }
}
