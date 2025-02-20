
namespace Fanex.NAP.SPU.DCS.Tests.Features.Transaction
{
    using AutoFixture;
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.DCS.Features.Transactions.Criteria;
    using Fanex.NAP.SPU.DCS.Features.Transactions;
    using NSubstitute;
    using Xunit;

    public class TransactionDataAccessTest
    {
        private readonly IDynamicRepository repository;
        private readonly ITransactionDataAccess transactionDataAccess;
        private readonly IFixture fixture = new Fixture();

        public TransactionDataAccessTest()
        {
            repository = Substitute.For<IDynamicRepository>();
            transactionDataAccess = new TransactionDataAccess(repository);
        }

        [Fact]
        public void InsertTransaction_WhenCalled_ExecutesWithCorrectCriteria()
        {
            // Arrange
            var fromId = fixture.Create<long>();
            var toId = fixture.Create<long>();
            bool isReadyDbExtra = true;
            var criteria = fixture.Create<InsertTransactionCriteria>();

            // Act
            transactionDataAccess.InsertTransaction(fromId, toId, isReadyDbExtra);

            // Assert
            var isValid = criteria.IsInvalid();

            repository.Received(1).Execute(Arg.Is<InsertTransactionCriteria>(c =>
                c.FromStagingTransId == fromId &&
                c.ToStagingTransId == toId &&
                c.IsReadyDbExtra == isReadyDbExtra &&
                c.GetSettingKey() == "DCS_ET_Transform_Transaction_Insert"));
        }

        [Fact]
        public void InserUserAgent_WhenCalled_ExecutesWithCorrectCriteria()
        {
            // Arrange
            var fromId = fixture.Create<long>();
            var toId = fixture.Create<long>();
            bool isReadyDbExtra = true;
            var criteria = fixture.Create<InsertUserAgentCriteria>();

            // Act
            transactionDataAccess.InserUserAgent(fromId, toId, isReadyDbExtra);

            // Assert
            var isValid = criteria.IsInvalid();

            repository.Received(1).Execute(Arg.Is<InsertUserAgentCriteria>(c =>
                c.FromStagingTransId == fromId &&
                c.ToStagingTransId == toId &&
                c.IsReadyDbExtra == isReadyDbExtra &&
                c.GetSettingKey() == "DCS_ET_Transform_UserAgent_Insert"));
        }
    }
}
