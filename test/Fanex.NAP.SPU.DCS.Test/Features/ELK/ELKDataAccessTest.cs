
namespace Fanex.NAP.SPU.DCS.Tests.Features.ELK
{
    using System;
    using AutoFixture;
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.DCS.Features.ELK.Criteria;
    using Fanex.NAP.SPU.DCS.Features.ELK;
    using NSubstitute;
    using Xunit;

    public class ELKDataAccessTest
    {
        private readonly IDynamicRepository repository;
        private readonly IELKDataAccess elkDataAccess;
        private readonly IFixture fixture = new Fixture();

        public ELKDataAccessTest()
        {
            repository = Substitute.For<IDynamicRepository>();
            elkDataAccess = new ELKDataAccess(repository);
        }

        [Theory]
        [InlineData(1, "Task_PrepareStaging_ParlayValidationStats1")]
        [InlineData(2, "Task_PrepareStaging_ParlayValidationStats2")]
        public void PrepareStagingForParlayValidationStats_WhenCalled_ExecutesWithCorrectCriteria(int runningCounter, string expectedSettingKey)
        {
            // Arrange
            var month = fixture.Create<DateTime>();
            var sport = fixture.Create<string>();
            var siteType = fixture.Create<string>();
            var bettingMessageCount = fixture.Create<string>();

            // Act
            elkDataAccess.PrepareStagingForParlayValidationStats(month, sport, siteType, bettingMessageCount, runningCounter);

            // Assert
            if (runningCounter == 1)
            {
                repository.Received(1).Execute(Arg.Is<PrepareStaging_ParlayValidationStatsCriteria1>(criteria =>
                    criteria.Month == month &&
                    criteria.Sport == sport &&
                    criteria.SiteType == siteType &&
                    criteria.BettingMessageCount == bettingMessageCount &&
                    criteria.GetSettingKey() == expectedSettingKey));
            }
            else
            {
                repository.Received(1).Execute(Arg.Is<PrepareStaging_ParlayValidationStatsCriteria2>(criteria =>
                    criteria.Month == month &&
                    criteria.Sport == sport &&
                    criteria.SiteType == siteType &&
                    criteria.BettingMessageCount == bettingMessageCount &&
                    criteria.GetSettingKey() == expectedSettingKey));
            }
        }

        [Theory]
        [InlineData(1, "Task_PrepareStaging_BetMessageELK1")]
        [InlineData(2, "Task_PrepareStaging_BetMessageELK2")]
        public void PrepareStagingForBetMessage_WhenCalled_ExecutesWithCorrectCriteria(int runningCounter, string expectedSettingKey)
        {
            // Arrange
            var month = fixture.Create<DateTime>();
            var sport = fixture.Create<string>();
            var siteType = fixture.Create<string>();
            var bettingMessageCount = fixture.Create<string>();

            // Act
            elkDataAccess.PrepareStagingForBetMessage(month, sport, siteType, bettingMessageCount, runningCounter);

            // Assert
            if (runningCounter == 1)
            {
                repository.Received(1).Execute(Arg.Is<PrepareStaging_BetMessageCriteria1>(criteria =>
                    criteria.Month == month &&
                    criteria.Sport == sport &&
                    criteria.SiteType == siteType &&
                    criteria.BettingMessageCount == bettingMessageCount &&
                    criteria.GetSettingKey() == expectedSettingKey));
            }
            else
            {
                repository.Received(1).Execute(Arg.Is<PrepareStaging_BetMessageCriteria2>(criteria =>
                    criteria.Month == month &&
                    criteria.Sport == sport &&
                    criteria.SiteType == siteType &&
                    criteria.BettingMessageCount == bettingMessageCount &&
                    criteria.GetSettingKey() == expectedSettingKey));
            }
        }

        [Theory]
        [InlineData(1, "Task_PrepareStaging_BetMessageELKDaily1")]
        [InlineData(2, "Task_PrepareStaging_BetMessageELKDaily2")]
        public void PrepareStagingForBetMessageDaily_WhenCalled_ExecutesWithCorrectCriteria(int runningCounter, string expectedSettingKey)
        {
            // Arrange
            var betDate = fixture.Create<DateTime>();
            var sport = fixture.Create<string>();
            var siteType = fixture.Create<string>();
            var leagueGroupId = fixture.Create<int>();
            var bettingMessageCount = fixture.Create<string>();

            // Act
            elkDataAccess.PrepareStagingForBetMessageDaily(betDate, sport, siteType, leagueGroupId, bettingMessageCount, runningCounter);

            // Assert
            if (runningCounter == 1)
            {
                repository.Received(1).Execute(Arg.Is<PrepareStaging_BetMessageELKDailyCriteria1>(criteria =>
                    criteria.BetDate == betDate &&
                    criteria.Sport == sport &&
                    criteria.SiteType == siteType &&
                    criteria.LeagueGroupID == leagueGroupId &&
                    criteria.BettingMessageCount == bettingMessageCount &&
                    criteria.GetSettingKey() == expectedSettingKey));
            }
            else
            {
                repository.Received(1).Execute(Arg.Is<PrepareStaging_BetMessageELKDailyCriteria2>(criteria =>
                    criteria.BetDate == betDate &&
                    criteria.Sport == sport &&
                    criteria.SiteType == siteType &&
                    criteria.LeagueGroupID == leagueGroupId &&
                    criteria.BettingMessageCount == bettingMessageCount &&
                    criteria.GetSettingKey() == expectedSettingKey));
            }
        }
    }
}
