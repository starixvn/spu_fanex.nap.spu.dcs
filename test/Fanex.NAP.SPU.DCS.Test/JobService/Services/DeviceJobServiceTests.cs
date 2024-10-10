namespace Fanex.NAP.SPU.DCS.Tests.JobService.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
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
        public async void TransformDevice_WhenSuccessAndIsDeviceRecover_ShouldCallExactFunctionAndExactParams()
        {
            ////Arrange
            //List<StagingTransId> stagingTransIds = GetStagingTrans();

            //var groupTransCount = stagingTransIds.GroupBy(x => x.BatchId).Count();

            //var transactionRecover = fixture.Build<TransactionRecover>()
            //                        .With(x => x.TransID, (ulong)1)
            //                        .With(x => x.FPDeviceID, (ulong)2)
            //                        .CreateMany(3);

            //var transactionAttr = GetEvolutionAttributes();
            //var transactionEvolution = GetTransactionEvolution();

            //var deviceRecovers = (transactionRecover, transactionEvolution);

            //var identityDevice = fixture.Build<IdentityDevice>()
            //                        .With(x => x.TransID, (ulong)1)
            //                        .CreateMany();

            //var deviceJobService = GetService();

            //deviceService.GetStagingTransIdForDeviceAsync(Arg.Any<int>(), Arg.Any<int>(), isExtra).Returns(stagingTransIds);
            //deviceService.TransformDeviceInsertAsync(Arg.Any<IEnumerable<StagingTransId>>(), isExtra).Returns(identityDevice);
            //deviceService.GetIdentityDeviceRecoverAsync(Arg.Any<string>(), isExtra).Returns(deviceRecovers);
            //deviceService.GetIdentityEvolutionAttributesAsync(Arg.Any<string>(), Arg.Any<string>(), isExtra).Returns(transactionAttr);
            //deviceService.IdentityEvolutionInsertAsync(Arg.Any<IEnumerable<IdentifyEvolutionModel>>(), isExtra).Returns(transactionRecover);

            ////Act
            //var result = await deviceJobService.Execute(1, 1, false);

            ////Assert
            //await deviceService.Received().TransformDeviceInsertAsync(Arg.Any<IEnumerable<StagingTransId>>(), isExtra);
            //await deviceService.Received().GetIdentityDeviceRecoverAsync(Arg.Any<string>(), isExtra);

            //await deviceService.DidNotReceive().IdentityDeviceInsertAsync(Arg.Any<string>(), isExtra);
            //await deviceService.Received().TransformTransaction07InsertAsync(Arg.Any<IEnumerable<TransformTransactionModel>>(), isExtra);

            //Assert.True(result);
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
        public async void TransformDevice_WhenGetIdentityDeviceRecoverAsyncThrowError_ShouldCallExactSentryWithTimes()
        {

            //Arrange
            List<StagingTransId> stagingTransIds = GetStagingTrans();

            var deviceJobService = GetService();

            deviceService.GetStagingTransIdForDeviceAsync(Arg.Any<int>(), Arg.Any<int>(), isExtra).Returns(stagingTransIds);

            //Act
            var result = await deviceJobService.Execute(1, 1, false);

            //Assert
            await logService.Received(1).ErrorAsync(Arg.Any<Exception>());

            Assert.False(result);
        }



        [Fact]
        public async void TransformDevice_WhenTwoTasksThrowError_ShouldCallExactSentryWithTimes()
        {
            ////Arrange
            //List<StagingTransId> stagingTransIds = GetStagingTrans();

            //var identityDevice = fixture.Create<IEnumerable<IdentityDevice>>();

            //var deviceJobService = GetService();

            //deviceService.GetStagingTransIdForDeviceAsync(Arg.Any<int>(), Arg.Any<int>(), isExtra).Returns(stagingTransIds);

            //deviceService.When(x => x.TransformDeviceInsertAsync(Arg.Any<IEnumerable<StagingTransId>>(), isExtra)).Do(x => { throw new Exception("Sql exception"); });
            
            ////Act
            //var result = await deviceJobService.Execute(1, 1, false);

            ////Assert
            //await logService.Received(3).ErrorAsync(Arg.Any<Exception>());

            //Assert.False(result);
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
