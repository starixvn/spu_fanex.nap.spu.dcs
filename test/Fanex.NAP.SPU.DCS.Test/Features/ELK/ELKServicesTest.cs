
namespace Fanex.NAP.SPU.DCS.Tests.Features.ELK
{
    using System;
    using AutoFixture;
    using Fanex.NAP.SPU.DCS.Features.ELK.Entities;
    using Fanex.NAP.SPU.DCS.Features.ELK;
    using Xunit;
    using NSubstitute;

    public class ELKServicesTest
    {
        private readonly IELKDataAccess elkDataAccess;
        private readonly IELKServices elkServices;
        private readonly IFixture fixture = new Fixture();

        public ELKServicesTest()
        {
            elkDataAccess = Substitute.For<IELKDataAccess>();
            elkServices = new ELKServices(elkDataAccess);
        }

        [Fact]
        public void GetLeagueGroupId_WhenCalled_ReturnsExpectedData()
        {
            // Arrange
            var betDate = fixture.Create<DateTime>();
            var expectedLeagueGroups = fixture.CreateMany<LeagueGroupEntity>();
            elkDataAccess.GetLeagueGroupId(betDate).Returns(expectedLeagueGroups);

            // Act
            var actual = elkServices.GetLeagueGroupId(betDate);

            // Assert
            Assert.Equal(expectedLeagueGroups, actual);
        }

        [Fact]
        public void PrepareStagingForBetMessage_WhenCalled_ExecutesWithCorrectCriteria()
        {
            // Arrange
            var month = fixture.Create<DateTime>();
            var sport = fixture.Create<string>();
            var siteType = fixture.Create<string>();
            var bettingMessageCount = fixture.Create<string>();
            var runningCounter = fixture.Create<int>();

            // Act
            elkServices.PrepareStagingForBetMessage(month, sport, siteType, bettingMessageCount, runningCounter);

            // Assert
            elkDataAccess.Received(1).PrepareStagingForBetMessage(month, sport, siteType, bettingMessageCount, runningCounter);
        }

        [Fact]
        public void PrepareStagingForBetMessageDaily_WhenCalled_ExecutesWithCorrectCriteria()
        {
            // Arrange
            var betDate = fixture.Create<DateTime>();
            var sport = fixture.Create<string>();
            var siteType = fixture.Create<string>();
            var leagueGroupId = fixture.Create<int>();
            var bettingMessageCount = fixture.Create<string>();
            var runningCounter = fixture.Create<int>();

            // Act
            elkServices.PrepareStagingForBetMessageDaily(betDate, sport, siteType, leagueGroupId, bettingMessageCount, runningCounter);

            // Assert
            elkDataAccess.Received(1).PrepareStagingForBetMessageDaily(betDate, sport, siteType, leagueGroupId, bettingMessageCount, runningCounter);
        }

        [Fact]
        public void PrepareStagingForParlayValidationStats_WhenCalled_ExecutesWithCorrectCriteria()
        {
            // Arrange
            var month = fixture.Create<DateTime>();
            var sport = fixture.Create<string>();
            var siteType = fixture.Create<string>();
            var bettingMessageCount = fixture.Create<string>();
            var runningCounter = fixture.Create<int>();

            // Act
            elkServices.PrepareStagingForParlayValidationStats(month, sport, siteType, bettingMessageCount, runningCounter);

            // Assert
            elkDataAccess.Received(1).PrepareStagingForParlayValidationStats(month, sport, siteType, bettingMessageCount, runningCounter);
        }
    }
}
