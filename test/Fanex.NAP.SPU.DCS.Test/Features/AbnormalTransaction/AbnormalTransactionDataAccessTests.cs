
namespace Fanex.NAP.SPU.DCS.Tests.Features.AbnormalTransaction
{
    using AutoFixture;
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.DCS.Features.AbnormalTransaction;
    using Fanex.NAP.SPU.DCS.Features.AbnormalTransaction.Criteria;
    using NSubstitute;
    using Xunit;

    public class AbnormalTransactionDataAccessTests
    {
        private IDynamicRepository repository;
        private IAbnormalTransactionDataAccess abnormalTransactionDataAccess;
        private IFixture fixture = new Fixture();

        public AbnormalTransactionDataAccessTests()
        {
            repository = Substitute.For<IDynamicRepository>();
            abnormalTransactionDataAccess = new AbnormalTransactionDataAccess(repository);
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
