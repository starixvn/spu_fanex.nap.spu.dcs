namespace Fanex.NAP.SPU.DCS.Tests.JobService.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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

        private readonly IFixture fixture = new Fixture();


        [Fact]
        public async void TransformDevice_WhenSuccessAndIsNewDevice_ShouldCallExactFunctionWithTimes()
        {
            //Arrange

            List<StagingTransId> stagingTransIds = GetStagingTrans();

            var groupTransCount = stagingTransIds.GroupBy(x => x.BatchId).Count();

            var identityDevices = fixture.Create<IEnumerable<IdentityDevice>>();
            var transactionRecover = fixture.Create<IEnumerable<TransactionRecover>>();
            var evolutionAttributes = fixture.Create<IEnumerable<TransactionEvolutionAttributes>>();
            var transactionEvolution = fixture.Create<IEnumerable<TransactionEvolution>>();

            var deviceRecovers = (transactionRecover, transactionEvolution);

            var deviceJobService = GetService();

            deviceService.GetStagingTransIdForDeviceAsync(1, 1).Returns(stagingTransIds);
            deviceService.TransformDeviceInsertAsync(Arg.Any<IEnumerable<StagingTransId>>()).Returns(identityDevices);
            deviceService.GetIdentityDeviceRecoverAsync(Arg.Any<string>()).Returns(deviceRecovers);
            deviceService.GetIdentityEvolutionAttributesAsync(Arg.Any<string>(), Arg.Any<string>()).Returns(evolutionAttributes);

            deviceService.IdentityDeviceInsertAsync(Arg.Any<string>()).Returns(transactionRecover);

            //Act
            var result = await deviceJobService.Execute(1, 1, false);

            //Assert
            await deviceService.Received(groupTransCount).TransformDeviceInsertAsync(Arg.Any<IEnumerable<StagingTransId>>());
            await deviceService.Received(groupTransCount).GetIdentityDeviceRecoverAsync(Arg.Any<string>());

            await deviceService.Received(groupTransCount).IdentityDeviceInsertAsync(Arg.Any<string>());
            await deviceService.DidNotReceive().IdentityEvolutionInsertAsync(Arg.Any<IEnumerable<IdentifyEvolutionModel>>());

            await deviceService.Received().TransformTransaction07InsertAsync(Arg.Any<IEnumerable<TransformTransactionModel>>());

            Assert.True(result);
        }

        [Fact]
        public async void TransformDevice_WhenSuccessAndIsDeviceRecover_ShouldCallExactFunctionAndExactParams()
        {
            //Arrange
            List<StagingTransId> stagingTransIds = GetStagingTrans();

            var groupTransCount = stagingTransIds.GroupBy(x => x.BatchId).Count();

            var transactionRecover = fixture.Build<TransactionRecover>()
                                    .With(x => x.TransID, (ulong)1)
                                    .With(x => x.FPDeviceID, (ulong)2)
                                    .CreateMany(3);

            var transactionAttr = GetEvolutionAttributes();
            var transactionEvolution = GetTransactionEvolution();

            var deviceRecovers = (transactionRecover, transactionEvolution);

            var identityDevice = fixture.Build<IdentityDevice>()
                                    .With(x => x.TransID, (ulong)1)
                                    .CreateMany();

            var deviceJobService = GetService();

            deviceService.GetStagingTransIdForDeviceAsync(Arg.Any<int>(), Arg.Any<int>()).Returns(stagingTransIds);
            deviceService.TransformDeviceInsertAsync(Arg.Any<IEnumerable<StagingTransId>>()).Returns(identityDevice);
            deviceService.GetIdentityDeviceRecoverAsync(Arg.Any<string>()).Returns(deviceRecovers);
            deviceService.GetIdentityEvolutionAttributesAsync(Arg.Any<string>(), Arg.Any<string>()).Returns(transactionAttr);
            deviceService.IdentityEvolutionInsertAsync(Arg.Any<IEnumerable<IdentifyEvolutionModel>>()).Returns(transactionRecover);

            //Act
            var result = await deviceJobService.Execute(1, 1, false);

            //Assert
            await deviceService.Received().TransformDeviceInsertAsync(Arg.Any<IEnumerable<StagingTransId>>());
            await deviceService.Received().GetIdentityDeviceRecoverAsync(Arg.Any<string>());

            await deviceService.DidNotReceive().IdentityDeviceInsertAsync(Arg.Any<string>());
            await deviceService.Received().TransformTransaction07InsertAsync(Arg.Any<IEnumerable<TransformTransactionModel>>());

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

            var transactionRecover = fixture.Build<TransactionRecover>()
                                    .With(x => x.TransID, (ulong)1)
                                    .With(x => x.FPDeviceID, (ulong)2)
                                    .CreateMany(3);

            var transactionEvolution = GetTransactionEvolution();

            var deviceRecovers = (transactionRecover, transactionEvolution);

            var deviceJobService = GetService();

            deviceService.GetStagingTransIdForDeviceAsync(Arg.Any<int>(), Arg.Any<int>()).Returns(stagingTransIds);
            deviceService.GetIdentityDeviceRecoverAsync(Arg.Any<string>()).Returns(deviceRecovers);

            deviceService.When(x => x.TransformDeviceInsertAsync(Arg.Any<IEnumerable<StagingTransId>>()))
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

            var identityDevice = fixture.Create<IEnumerable<IdentityDevice>>();

            var deviceJobService = GetService();

            deviceService.GetStagingTransIdForDeviceAsync(Arg.Any<int>(), Arg.Any<int>()).Returns(stagingTransIds);
            deviceService.TransformDeviceInsertAsync(Arg.Any<IEnumerable<StagingTransId>>()).Returns(identityDevice);

            deviceService.When(x => x.GetIdentityDeviceRecoverAsync(Arg.Any<string>()))
                            .Do(x => { throw new Exception("Sql exception"); });

            //Act
            var result = await deviceJobService.Execute(1, 1, false);

            //Assert
            await logService.Received(1).ErrorAsync(Arg.Any<Exception>());

            Assert.False(result);
        }



        [Fact]
        public async void TransformDevice_WhenTwoTasksThrowError_ShouldCallExactSentryWithTimes()
        {
            //Arrange
            List<StagingTransId> stagingTransIds = GetStagingTrans();

            var identityDevice = fixture.Create<IEnumerable<IdentityDevice>>();

            var deviceJobService = GetService();

            deviceService.GetStagingTransIdForDeviceAsync(Arg.Any<int>(), Arg.Any<int>()).Returns(stagingTransIds);

            deviceService.When(x => x.TransformDeviceInsertAsync(Arg.Any<IEnumerable<StagingTransId>>())).Do(x => { throw new Exception("Sql exception"); });
            deviceService.When(x => x.GetIdentityDeviceRecoverAsync(Arg.Any<string>())).Do(x => { throw new Exception("Sql exception"); });

            //Act
            var result = await deviceJobService.Execute(1, 1, false);

            //Assert
            await logService.Received(3).ErrorAsync(Arg.Any<Exception>());

            Assert.False(result);
        }

        [Fact]
        public async void TransformDevice_WhenTransformTransaction07InsertAsyncThrowError_ShouldCallExactSentryWithTimes()
        {
            //Arrange
            List<StagingTransId> stagingTransIds = GetStagingTrans();

            var groupTransCount = stagingTransIds.GroupBy(x => x.BatchId).Count();

            var transactionRecover = fixture.Create<IEnumerable<TransactionRecover>>();

            var transactionAttr = GetEvolutionAttributes();
            var transactionEvolution = GetTransactionEvolution();

            var deviceRecovers = (transactionRecover, transactionEvolution);

            var identityDevice = fixture.Create<IEnumerable<IdentityDevice>>();

            var deviceJobService = GetService();

            deviceService.GetStagingTransIdForDeviceAsync(Arg.Any<int>(), Arg.Any<int>()).Returns(stagingTransIds);
            deviceService.TransformDeviceInsertAsync(Arg.Any<IEnumerable<StagingTransId>>()).Returns(identityDevice);
            deviceService.GetIdentityDeviceRecoverAsync(Arg.Any<string>()).Returns(deviceRecovers);
            deviceService.GetIdentityEvolutionAttributesAsync(Arg.Any<string>(), Arg.Any<string>()).Returns(transactionAttr);
            deviceService.IdentityEvolutionInsertAsync(Arg.Any<IEnumerable<IdentifyEvolutionModel>>()).Returns(transactionRecover);

            deviceService.When(x => x.TransformTransaction07InsertAsync(Arg.Any<IEnumerable<TransformTransactionModel>>())).Do(x => { throw new Exception("Sql exception"); });

            //Act
            var result = await deviceJobService.Execute(1, 1, false);

            //Assert
            await logService.Received(1).ErrorAsync(Arg.Any<Exception>());

            Assert.False(result);
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
            return new DeviceJobService(logService, deviceService, appSettings);
        }

        private static List<TransactionEvolutionAttributes> GetEvolutionAttributes()
        {
            return new List<TransactionEvolutionAttributes>
            {
                new TransactionEvolutionAttributes
                {
                    RawFingerPrintID = 1,
                    UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/116.0.0.0 Safari/537.36",
                    BrowserFamilyCode = "7a8f56652855ae80aae2e34dc5dc4e81",
                    OSName = "OSName",
                    BrowserName = "BrowserName",
                    BrowserVersion = "116.0.0.0",
                    Platform = "Platform",
                    Accept = "Accept",
                    Encoding = "UTF-8",
                    Headers = "Headers",
                    LocalStorage = "LocalStorage",
                    DoNotTrack= "LocalStorage",
                    Cookies = "Cookies",
                    Canvas = "Canvas",
                    Fonts = "[\"Calibri\", \"Franklin Gothic\", \"MS UI Gothic\", \"Marlett\", \"Segoe UI Light\"]",
                    Vendor = "Vendor",
                    GLRenderer = "GLRenderer",
                    Plugins = "Plugins",
                    Languages = "Languages",
                    Resolution = "Resolution",
                    Timezone = "Timezone"
                }
            };
        }

        private static List<TransactionEvolution> GetTransactionEvolution()
        {
            return new List<TransactionEvolution>
            {
                new TransactionEvolution
                {
                    TransID = 1,
                    RawFingerPrintIDs = "1",
                    RawFingerPrintID = 1,
                    UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/116.0.0.0 Safari/537.36",
                    BrowserFamilyCode = "7a8f56652855ae80aae2e34dc5dc4e81",
                    OSName = "OSName",
                    BrowserName = "BrowserName",
                    BrowserVersion = "116.0.0.0",
                    Platform = "Platform",
                    Accept = "Accept",
                    Encoding = "UTF-8",
                    Headers = "Headers",
                    LocalStorage = "LocalStorage",
                    DoNotTrack= "LocalStorage",
                    Cookies = "Cookies",
                    Canvas = "Canvas",
                    Fonts = "[\"Calibri\", \"Franklin Gothic\", \"MS UI Gothic\", \"Marlett\", \"Segoe UI Light\"]",
                    Vendor = "Vendor",
                    GLRenderer = "GLRenderer",
                    Plugins = "Plugins",
                    Languages = "Languages",
                    Resolution = "Resolution",
                    Timezone = "Timezone"
                }
            };
        }
    }
}
