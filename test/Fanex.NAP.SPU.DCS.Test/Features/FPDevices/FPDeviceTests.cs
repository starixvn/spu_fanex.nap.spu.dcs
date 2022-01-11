namespace Fanex.NAP.SPU.DCS.Tests.Features.FPDevices
{
    using Fanex.NAP.SPU.DCS.Features.FPDevices;
    using Fanex.NAP.SPU.DCS.Features.FPDevices.Entities;
    using Fanex.NAP.SPU.DCS.Shares;
    using FluentAssertions;
    using Newtonsoft.Json;
    using NSubstitute;    
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    public class FPDeviceTests
    {
        private readonly IFPDeviceDataAccess fPDeviceDataAccess;
        private readonly IFPDeviceServices fPDeviceServices;

        public FPDeviceTests()
        {
            fPDeviceDataAccess = Substitute.For<IFPDeviceDataAccess>();

            fPDeviceServices = new FPDeviceServices(fPDeviceDataAccess);
        }

        [Fact]
        public void GetFPStagingTransIdForDevice_WhenSuccess_ReturnTransIdList()
        {
            var stagingTransIdEntity = new List<FPStagingTransIdEntity> {
                new FPStagingTransIdEntity
                {
                    BatchId = 1,
                    TransId = 100
                },
                new FPStagingTransIdEntity
                {
                    BatchId = 1,
                    TransId = 200
                },
                new FPStagingTransIdEntity
                {
                    BatchId = 1,
                    TransId = 300
                },
                new FPStagingTransIdEntity
                {
                    BatchId = 2,
                    TransId = 102
                },
                new FPStagingTransIdEntity
                {
                    BatchId = 2,
                    TransId = 202
                },
                new FPStagingTransIdEntity
                {
                    BatchId = 2,
                    TransId = 302
                }
            };

            fPDeviceDataAccess.GetFPStagingTransIdForDevice(100, 2).Returns(stagingTransIdEntity);

            var result = fPDeviceServices.GetStagingTransIdForDevice(100, 2);

            result.Should().BeEquivalentTo(stagingTransIdEntity);
        }

        [Theory]
        [InlineData(-1, 1)]
        [InlineData(1, -1)]
        [InlineData(-1, -1)]
        [InlineData(0, 0)]
        [InlineData(0, -1)]
        [InlineData(-1, 0)]
        public void GetFPStagingTransIdForDevice_WhenFailed_ReturnTransIdList(int size, int batchSize)
        {
            var expectedResult = new List<FPStagingTransIdEntity>();

            fPDeviceDataAccess.GetFPStagingTransIdForDevice(size, batchSize);

            var result = fPDeviceServices.GetStagingTransIdForDevice(size, batchSize);

            result.Should().BeEmpty();
            result.Should().BeEquivalentTo(expectedResult);
            result.Count().Should().Be(0);
        }

        [Fact]
        public void UpdateDevice_WhenSucess_ReturnDataResult()
        {
            var stagingTransIdEntity = new List<FPStagingTransIdEntity> {
                new FPStagingTransIdEntity
                {
                    BatchId = 1,
                    TransId = 100
                },
                new FPStagingTransIdEntity
                {
                    BatchId = 1,
                    TransId = 200
                },
                new FPStagingTransIdEntity
                {
                    BatchId = 1,
                    TransId = 300
                }
            };

            var dataResult = new DataResult
            {
                ErrorCode = 0,
                ErrorMessage = string.Empty
            };

            var jsonResult = JsonConvert.SerializeObject(stagingTransIdEntity);

            fPDeviceDataAccess.UpdateDevice(jsonResult).Returns(dataResult);

            var result = fPDeviceServices.UpdateDevices(stagingTransIdEntity);

            result.Should().Be(dataResult);
            result.ErrorCode.Should().Be(0);
        }

        [Fact]
        public void UpdateDevice_WhenFailed_ReturnDataResult()
        {
            var dataResult = new DataResult
            {
                ErrorCode = -1,
                ErrorMessage = "Parameter is empty."
            };

            fPDeviceDataAccess.UpdateDevice(null).Returns(dataResult);

            var result = fPDeviceServices.UpdateDevices(null);

            result.ErrorCode.Should().Be(-1);
        }
    }
}
