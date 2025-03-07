
namespace Fanex.NAP.SPU.DCS.Tests.JobService.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoFixture;
    using Fanex.NAP.SPU.DCS.Features.MBTransactions;
    using Fanex.NAP.SPU.DCS.Features.MBTransactions.Entities;
    using Fanex.NAP.SPU.DCS.JobService.Services;
    using NSubstitute;
    using Xunit;

    public class MBTransactionJobServiceTests
    {
        private readonly ILogService logService;
        private readonly IMBTransactionServices transactionService;
        private readonly MBTransactionJobService mbTransactionJobService;

        public MBTransactionJobServiceTests()
        {
            logService = Substitute.For<ILogService>();
            transactionService = Substitute.For<IMBTransactionServices>();
            mbTransactionJobService = new MBTransactionJobService(logService, transactionService);
        }

        [Fact]
        public async Task Execute_WithValidParameters_ReturnsTrue()
        {
            // Arrange
            int numberOfTrans = 10;
            int numberOfBatch = 2;

            // Act
            var result = await mbTransactionJobService.Execute(numberOfTrans, numberOfBatch);

            // Assert
            Assert.True(result);
            await logService.DidNotReceiveWithAnyArgs().ErrorAsync(Arg.Any<Exception>());
        }

        [Fact]
        public async Task Execute_WithException_ReturnsFalseAndLogsError()
        {
            // Arrange
            int numberOfTrans = 10;
            int numberOfBatch = 2;

            transactionService.When(x => x.GetMBRawTransaction(numberOfTrans, numberOfBatch))
                              .Do(x => { throw new Exception("Test exception"); });

            // Act
            var result = await mbTransactionJobService.Execute(numberOfTrans, numberOfBatch);

            // Assert
            Assert.False(result);
            await logService.Received(1).ErrorAsync(Arg.Any<Exception>());
        }

        [Fact]
        public async Task MBTransformTransaction_NullRawTransactions_DoesNothing()
        {
            // Arrange
            transactionService.GetMBRawTransaction(100, 2).Returns((List<MBRawTransactionEntity>)null);

            // Act
            await mbTransactionJobService.Execute(100, 2);

            // Assert
            transactionService.DidNotReceive().InsertMBTransaction(Arg.Any<ulong>(), Arg.Any<ulong>());
        }

        [Fact]
        public async Task MBTransformTransaction_EmptyRawTransactions_DoesNothing()
        {
            // Arrange
            transactionService.GetMBRawTransaction(100, 2).Returns(new List<MBRawTransactionEntity>());

            // Act
            await mbTransactionJobService.Execute(100, 2);

            // Assert
            transactionService.DidNotReceive().InsertMBTransaction(Arg.Any<ulong>(), Arg.Any<ulong>());
        }

        [Fact]
        public async Task MBTransformTransaction_ValidTransactions_InsertsEachTransaction()
        {
            // Arrange
            var fixture = new Fixture();
            var rawTrans = fixture.Create<IEnumerable<MBRawTransactionEntity>>();
            transactionService.GetMBRawTransaction(100, 2).Returns(rawTrans);

            // Act
            await mbTransactionJobService.Execute(100, 2);

            // Assert
            transactionService.Received(3).InsertMBTransaction(Arg.Any<ulong>(), Arg.Any<ulong>());
        }
    }
}