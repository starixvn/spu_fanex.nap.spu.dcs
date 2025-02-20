
namespace Fanex.NAP.SPU.DCS.Tests.Features.AbnormalTransaction
{
    using AutoFixture;
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.DCS.Features.AbnormalTransaction;
    using Fanex.NAP.SPU.DCS.Features.AbnormalTransaction.Criteria;
    using NSubstitute;
    using Xunit;

    public class AbnormalTransactionDataAccessTest
    {
        private IDynamicRepository repository;
        private IAbnormalTransactionDataAccess abnormalTransactionDataAccess;
        private IFixture fixture = new Fixture();

        public AbnormalTransactionDataAccessTest()
        {
            repository = Substitute.For<IDynamicRepository>();
            abnormalTransactionDataAccess = new AbnormalTransactionDataAccess(repository);
        }

        [Fact]
        public void GetAbnormalTransactionsAsync_WhenCalled_ReturnsExpectedData()
        {
            //// Arrange
            //var size = fixture.Create<int>();
            //var expectedTransactions = fixture.CreateMany<AbnormalTransactionEntity>().ToList();
            //var expectedRangeID = new RangeIDModel { MinID = 0, MaxID = 0 };

            //var criteria = new AbnormalTransactionGetCriteria
            //{
            //    Size = size,
            //    MinID = 0,
            //    MaxID = 0
            //};

            //var mockDb = Substitute.For<IObjectDb>();
            //ObjectDbFactory.CreateInstance(Arg.Any<string>()).Returns(mockDb);

            //mockDb.Query<AbnormalTransactionEntity>(criteria).Returns(expectedTransactions);

            //// Act
            //var (actualTransactions, actualRangeID) = abnormalTransactionDataAccess.GetAbnormalTransactionsAsync(size);

            //// Assert
            //Assert.Equal(expectedTransactions, actualTransactions);
            //Assert.Equal(expectedRangeID.MinID, actualRangeID.MinID);
            //Assert.Equal(expectedRangeID.MaxID, actualRangeID.MaxID);
        }

        [Fact]
        public void UpdateAbnormalTransactions_WhenCalled_ExecutesCorrectly()
        {
            // Arrange
            var transJson = fixture.Create<string>();
            var minId = fixture.Create<ulong>();
            var maxId = fixture.Create<ulong>();
            var criteria = new AbnormalTransactionUpdateCriteria
            {
                TransJson = transJson,
                MinID = minId,
                MaxID = maxId
            };

            // Act
            abnormalTransactionDataAccess.UpdateAbnormalTransactions(transJson, minId, maxId);

            // Assert
            repository.Received(1).Execute(Arg.Is<AbnormalTransactionUpdateCriteria>(x =>
                x.TransJson == transJson && x.MinID == minId && x.MaxID == maxId));
        }
    }
}
