namespace Fanex.NAP.SPU.DCS.Tests.Features.Devices
{
    using AutoFixture;
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.DCS.Features.ArchiveData.Criteria;
    using Fanex.NAP.SPU.DCS.Features.Devices;
    using Fanex.NAP.SPU.DCS.Features.Devices.Criteria;
    using Fanex.NAP.SPU.DCS.Features.Devices.Entities;
    using NSubstitute;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    public class DeviceDataAccessTests
    {
        private IDynamicRepository repository;
        private IDeviceDataAccess deviceDataAccess;
        private IFixture fixture = new Fixture();

        public DeviceDataAccessTests()
        {
            repository = Substitute.For<IDynamicRepository>();
            deviceDataAccess = new DeviceDataAccess(repository);
        }

        [Theory]
        [InlineData(true, "DCS_ET_Transform_Device_GetPackage")]
        [InlineData(false, "DCS_DC_Transform_Device_GetPackage")]
        public async Task GetStagingTransIdForDeviceAsync_WhenCall_ReturnExactData(bool isExtra, string expectedSettingKey)
        {
            // Arrange
            var stagingTransIds = fixture.CreateMany<StagingTransId>();
            var criteria = fixture.Create<GetDeviceStagingTransactionPackageCriteria>();

            repository.FetchAsync<StagingTransId>(Arg.Do<GetDeviceStagingTransactionPackageCriteria>(x => criteria = x)).Returns(stagingTransIds);

            // Act
            var actual = await deviceDataAccess.GetStagingTransIdForDeviceAsync(1, 1, isExtra);

            // Assert
            var settingKey = criteria.GetSettingKey();
            Assert.Equal(stagingTransIds, actual);
            Assert.Equal(expectedSettingKey, settingKey);
        }

        [Theory]
        [InlineData(true, "DCS_ET_Transform_Device_Insert")]
        [InlineData(false, "DCS_DC_Transform_Device_Insert")]
        public async Task GetFPDeviceEvolutionAsync_WhenCalled_ExecutesWithCorrectCriteria(bool isReadyDbExtra, string expectedSettingKey)
        {
            // Arrange
            var criteria = fixture.Create<GetFPDeviceEvolutionCriteria>();
            var deviceEvolutions = fixture.CreateMany<DeviceEvolutionEntity>();

            repository.FetchMultipleAsync<DeviceEvolutionEntity, DeviceEvolutionEntity>(Arg.Do<GetFPDeviceEvolutionCriteria>(x => criteria = x))
                .Returns(Task.FromResult((deviceEvolutions.AsEnumerable(), deviceEvolutions.AsEnumerable())));

            // Act
            await deviceDataAccess.GetFPDeviceEvolutionAsync(1, isReadyDbExtra);

            // Assert
            Assert.NotNull(criteria);
            Assert.Equal(isReadyDbExtra, criteria.IsReadyDbExtra);

            var settingKey = criteria.GetSettingKey();
            var isValid = criteria.IsInvalid();

            Assert.Equal(expectedSettingKey, settingKey);
        }

        [Theory]
        [InlineData(true, "DCS_ET_Transform_Device_Insert")]
        [InlineData(false, "DCS_DC_Transform_Device_Insert")]
        public async Task TransformDeviceInsertAsync_WhenCalled_ExecutesWithCorrectCriteria(bool isExtra, string expectedSettingKey)
        {
            // Arrange
            var transIDs = "1,2,3";
            var criteria = fixture.Create<TransformDeviceInsertCriteria>();

            repository.ExecuteAsync(Arg.Do<TransformDeviceInsertCriteria>(x => criteria = x)).Returns(1);

            // Act
            await deviceDataAccess.TransformDeviceInsertAsync(transIDs, isExtra);

            // Assert
            Assert.NotNull(criteria);
            Assert.Equal(transIDs, criteria.TransIDs);
            Assert.Equal(isExtra, criteria.IsReadyDbExtra);

            var settingKey = criteria.GetSettingKey();
            var isValid = criteria.IsInvalid();

            Assert.Equal(expectedSettingKey, settingKey);
        }

        [Theory]
        [InlineData(true, "")]
        [InlineData(false, "DCS_DC_Transform_FP_DeviceMappingInsert")]
        public async Task InsertFPDeviceMappingAsync_WhenCalled_ExecutesWithCorrectCriteria(bool isTransformExtra, string expectedSettingKey)
        {
            // Arrange
            var criteria = fixture.Create<InsertFPDeviceMappingCriteria>();

            repository.ExecuteAsync(Arg.Do<InsertFPDeviceMappingCriteria>(x => criteria = x))
                      .Returns(1); 

            // Act
            await deviceDataAccess.InsertFPDeviceMappingAsync(1, isTransformExtra);

            // Assert
            Assert.NotNull(criteria);
            Assert.Equal(1, criteria.BatchSize);
            Assert.Equal(isTransformExtra, criteria.IsReadyDbExtra);

            var settingKey = criteria.GetSettingKey();
            Assert.Equal(expectedSettingKey, settingKey);

            await repository.Received(1).ExecuteAsync(Arg.Any<InsertFPDeviceMappingCriteria>());
        }

        [Theory]
        [InlineData(true, "")]
        [InlineData(false, "DCS_DC_Transform_FP_DeviceEvolutionInsert")]
        public async Task InsertFPDeviceEvolutionAsync_WhenCalled_ExecutesWithCorrectCriteria(bool isTransformExtra, string expectedSettingKey)
        {
            // Arrange
            var deviceEvolutionJson = "{\"id\":1,\"name\":\"Device 1\"}";
            var criteria = fixture.Create<InsertFPDeviceEvolutionCriteria>();

            repository.ExecuteAsync(Arg.Do<InsertFPDeviceEvolutionCriteria>(x => criteria = x))
                      .Returns(1);

            // Act
            await deviceDataAccess.InsertFPDeviceEvolutionAsync(deviceEvolutionJson, isTransformExtra);

            // Assert
            Assert.NotNull(criteria);
            Assert.Equal(deviceEvolutionJson, criteria.DeviceEvolutionJson);
            Assert.Equal(isTransformExtra, criteria.IsReadyDbExtra);

            var settingKey = criteria.GetSettingKey();
            Assert.Equal(expectedSettingKey, settingKey);

            await repository.Received(1).ExecuteAsync(Arg.Any<InsertFPDeviceEvolutionCriteria>());
        }
    }
}
