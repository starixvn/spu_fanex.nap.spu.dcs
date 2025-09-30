
namespace Fanex.NAP.SPU.DCS.Tests.Features.UserAgent
{
    using AutoFixture;
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.DCS.Features.UserAgent.Criteria;
    using Fanex.NAP.SPU.DCS.Features.UserAgent;
    using NSubstitute;
    using Xunit;

    public class UserAgentDataAccessTests
    {
        private readonly IDynamicRepository repository;
        private readonly IUserAgentDataAccess userAgentDataAccess;
        private readonly IFixture fixture = new Fixture();

        public UserAgentDataAccessTests()
        {
            repository = Substitute.For<IDynamicRepository>();
            userAgentDataAccess = new UserAgentDataAccess(repository);
        }

        [Fact]
        public void UpdateUserAgent_WhenCalled_ExecutesWithCorrectCriteria()
        {
            // Arrange
            var userAgentJson = fixture.Create<string>();

            repository.Execute(Arg.Any<UpdateUserAgentCriteria>()).Returns(0);

            // Act
            var result = userAgentDataAccess.UpdateUserAgent(userAgentJson);

            // Assert
            repository.Received(1).Execute(Arg.Is<UpdateUserAgentCriteria>(c =>
                c.UserAgentJson == userAgentJson &&
                c.GetSettingKey() == "DCS_DC_Transform_UserAgent_UpdateOSBrowser"));
            Assert.Equal(0, result.ErrorCode);
        }
    }
}
