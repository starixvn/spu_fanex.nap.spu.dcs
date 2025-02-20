namespace Fanex.NAP.SPU.DCS.Tests.JobService.Services
{
    using Fanex.NAP.SPU.DCS.Features.UserAgent;
    using Fanex.NAP.SPU.DCS.JobService.Services;
    using Fanex.NAP.SPU.DCS.JobService;
    using NSubstitute;
    using AutoFixture;
    using System.Threading.Tasks;
    using System;
    using Xunit;
    using Fanex.NAP.SPU.DCS.Features.UserAgent.Entities;
    using System.Collections.Generic;
    using Fanex.NAP.SPU.DCS.Shares.Models;
    using NSubstitute.ExceptionExtensions;
    using UAParser.FormFactor.Models;

    public class UserAgentJobServiceTests
    {
        private ILogService logService;
        private IUserAgentServices userAgentService;
        private IAppSettings appSettings;

        private readonly Fixture fixture = new Fixture();

        [Fact]
        public async Task Execute_WhenCalled_ShouldReturnTrueAndCallExactFunction()
        {
            // Arrange
            var userAgents = fixture.Create<List<UserAgentEntity>>();
            var transformUserAgent = fixture.Create<List<UserAgentModel>>();

            var service = GetService();

            userAgentService.GetUserAgent(Arg.Any<int>(), Arg.Any<bool>()).Returns(userAgents);
            userAgentService.TransformUserAgent(userAgents).Returns(transformUserAgent);

            // Act
            var result = await service.Execute(1, true);

            // Assert
            Assert.True(result);

            userAgentService.Received().UpdateUserAgent(Arg.Any<string>(), Arg.Any<bool>());
            await logService.DidNotReceiveWithAnyArgs().ErrorAsync(Arg.Any<Exception>());
        }

        [Fact]
        public async Task Execute_WhenThrowException_ShouldReturnTrueAndCallExactFunction()
        {
            // Arrange

            var service = GetService();

            userAgentService.GetUserAgent(Arg.Any<int>(), Arg.Any<bool>()).Throws(new Exception("Simulate Exception"));

            // Act
            var result = await service.Execute(1, true);

            // Assert
            Assert.False(result);

            await logService.Received().ErrorAsync(Arg.Any<Exception>());
            userAgentService.DidNotReceiveWithAnyArgs().TransformUserAgent(Arg.Any<List<UserAgentEntity>>());
            userAgentService.DidNotReceiveWithAnyArgs().UpdateUserAgent(Arg.Any<string>(), Arg.Any<bool>());
        }

        [Fact]
        public void ConvertUserAgent_WhenCalled_ReturnsExpectedData()
        {
            // Arrange
            var userAgent = fixture.Create<string>();
            var expectedClientInfo = fixture.Create<ClientInfo>();
            var service = GetService();

            userAgentService.ConvertUserAgent(userAgent).Returns(expectedClientInfo);

            // Act
            var actual = service.ConvertUserAgent(userAgent);

            // Assert
            Assert.Equal(expectedClientInfo, actual);
        }


        public UserAgentJobService GetService()
        {
            logService = Substitute.For<ILogService>();
            userAgentService = Substitute.For<IUserAgentServices>();
            appSettings = Substitute.For<IAppSettings>();

            return new UserAgentJobService(logService, userAgentService, appSettings);
        }
    }
}
