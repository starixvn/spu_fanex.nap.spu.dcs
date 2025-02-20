
namespace Fanex.NAP.SPU.DCS.Tests.JobService.Services
{
    using System;
    using System.Threading.Tasks;
    using Fanex.NAP.SPU.DCS.Features.MBTransactions;
    using Fanex.NAP.SPU.DCS.JobService.Services;
    using NSubstitute;
    using Xunit;

    public class MBTransactionJobServiceTest
    {
        private readonly ILogService logService;
        private readonly IMBTransactionServices transactionService;
        private readonly MBTransactionJobService mbTransactionJobService;

        public MBTransactionJobServiceTest()
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
        public async Task Execute_WithInvalidParameters_ReturnsFalseAndLogsError()
        {
            // Arrange
            int numberOfTrans = 0;
            int numberOfBatch = 0;

            // Act
            var result = await mbTransactionJobService.Execute(numberOfTrans, numberOfBatch);

            // Assert
            Assert.False(result);
            await logService.Received(1).ErrorAsync(Arg.Any<Exception>());
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
    }
}