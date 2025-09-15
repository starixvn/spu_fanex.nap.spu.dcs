
namespace Fanex.NAP.SPU.DCS.Tests.Features.Transaction
{
    using AutoFixture;
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.DCS.Features.Transactions.Criteria;
    using Fanex.NAP.SPU.DCS.Features.Transactions;
    using NSubstitute;
    using Xunit;

    public class TransactionDataAccessTests
    {
        private readonly IDynamicRepository repository;
        private readonly ITransactionDataAccess transactionDataAccess;
        private readonly IFixture fixture = new Fixture();

        public TransactionDataAccessTests()
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

            var criteria = fixture.Create<InsertTransactionCriteria>();

            // Act
            transactionDataAccess.InsertTransaction(fromId, toId);

            // Assert
            var isValid = criteria.IsInvalid();

            repository.Received(1).Execute(Arg.Is<InsertTransactionCriteria>(c =>
                c.FromStagingTransId == fromId &&
                c.ToStagingTransId == toId &&
                c.GetSettingKey() == "DCS_DC_Transform_Transaction_Insert"));
        }

        [Fact]
        public void InserUserAgent_WhenCalled_ExecutesWithCorrectCriteria()
        {
            // Arrange
            var fromId = fixture.Create<long>();

            var toId = fixture.Create<long>();

            var criteria = fixture.Create<InsertUserAgentCriteria>();

            // Act
            transactionDataAccess.InserUserAgent(fromId, toId);

            // Assert
            var isValid = criteria.IsInvalid();

            repository.Received(1).Execute(Arg.Is<InsertUserAgentCriteria>(c =>
                c.FromStagingTransId == fromId &&
                c.ToStagingTransId == toId &&
                c.GetSettingKey() == "DCS_DC_Transform_UserAgent_Insert"));
        }

        [Fact]
        public void ProcessRawFingerprintQueue_WhenCalled_ExecutesWithCorrectCriteria()
        {
            // Arrange
            var batchSize = fixture.Create<int>();

            // Act
            transactionDataAccess.ProcessRawFingerprintQueue(batchSize);

            // Assert
            repository.Received(1).Execute(Arg.Is<ProcessRawFingerprintQueueCriteria>(c =>
                c.BatchSize == batchSize &&
                c.GetSettingKey() == "DCS_DC_Transform_RawFingerprintQueue_Process"));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(100)]
        [InlineData(1000)]
        [InlineData(-1)]
        public void ProcessRawFingerprintQueue_WithDifferentBatchSizes_ExecutesWithCorrectCriteria(int batchSize)
        {
            // Act
            transactionDataAccess.ProcessRawFingerprintQueue(batchSize);

            // Assert
            repository.Received(1).Execute(Arg.Is<ProcessRawFingerprintQueueCriteria>(c =>
                c.BatchSize == batchSize &&
                c.GetSettingKey() == "DCS_DC_Transform_RawFingerprintQueue_Process"));
        }
    }
}
