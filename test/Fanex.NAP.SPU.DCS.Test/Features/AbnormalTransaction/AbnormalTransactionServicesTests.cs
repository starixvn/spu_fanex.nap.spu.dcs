
namespace Fanex.NAP.SPU.DCS.Tests.Features.AbnormalTransaction
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoFixture;
    using Fanex.NAP.SPU.DCS.Features.AbnormalTransaction;
    using Fanex.NAP.SPU.DCS.Features.AbnormalTransaction.Entities;
    using Fanex.NAP.SPU.DCS.Features.AbnormalTransaction.Model;
    using Fanex.NAP.SPU.DCS.JobService.Services;
    using NSubstitute;
    using Xunit;

    public class AbnormalTransactionServicesTests
    {
        private readonly IAbnormalTransactionDataAccess dataAccess;
        private readonly ILogService logService;
        private readonly IAbnormalTransactionServices abnormalServices;
        private readonly IFixture fixture = new Fixture();

        public AbnormalTransactionServicesTests()
        {
            dataAccess = Substitute.For<IAbnormalTransactionDataAccess>();
            logService = Substitute.For<ILogService>();

            abnormalServices = new AbnormalTransactionServices(logService, dataAccess);
        }

        [Fact]
        public async Task PushAbnormalTransaction_WhenCalledWithoutTransactions_UpdatesMaxId()
        {
            // Arrange
            var size = fixture.Create<int>();

            var abnormalTransactionUrl = fixture.Create<string>();

            var ignorePushData = false;

            var abnormalTransactions = new List<AbnormalTransactionEntity>();

            var rangeID = new RangeIDModel { MinID = 0, MaxID = 1 };

            dataAccess.GetAbnormalTransactionsAsync(size).Returns((abnormalTransactions, rangeID));

            // Act
            var result = await abnormalServices.PushAbnormalTransaction(size, abnormalTransactionUrl, ignorePushData);

            // Assert
            Assert.True(result);
            dataAccess.Received(1).UpdateAbnormalTransactions(string.Empty, rangeID.MinID, rangeID.MaxID);
        }

        [Fact]
        public async Task PushAbnormalTransaction_WhenIgnorePushDataIsTrue_ReturnsTrue()
        {
            // Arrange
            var size = fixture.Create<int>();

            var abnormalTransactionUrl = fixture.Create<string>();

            var ignorePushData = true;

            var abnormalTransactions = fixture.CreateMany<AbnormalTransactionEntity>().ToList();

            var rangeID = new RangeIDModel { MinID = 0, MaxID = 0 };

            dataAccess.GetAbnormalTransactionsAsync(size).Returns((abnormalTransactions, rangeID));

            // Act
            var result = await abnormalServices.PushAbnormalTransaction(size, abnormalTransactionUrl, ignorePushData);

            // Assert
            Assert.True(result);
            dataAccess.Received(1).UpdateAbnormalTransactions(Arg.Any<string>(), rangeID.MinID, rangeID.MaxID);
        }
    }
}
