
namespace Fanex.NAP.SPU.DCS.Tests.Features.SystemSetting
{
    using System.Threading.Tasks;
    using AutoFixture;
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.DCS.Features.SystemSetting.Criteria;
    using Fanex.NAP.SPU.DCS.Features.SystemSetting;
    using NSubstitute;
    using Xunit;

    public class SystemSettingDataAccessTest
    {
        private readonly IDynamicRepository repository;
        private readonly ISystemSettingDataAccess systemSettingDataAccess;
        private readonly IFixture fixture = new Fixture();

        public SystemSettingDataAccessTest()
        {
            repository = Substitute.For<IDynamicRepository>();
            systemSettingDataAccess = new SystemSettingDataAccess(repository);
        }

        [Fact]
        public async Task UpdateDCSystemSettingByID_WhenCalled_ExecutesWithCorrectCriteria()
        {
            // Arrange
            var id = fixture.Create<int>();
            var value = fixture.Create<string>();
            var criteria = fixture.Create<UpdateDCSystemSettingParameterCriteria>();

            // Act
            await systemSettingDataAccess.UpdateDCSystemSettingByID(id, value);

            // Assert
            await repository.Received(1).ExecuteAsync(Arg.Is<UpdateDCSystemSettingParameterCriteria>(c =>
                c.ParameterID == id &&
                c.ParameterValue == value &&
                c.GetSettingKey() == "DCS_DC_SystemSetting_UpdateParameterByID"));
        }
    }
}