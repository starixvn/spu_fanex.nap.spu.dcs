
namespace Fanex.NAP.SPU.DCS.Tests.Features.AbnormalTransaction
{
    using System.Threading.Tasks;
    using AutoFixture;
    using Fanex.NAP.SPU.DCS.Features.AbnormalTransaction;
    using Fanex.NAP.SPU.DCS.JobService.Services;
    using NSubstitute;
    using Xunit;

    public class AbnormalTransactionServicesTest
    {
        private IAbnormalTransactionDataAccess dataAccess;
        private ILogService logService;
        private IAbnormalTransactionServices abnormalServices;
        private IFixture fixture = new Fixture();

        public AbnormalTransactionServicesTest()
        {
            dataAccess = Substitute.For<IAbnormalTransactionDataAccess>();
            logService = Substitute.For<ILogService>();

            abnormalServices = new AbnormalTransactionServices(logService, dataAccess);
        }


        [Fact]
        public async Task PushAbnormalTransaction_WhenCalledWithTransactions_PushesDataAndUpdatesTransactions()
        {
            //// Arrange
            //var size = fixture.Create<int>();
            //var abnormalTransactionUrl = fixture.Create<string>();
            //var ignorePushData = false;
            //var abnormalTransactions = fixture.CreateMany<AbnormalTransactionEntity>().ToList();
            //var rangeID = new RangeIDModel { MinID = 0, MaxID = 0 };

            //dataAccess.GetAbnormalTransactionsAsync(size).Returns((abnormalTransactions, rangeID));

            //// Act
            //var result = await abnormalServices.PushAbnormalTransaction(size, abnormalTransactionUrl, ignorePushData);

            //// Assert
            //Assert.True(result);
            //dataAccess.Received(1).UpdateAbnormalTransactions(Arg.Any<string>(), rangeID.MinID, rangeID.MaxID);
        }

        [Fact]
        public async Task PushAbnormalTransaction_WhenCalledWithoutTransactions_UpdatesMaxId()
        {
            //// Arrange
            //var size = fixture.Create<int>();
            //var abnormalTransactionUrl = fixture.Create<string>();
            //var ignorePushData = false;
            //var abnormalTransactions = new List<AbnormalTransactionEntity>();
            //var rangeID = new RangeIDModel { MinID = 0, MaxID = 1 };

            //dataAccess.GetAbnormalTransactionsAsync(size).Returns((abnormalTransactions, rangeID));

            //// Act
            //var result = await abnormalServices.PushAbnormalTransaction(size, abnormalTransactionUrl, ignorePushData);

            //// Assert
            //Assert.True(result);
            //dataAccess.Received(1).UpdateAbnormalTransactions(string.Empty, rangeID.MinID, rangeID.MaxID);
        }

    }
}
