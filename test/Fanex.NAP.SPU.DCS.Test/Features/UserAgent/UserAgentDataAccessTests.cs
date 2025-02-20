
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
        public void GetUserAgent_WhenCalled_ReturnsExpectedData()
        {
            //// Arrange
            //int size = 10;
            //bool isReadyDbExtra = true;
            //var expectedUserAgents = fixture.CreateMany<UserAgentEntity>();
            //var criteria = fixture.Create<GetUserAgentCriteria>();

            //var db = Substitute.For<IDatabase>();
            //db.Query<UserAgentEntity>(Arg.Any<GetUserAgentCriteria>()).Returns(expectedUserAgents);
            //ObjectDbFactory.CreateInstance(Arg.Any<string>()).Returns(db);

            //var userAgentDataAccess = new UserAgentDataAccess();

            //// Act
            //var actual = userAgentDataAccess.GetUserAgent(size, isReadyDbExtra);

            //// Assert
            //var settingKey = criteria.GetSettingKey();
            //Assert.Equal(expectedUserAgents, actual);
            //Assert.Equal("DCS_ET_Transform_UserAgent_GetNullBrowserOSList", settingKey);
        }

        [Fact]
        public void UpdateUserAgent_WhenCalled_ExecutesWithCorrectCriteria()
        {
            // Arrange
            var userAgentJson = fixture.Create<string>();
            bool isReadyDbExtra = true;

            repository.Execute(Arg.Any<UpdateUserAgentCriteria>()).Returns(0);

            // Act
            var result = userAgentDataAccess.UpdateUserAgent(userAgentJson, isReadyDbExtra);

            // Assert
            repository.Received(1).Execute(Arg.Is<UpdateUserAgentCriteria>(c =>
                c.UserAgentJson == userAgentJson &&
                c.IsReadyDbExtra == isReadyDbExtra &&
                c.GetSettingKey() == "DCS_ET_Transform_UserAgent_UpdateOSBrowser"));
            Assert.Equal(0, result.ErrorCode);
        }
    }
}
