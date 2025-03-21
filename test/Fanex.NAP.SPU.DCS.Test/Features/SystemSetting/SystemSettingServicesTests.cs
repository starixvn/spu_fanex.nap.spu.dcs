
namespace Fanex.NAP.SPU.DCS.Tests.Features.SystemSetting
{
    using System.Threading.Tasks;
    using AutoFixture;
    using Fanex.NAP.SPU.DCS.Features.SystemSetting;
    using NSubstitute;
    using Xunit;
    public class SystemSettingServicesTests
    {
        private readonly ISystemSettingDataAccess dataAccess;
        private readonly ISystemSettingServices systemSettingServices;
        private readonly IFixture fixture = new Fixture();

        public SystemSettingServicesTests()
        {
            dataAccess = Substitute.For<ISystemSettingDataAccess>();
            systemSettingServices = new SystemSettingServices(dataAccess);
        }

        [Fact]
        public void UpdateDCSystemSettingByID_WhenCalled_ExecutesWithCorrectParameters()
        {
            // Arrange
            var systemSettingID = fixture.Create<int>();

            var systemSettingValue = fixture.Create<string>();

            // Act
            var result = systemSettingServices.UpdateDCSystemSettingByID(systemSettingID, systemSettingValue);

            // Assert
            dataAccess.Received(1).UpdateDCSystemSettingByID(systemSettingID, systemSettingValue);

            Assert.True(result);
        }
    }
}