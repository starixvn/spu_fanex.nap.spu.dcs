namespace Fanex.NAP.SPU.DCS.Tests.JobService.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using AutoFixture;
    using Fanex.NAP.SPU.DCS.Features.Devices;
    using Fanex.NAP.SPU.DCS.Features.Devices.Entities;
    using Fanex.NAP.SPU.DCS.JobService;
    using Fanex.NAP.SPU.DCS.JobService.Services;
    using NSubstitute;
    using NSubstitute.ReceivedExtensions;
    using Xunit;

    public class DeviceJobServiceTests
    {
        private ILogService logService;
        private IDeviceServices deviceService;
        private IAppSettings appSettings;
        private HttpClient httpClient;

        private readonly IFixture fixture = new Fixture();
        private readonly bool isExtra = false;


        [Fact]
        public async Task TransformDevice_WhenSuccessAndIsDeviceRecover_ShouldCallExactFunctionAndExactParams()
        {
            // Arrange
            var stagingTransIds = GetStagingTrans();
            var deviceJobService = GetService();

            deviceService.GetStagingTransIdForDeviceAsync(Arg.Any<int>(), Arg.Any<int>(), isExtra).Returns(stagingTransIds);

            // Act
            var result = await deviceJobService.Execute(1, 1, isExtra);

            // Assert
            foreach (var group in stagingTransIds.GroupBy(x => x.BatchId))
            {
                await deviceService.Received(1).TransformDeviceInsertAsync(Arg.Is<IEnumerable<StagingTransId>>(transIds => transIds.SequenceEqual(group)), isExtra);
            }
            Assert.True(result);
        }

        [Fact]
        public async void TransformDevice_WhenGetStagingTransThrowError_ShouldCallExactSentry()
        {
            //Arrange
            var deviceJobService = GetService();

            deviceService.When(x => x.GetStagingTransIdForDeviceAsync(1, 1, isExtra))
                            .Do(x => { throw new Exception("Sql exception"); });

            //Act
            var result = await deviceJobService.Execute(1, 1, false);

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
            };
            var deviceJobService = GetService();

            deviceService.GetStagingTransIdForDeviceAsync(Arg.Any<int>(), Arg.Any<int>(), isExtra).Returns(stagingTransIds);

            deviceService.When(x => x.TransformDeviceInsertAsync(Arg.Any<IEnumerable<StagingTransId>>(), isExtra))
                            .Do(x => { throw new Exception("Sql exception"); });

            //Act
            var result = await deviceJobService.Execute(1, 1, false);

            //Assert
            await logService.Received(3).ErrorAsync(Arg.Any<Exception>());

            Assert.False(result);
        }

        [Fact]
        public async Task TransformDevice_WhenSizeAndNumberOfBatchAreGreaterThanZero_ShouldCallInsertFPDeviceMappingAndTransformDevice()
        {
            // Arrange
            var deviceJobService = GetService();
            int size = 1;
            int numberOfBatch = 1;
            var stagingTransIds = GetStagingTrans();

            deviceService.GetStagingTransIdForDeviceAsync(Arg.Any<int>(), Arg.Any<int>(), isExtra).Returns(stagingTransIds);

            // Act
            var result = await deviceJobService.Execute(size, numberOfBatch, isExtra);

            // Assert
            await deviceService.Received(1).InsertFPDeviceMappingAsync(size * numberOfBatch, isExtra);
            foreach (var group in stagingTransIds.GroupBy(x => x.BatchId))
            {
                await deviceService.Received(1).TransformDeviceInsertAsync(Arg.Is<IEnumerable<StagingTransId>>(transIds => transIds.SequenceEqual(group)), isExtra);
            }
            Assert.True(result);
        }

        [Fact]
        public async Task TransformDevice_WhenSizeOrNumberOfBatchIsZero_ShouldLogErrorAndReturnFalse()
        {
            // Arrange
            var deviceJobService = GetService();

            // Act
            var result = await deviceJobService.Execute(0, 1, isExtra);

            // Assert
            await logService.Received(1).ErrorAsync(Arg.Is<Exception>(ex => ex.Message == "TransformDevice BatchSize = 0!"));
            Assert.False(result);
        }

        [Fact]
        public async Task TransformDevice_WhenExceptionIsThrown_ShouldLogErrorAndReturnFalse()
        {
            // Arrange
            var deviceJobService = GetService();
            deviceService.When(x => x.InsertFPDeviceMappingAsync(Arg.Any<int>(), Arg.Any<bool>()))
                         .Do(x => { throw new Exception("Test exception"); });

            // Act
            var result = await deviceJobService.Execute(1, 1, isExtra);

            // Assert
            await logService.Received(1).ErrorAsync(Arg.Is<Exception>(ex => ex.Message == "Test exception"));
            Assert.False(result);
        }

        [Fact]
        public async Task DeviceEvolution_WhenBatchSizeIsGreaterThanZero_ShouldCallGetFPDeviceEvolutionAsyncOnly()
        {
            // Arrange
            var deviceJobService = GetService();
            int batchSize = 1;
            var dataEvolution = fixture.CreateMany<DeviceEvolutionEntity>().ToList();
            var dataEvolutionAttribute = fixture.CreateMany<DeviceEvolutionEntity>().ToList();
            var resultDeviceEvolution = (dataEvolution, dataEvolutionAttribute);

            deviceService.GetFPDeviceEvolutionAsync(Arg.Any<int>(), Arg.Any<bool>()).Returns(resultDeviceEvolution);

            // Act
            var result = await deviceJobService.DeviceEvolution(batchSize, isExtra);

            // Assert
            await deviceService.Received(1).GetFPDeviceEvolutionAsync(batchSize, isExtra);
            Assert.False(result);
        }

        [Fact]
        public async Task DeviceEvolution_WhenBatchSizeIsZero_ShouldLogErrorAndReturnFalse()
        {
            // Arrange
            var deviceJobService = GetService();

            // Act
            var result = await deviceJobService.DeviceEvolution(0, isExtra);

            // Assert
            await logService.Received(1).ErrorAsync(Arg.Is<Exception>(ex => ex.Message == "DeviceEvolution BatchSize = 0!"));
            Assert.False(result);
        }

        [Fact]
        public async Task DeviceEvolution_WhenExceptionIsThrown_ShouldLogErrorAndReturnFalse()
        {
            // Arrange
            var deviceJobService = GetService();
            deviceService.When(x => x.GetFPDeviceEvolutionAsync(Arg.Any<int>(), Arg.Any<bool>()))
                         .Do(x => { throw new Exception("Test exception"); });

            // Act
            var result = await deviceJobService.DeviceEvolution(1, isExtra);

            // Assert
            await logService.Received(1).ErrorAsync(Arg.Is<Exception>(ex => ex.Message == "Test exception"));
            Assert.False(result);
        }

        [Fact]
        public async Task DeviceEvolution_WhenFindMatchingCandidateExceptionIsThrown_ShouldLogErrorAndReturnFalse()
        {
            // Arrange
            var deviceJobService = GetService();
            var dataEvolution = fixture.CreateMany<DeviceEvolutionEntity>();
            var dataEvolutionAttribute = fixture.CreateMany<DeviceEvolutionEntity>();
            var resultDeviceEvolution = (dataEvolution, dataEvolutionAttribute);

            deviceService.GetFPDeviceEvolutionAsync(Arg.Any<int>(), Arg.Any<bool>()).Returns(resultDeviceEvolution);

            // Act
            var result = await deviceJobService.DeviceEvolution(1, true);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DeviceEvolution_WhenDataEvolutionIsNull_ShouldNotCallInsertFPDeviceEvolutionAsync()
        {
            // Arrange
            var deviceJobService = GetService();
            var resultDeviceEvolution = (null as IEnumerable<DeviceEvolutionEntity>, fixture.CreateMany<DeviceEvolutionEntity>());

            deviceService.GetFPDeviceEvolutionAsync(Arg.Any<int>(), Arg.Any<bool>()).Returns(resultDeviceEvolution);

            // Act
            var result = await deviceJobService.DeviceEvolution(1, isExtra);

            // Assert
            await deviceService.Received(1).GetFPDeviceEvolutionAsync(1, isExtra);
            await deviceService.DidNotReceive().InsertFPDeviceEvolutionAsync(Arg.Any<string>(), isExtra);
            Assert.True(result);
        }

        [Fact]
        public async Task DeviceEvolution_WhenDataEvolutionIsEmpty_ShouldNotCallInsertFPDeviceEvolutionAsync()
        {
            // Arrange
            var deviceJobService = GetService();
            var resultDeviceEvolution = (Enumerable.Empty<DeviceEvolutionEntity>(), fixture.CreateMany<DeviceEvolutionEntity>());

            deviceService.GetFPDeviceEvolutionAsync(Arg.Any<int>(), Arg.Any<bool>()).Returns(resultDeviceEvolution);

            // Act
            var result = await deviceJobService.DeviceEvolution(1, isExtra);

            // Assert
            await deviceService.Received(1).GetFPDeviceEvolutionAsync(1, isExtra);
            await deviceService.DidNotReceive().InsertFPDeviceEvolutionAsync(Arg.Any<string>(), isExtra);
            Assert.True(result);
        }

        private static List<StagingTransId> GetStagingTrans()
        {
            return new List<StagingTransId>
            {
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
            };
        }

        private DeviceJobService GetService()
        {
            logService = Substitute.For<ILogService>();
            deviceService = Substitute.For<IDeviceServices>();
            appSettings = Substitute.For<IAppSettings>();
            httpClient = Substitute.For<HttpClient>();
            return new DeviceJobService(logService, deviceService, appSettings, httpClient);
        }

    }
}
