
namespace Fanex.NAP.SPU.DCS.Tests.Features.Transaction
{
    using System.Collections.Generic;
    using Fanex.NAP.SPU.DCS.Features.Transactions;
    using Fanex.NAP.SPU.DCS.Features.Transactions.Entities;
    using FluentAssertions;
    using NSubstitute;
    using Xunit;

    public class TransactionServicesTests
    {
        private readonly ITransactionDataAccess transactionDataAccess;
        private readonly ITransactionServices transactionServices;

        public TransactionServicesTests()
        {
            transactionDataAccess = Substitute.For<ITransactionDataAccess>();

            transactionServices = new TransactionServices(transactionDataAccess);
        }

        [Theory]
        [InlineData(0, 2, 1)]
        [InlineData(0, 2, 2)]
        public void GetRawTransaction_ReturnListRawTrans(int isProcessed, int numberOfTrans, int numberOfBatch)
        {
            var isReadyDbExtra = false;
            var result = new List<RawTransactionEntity> {
                new RawTransactionEntity
                {
                    MinTransId = 1,
                    MaxTransId = 2
                },
                new RawTransactionEntity
                {
                    MinTransId = 3,
                    MaxTransId = 4
                },
                new RawTransactionEntity
                {
                    MinTransId = 5,
                    MaxTransId = 6
                },
                new RawTransactionEntity
                {
                    MinTransId = 7,
                    MaxTransId = 8
                }
            };

            transactionDataAccess.GetRawTransaction(isProcessed, numberOfTrans, numberOfBatch, isReadyDbExtra).Returns(result);

            var expectedResult = transactionServices.GetRawTransaction(isProcessed, numberOfTrans, numberOfBatch, isReadyDbExtra);

            expectedResult.Should().BeEquivalentTo(result);
        }

        [Fact]
        public void TransactionProcessing_WhenCall_ReturnExactData()
        {
            var fromId = 1;
            var toId = 2;
            var isReadyDbExtra = true;

            var expectedResult = transactionServices.TransactionProcessing(fromId, toId, isReadyDbExtra);

            Assert.Same(expectedResult, transactionServices);
        }

        [Fact]
        public void UserAgentProcessing_WhenCall_ReturnExactData()
        {
            var fromId = 1;
            var toId = 2;
            var isReadyDbExtra = true;

            var expectedResult = transactionServices.UserAgentProcessing(fromId, toId, isReadyDbExtra);

            Assert.Same(expectedResult, transactionServices);
        }

        [Fact]
        public void ProcessRawFingerprintQueue_WhenCalled_ShouldCallDataAccessAndReturnSelf()
        {
            // Arrange
            var batchSize = 1000;

            // Act
            var result = transactionServices.ProcessRawFingerprintQueue(batchSize);

            // Assert
            Assert.Same(result, transactionServices);
            transactionDataAccess.Received(1).ProcessRawFingerprintQueue(batchSize);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(100)]
        [InlineData(1000)]
        [InlineData(-1)]
        public void ProcessRawFingerprintQueue_WithDifferentBatchSizes_ShouldCallDataAccess(int batchSize)
        {
            // Act
            var result = transactionServices.ProcessRawFingerprintQueue(batchSize);

            // Assert
            Assert.Same(result, transactionServices);
            transactionDataAccess.Received(1).ProcessRawFingerprintQueue(batchSize);
        }
    }
}
