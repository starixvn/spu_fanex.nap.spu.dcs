namespace Fanex.NAP.SPU.DCS.Tests.Features.Devices
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoFixture;
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.DCS.Features.Devices;
    using Fanex.NAP.SPU.DCS.Features.Devices.Criteria;
    using Fanex.NAP.SPU.DCS.Features.Devices.Entities;
    using FluentAssertions;
    using NSubstitute;
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
            deviceDataAccess.IsReadyDbExtra = isExtra;
            var criteria = fixture.Create<GetDeviceStagingTransactionPackageCriteria>();

            repository.FetchAsync<StagingTransId>(Arg.Do<GetDeviceStagingTransactionPackageCriteria>(x => criteria = x)).Returns(stagingTransIds);

            // Act
            var actual = await deviceDataAccess.GetStagingTransIdForDeviceAsync(1, 1);

            // Assert
            var settingKey = criteria.GetSettingKey();
            Assert.Equal(stagingTransIds, actual);
            Assert.Equal(expectedSettingKey, settingKey);
        }

        [Theory]
        [InlineData(true, "DCS_ET_Transform_Device_Insert")]
        [InlineData(false, "DCS_DC_Transform_Device_Insert")]
        public async Task TransformDeviceInsertAsync_WhenCall_ReturnExactData(bool isExtra, string expectedSettingKey)
        {
            // Arrange
            var identityDevices = fixture.CreateMany<IdentityDevice>();
            deviceDataAccess.IsReadyDbExtra = isExtra;
            var criteria = fixture.Create<TransformDeviceInsertCriteria>();

            repository.FetchAsync<IdentityDevice>(Arg.Do<TransformDeviceInsertCriteria>(x => criteria = x)).Returns(identityDevices);

            // Act
            var actual = await deviceDataAccess.TransformDeviceInsertAsync("transIdJson");

            // Assert
            var settingKey = criteria.GetSettingKey();
            Assert.Equal(identityDevices, actual);
            Assert.Equal(expectedSettingKey, settingKey);
        }

        [Theory]
        [InlineData(true, "DCS_ET_Identity_Device_Recover")]
        [InlineData(false, "DCS_DC_Identity_Device_Recover")]
        public async Task GetIdentityDeviceRecoverAsync_WhenCall_ReturnExactData(bool isExtra, string expectedSettingKey)
        {
            // Arrange
            var transactionRecovers = fixture.CreateMany<TransactionRecover>();
            var transactionEvolutions = fixture.CreateMany<TransactionEvolution>();

            var indentityRecovers = (transactionRecovers, transactionEvolutions);

            deviceDataAccess.IsReadyDbExtra = isExtra;
            var criteria = fixture.Create<GetIdentityDeviceRecoverCriteria>();

            repository.FetchMultipleAsync<TransactionRecover, TransactionEvolution>(Arg.Do<GetIdentityDeviceRecoverCriteria>(x => criteria = x)).Returns(indentityRecovers);

            // Act
            var actual = await deviceDataAccess.GetIdentityDeviceRecoverAsync("transIds");

            // Assert
            var settingKey = criteria.GetSettingKey();
            indentityRecovers.Should().Equals(indentityRecovers);
            Assert.Equal(expectedSettingKey, settingKey);
        }

        [Theory]
        [InlineData(true, "DCS_ET_Identity_Evolution_GetAttributes")]
        [InlineData(false, "DCS_DC_Identity_Evolution_GetAttributes")]
        public async Task GetIdentityEvolutionAttributesAsync_WhenCall_ReturnExactData(bool isExtra, string expectedSettingKey)
        {
            // Arrange
            var transactionEvolutionAttributes = fixture.CreateMany<TransactionEvolutionAttributes>();

            deviceDataAccess.IsReadyDbExtra = isExtra;
            var criteria = fixture.Create<GetIdentityEvolutionAttributesCriteria>();

            repository.FetchAsync<TransactionEvolutionAttributes>(Arg.Do<GetIdentityEvolutionAttributesCriteria>(x => criteria = x)).Returns(transactionEvolutionAttributes);

            // Act
            var actual = await deviceDataAccess.GetIdentityEvolutionAttributesAsync("rawFingerPrintIDs", "browserFamilyCode");

            // Assert
            var settingKey = criteria.GetSettingKey();
            Assert.Equal(actual, transactionEvolutionAttributes);
            Assert.Equal(expectedSettingKey, settingKey);
        }

        [Theory]
        [InlineData(true, "DCS_ET_Identity_Device_Insert")]
        [InlineData(false, "DCS_DC_Identity_Device_Insert")]
        public async Task IdentityDeviceInsertAsync_WhenCall_ReturnExactData(bool isExtra, string expectedSettingKey)
        {
            // Arrange
            var transactionRecovers = fixture.CreateMany<TransactionRecover>();

            deviceDataAccess.IsReadyDbExtra = isExtra;
            var criteria = fixture.Create<IdentityDeviceInsertCriteria>();

            repository.FetchAsync<TransactionRecover>(Arg.Do<IdentityDeviceInsertCriteria>(x => criteria = x)).Returns(transactionRecovers);

            // Act
            var actual = await deviceDataAccess.IdentityDeviceInsertAsync("transId");

            // Assert
            var settingKey = criteria.GetSettingKey();
            Assert.Equal(actual, transactionRecovers);
            Assert.Equal(expectedSettingKey, settingKey);
        }

        [Theory]
        [InlineData(true, "DCS_ET_Identity_Evolution_Insert")]
        [InlineData(false, "DCS_DC_Identity_Evolution_Insert")]
        public async Task IdentityEvolutionInsertAsync_WhenCall_ReturnExactData(bool isExtra, string expectedSettingKey)
        {
            // Arrange
            var transactionRecovers = fixture.CreateMany<TransactionRecover>();

            deviceDataAccess.IsReadyDbExtra = isExtra;
            var criteria = fixture.Create<IdentityEvolutionInsertCriteria>();

            repository.FetchAsync<TransactionRecover>(Arg.Do<IdentityEvolutionInsertCriteria>(x => criteria = x)).Returns(transactionRecovers);

            // Act
            var actual = await deviceDataAccess.IdentityEvolutionInsertAsync("transJson");

            // Assert
            var settingKey = criteria.GetSettingKey();
            Assert.Equal(actual, transactionRecovers);
            Assert.Equal(expectedSettingKey, settingKey);
        }

        [Theory]
        [InlineData(true, "DCS_ET_Transform_Transaction07_Insert")]
        [InlineData(false, "DCS_DC_Transform_Transaction07_Insert")]
        public async Task TransformTransaction07InsertAsync_WhenCall_ReturnExactData(bool isExtra, string expectedSettingKey)
        {
            // Arrange

            deviceDataAccess.IsReadyDbExtra = isExtra;
            var criteria = fixture.Create<TransformTransaction07InsertCommand>();

            repository.ExecuteAsync(Arg.Do<TransformTransaction07InsertCommand>(x => criteria = x)).Returns(1);

            // Act
            var actual = await deviceDataAccess.TransformTransaction07InsertAsync("transJson");

            // Assert
            var settingKey = criteria.GetSettingKey();
            Assert.Equal(1, actual);
            Assert.Equal(expectedSettingKey, settingKey);
        }
    }
}
