
namespace Fanex.NAP.SPU.DCS.Tests.Features.SystemSetting
{
    using System.Threading.Tasks;
    using AutoFixture;
    using Fanex.NAP.SPU.DCS.Features.SystemSetting;
    using NSubstitute;
    using Xunit;
    public class SystemSettingServicesTest
    {
        private readonly ISystemSettingDataAccess dataAccess;
        private readonly ISystemSettingServices systemSettingServices;
        private readonly IFixture fixture = new Fixture();

        public SystemSettingServicesTest()
        {
            dataAccess = Substitute.For<ISystemSettingDataAccess>();
            systemSettingServices = new SystemSettingServices(dataAccess);
        }

        [Fact]
        public async Task UpdateDCSystemSettingByID_WhenCalled_ExecutesWithCorrectParameters()
        {
            // Arrange
            var systemSettingID = fixture.Create<int>();

            var systemSettingValue = fixture.Create<string>();

            // Act
            var result = await systemSettingServices.UpdateDCSystemSettingByID(systemSettingID, systemSettingValue);

            // Assert
            await dataAccess.Received(1).UpdateDCSystemSettingByID(systemSettingID, systemSettingValue);
            Assert.True(result);
        }
    }
}