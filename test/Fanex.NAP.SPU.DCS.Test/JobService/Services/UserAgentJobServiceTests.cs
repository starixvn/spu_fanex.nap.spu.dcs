namespace Fanex.NAP.SPU.DCS.Tests.JobService.Services
{
    using AutoFixture;
    using Fanex.NAP.SPU.DCS.Features.UserAgent;
    using Fanex.NAP.SPU.DCS.Features.UserAgent.Entities;
    using Fanex.NAP.SPU.DCS.JobService.Services;
    using Fanex.NAP.SPU.DCS.Shares.Models;
    using NSubstitute;
    using NSubstitute.ExceptionExtensions;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using UAParser.FormFactor.Models;
    using Xunit;

    public class UserAgentJobServiceTests
    {
        private ILogService logService;
        private IUserAgentServices userAgentService;

        private readonly Fixture fixture = new();

        [Fact]
        public async Task Execute_WithNegativeSize_ReturnsFalse()
        {
            // Arrange
            var service = GetService();

            // Act
            var result = await service.Execute(-1);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task Execute_WithNoUserAgents_ReturnsTrue()
        {
            // Arrange
            var service = GetService();

            userAgentService.GetUserAgent(Arg.Any<int>()).Returns(new List<UserAgentEntity>());

            // Act
            var result = await service.Execute(10);

            // Assert
            Assert.True(result);
            userAgentService.DidNotReceiveWithAnyArgs().TransformUserAgent(Arg.Any<List<UserAgentEntity>>());
            userAgentService.DidNotReceiveWithAnyArgs().UpdateUserAgent(Arg.Any<string>());
        }

        [Fact]
        public async Task Execute_WhenCalled_ShouldReturnTrueAndCallExactFunction()
        {
            // Arrange
            var userAgents = fixture.Create<List<UserAgentEntity>>();
            var transformUserAgent = fixture.Create<List<UserAgentModel>>();

            var service = GetService();

            userAgentService.GetUserAgent(Arg.Any<int>()).Returns(userAgents);

            userAgentService.TransformUserAgent(userAgents).Returns(transformUserAgent);

            // Act
            var result = await service.Execute(1);

            // Assert
            Assert.True(result);

            userAgentService.Received().UpdateUserAgent(Arg.Any<string>());
            await logService.DidNotReceiveWithAnyArgs().ErrorAsync(Arg.Any<Exception>());
        }

        [Fact]
        public async Task Execute_WhenThrowException_ShouldReturnFalseAndCallExactFunction()
        {
            // Arrange

            var service = GetService();

            userAgentService.GetUserAgent(Arg.Any<int>()).Throws(new Exception("Simulate Exception"));

            // Act
            var result = await service.Execute(1);

            // Assert
            Assert.False(result);

            await logService.Received().ErrorAsync(Arg.Any<Exception>());
            userAgentService.DidNotReceiveWithAnyArgs().TransformUserAgent(Arg.Any<List<UserAgentEntity>>());
            userAgentService.DidNotReceiveWithAnyArgs().UpdateUserAgent(Arg.Any<string>());
        }

        [Fact]
        public async Task Execute_WhenThrowExceptionOfTransformUserAgent_ShouldReturnFalseAndCallExactFunction()
        {
            // Arrange

            var userAgents = fixture.Create<List<UserAgentEntity>>();

            var transformUserAgent = fixture.Create<List<UserAgentModel>>();

            var service = GetService();

            userAgentService.GetUserAgent(Arg.Any<int>()).Returns(userAgents);

            userAgentService.TransformUserAgent(userAgents).Throws(new Exception("Simulate Exception"));

            // Act
            var result = await service.Execute(1);

            // Assert
            Assert.False(result);

            await logService.Received().ErrorAsync(Arg.Any<Exception>());
            userAgentService.DidNotReceiveWithAnyArgs().UpdateUserAgent(Arg.Any<string>());
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

            return new UserAgentJobService(logService, userAgentService);
        }
    }
}