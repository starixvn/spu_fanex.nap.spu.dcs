using Fanex.NAP.SPU.DCS.Features.Devices;
using Fanex.NAP.SPU.DCS.Features.Devices.Entities;
using Fanex.NAP.SPU.DCS.Shares;
using FluentAssertions;
using Newtonsoft.Json;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Fanex.NAP.SPU.DCS.Tests.Features.Devices
{
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

            var stagingTransIdEntity = new List<StagingTransIdEntity> {
                new StagingTransIdEntity
                {
                    BatchId = 1,
                    TransId = 100
                },
                new StagingTransIdEntity
                {
                    BatchId = 1,
                    TransId = 200
                },
                new StagingTransIdEntity
                {
                    BatchId = 1,
                    TransId = 300
                },
                new StagingTransIdEntity
                {
                    BatchId = 2,
                    TransId = 102
                },
                new StagingTransIdEntity
                {
                    BatchId = 2,
                    TransId = 202
                },
                new StagingTransIdEntity
                {
                    BatchId = 2,
                    TransId = 302
                }
            };

            deviceDataAccess.GetStagingTransIdForDevice(100, 2, isReadyDbExtra).Returns(stagingTransIdEntity);

            var result = deviceServices.GetStagingTransIdForDevice(100, 2, isReadyDbExtra);

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
            var expectedResult = new List<StagingTransIdEntity>();
            var isReadyDbExtra = false;

            deviceDataAccess.GetStagingTransIdForDevice(size, batchSize, isReadyDbExtra).Returns(expectedResult);

            var result = deviceServices.GetStagingTransIdForDevice(size, batchSize, isReadyDbExtra);

            result.Should().BeEmpty();
            result.Should().BeEquivalentTo(expectedResult);
            result.Count().Should().Be(0);
        }

        [Fact]
        public void UpdateDevice_WhenSucess_ReturnDataResult()
        {
            var isReadyDbExtra = false;
            var stagingTransIdEntity = new List<StagingTransIdEntity> {
                new StagingTransIdEntity
                {
                    BatchId = 1,
                    TransId = 100
                },
                new StagingTransIdEntity
                {
                    BatchId = 1,
                    TransId = 200
                },
                new StagingTransIdEntity
                {
                    BatchId = 1,
                    TransId = 300
                }
            };
            var dataResult = new DataResult {
                ErrorCode = 0,
                ErrorMessage = string.Empty
            };
            var jsonResult = JsonConvert.SerializeObject(stagingTransIdEntity);

            deviceDataAccess.UpdateDevice(jsonResult, isReadyDbExtra).Returns(dataResult);

            var result = deviceServices.UpdateDevices(stagingTransIdEntity, isReadyDbExtra);

            result.Should().Be(dataResult);
            result.ErrorCode.Should().Be(0);
        }

        [Fact]
        public void UpdateDevice_WhenFailed_ReturnDataResult()
        {
            var isReadyDbExtra = false;
            var dataResult = new DataResult
            {
                ErrorCode = -1,
                ErrorMessage = "Parameter is empty."
            };

            deviceDataAccess.UpdateDevice(null, isReadyDbExtra).Returns(dataResult);
            var result = deviceServices.UpdateDevices(null, isReadyDbExtra);
                       
            result.ErrorCode.Should().Be(-1);
        }
    }
}
