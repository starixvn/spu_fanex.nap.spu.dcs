namespace Fanex.NAP.SPU.DCS.Tests.Features.Devices
{
    using Fanex.NAP.SPU.DCS.Features.Devices;
    using Fanex.NAP.SPU.DCS.Features.Devices.Entities;
    using FluentAssertions;
    using NSubstitute;
    using System.Collections.Generic;
    using Xunit;

    public class DeviceTests
    {
        private readonly IDeviceDataAccess deviceDataAccess;
        private readonly IDeviceServices deviceServices;

        public DeviceTests()
        {
            deviceDataAccess = Substitute.For<IDeviceDataAccess>();

            deviceServices = new DeviceServices(deviceDataAccess);
        }

        [Fact]
        public void GetStagingTransIdForDevice_WhenSuccess_ReturnTransIdList()
        {
            var isReadyDbExtra = false;

            var stagingTransIdEntity = new List<StagingTransId> {
                new StagingTransId
                {
                    BatchId = 1,
                    TransId = 100
                },
                new StagingTransId
                {
                    BatchId = 1,
                    TransId = 200
                },
                new StagingTransId
                {
                    BatchId = 1,
                    TransId = 300
                },
                new StagingTransId
                {
                    BatchId = 2,
                    TransId = 102
                },
                new StagingTransId
                {
                    BatchId = 2,
                    TransId = 202
                },
                new StagingTransId
                {
                    BatchId = 2,
                    TransId = 302
                }
            };

            deviceDataAccess.GetStagingTransIdForDeviceAsync(100, 2, isReadyDbExtra).Returns(stagingTransIdEntity);

            var result = deviceServices.GetStagingTransIdForDeviceAsync(100, 2, isReadyDbExtra);

            result.Should().BeEquivalentTo(stagingTransIdEntity);
        }

        [Theory]
        [InlineData(-1, 1)]
        [InlineData(1, -1)]
        [InlineData(-1, -1)]
        [InlineData(0, 0)]
        [InlineData(0, -1)]
        [InlineData(-1, 0)]
        public void GetStagingTransIdForDevice_WhenFailed_ReturnTransIdList(int size, int batchSize)
        {
            var expectedResult = new List<StagingTransId>();
            var isReadyDbExtra = false;

            deviceDataAccess.GetStagingTransIdForDeviceAsync(size, batchSize, isReadyDbExtra).Returns(expectedResult);

            var result = deviceServices.GetStagingTransIdForDeviceAsync(size, batchSize, isReadyDbExtra);

            result.Should().BeEquivalentTo(expectedResult);
        }

        //[Fact]
        //public void UpdateDevice_WhenSucess_ReturnDataResult()
        //{
        //    var isReadyDbExtra = false;
        //    var stagingTransIdEntity = new List<StagingTransId> {
        //        new StagingTransId
        //        {
        //            BatchId = 1,
        //            TransId = 100
        //        },
        //        new StagingTransId
        //        {
        //            BatchId = 1,
        //            TransId = 200
        //        },
        //        new StagingTransId
        //        {
        //            BatchId = 1,
        //            TransId = 300
        //        }
        //    };
        //    var dataResult = new DataResult {
        //        ErrorCode = 0,
        //        ErrorMessage = string.Empty
        //    };
        //    var jsonResult = JsonConvert.SerializeObject(stagingTransIdEntity);

        //    deviceDataAccess.UpdateDeviceAsync(jsonResult, isReadyDbExtra).Returns(dataResult);

        //    var result = deviceServices.UpdateDevices(stagingTransIdEntity, isReadyDbExtra);

        //    result.Should().Be(dataResult);
        //    result.ErrorCode.Should().Be(0);
        //}

        //[Fact]
        //public void UpdateDevice_WhenFailed_ReturnDataResult()
        //{
        //    var isReadyDbExtra = false;
        //    var dataResult = new DataResult
        //    {
        //        ErrorCode = -1,
        //        ErrorMessage = "Parameter is empty."
        //    };

        //    deviceDataAccess.UpdateDeviceAsync(null, isReadyDbExtra).Returns(dataResult);
        //    var result = deviceServices.UpdateDevices(null, isReadyDbExtra);
                       
        //    result.ErrorCode.Should().Be(-1);
        //}
    }
}
