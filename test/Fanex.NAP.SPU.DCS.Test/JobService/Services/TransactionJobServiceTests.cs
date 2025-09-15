namespace Fanex.NAP.SPU.DCS.Tests.JobService.Services
{
    using AutoFixture;
    using Fanex.NAP.SPU.DCS.Features.Transactions;
    using Fanex.NAP.SPU.DCS.Features.Transactions.Entities;
    using Fanex.NAP.SPU.DCS.JobService;
    using Fanex.NAP.SPU.DCS.JobService.Services;
    using NSubstitute;
    using NSubstitute.ExceptionExtensions;
    using System;
    using System.Threading.Tasks;
    using Xunit;

    public class TransactionJobServiceTests
    {
        private ILogService logService;
        private ITransactionServices transactionService;
        private IAppSettings appSettings;

        private readonly Fixture fixture = new Fixture();

        [Fact]
        public async Task Execute_WhenCalled_ShouldReturnTrueAndCallExactFunction()
        {
            // Arrange
            var rawTrans = fixture.CreateMany<RawTransactionEntity>();

            var service = GetService();

            var appSettings = Substitute.For<IAppSettings>();

            appSettings.GetValue<bool>("IsReadyDbExtra").Returns(true);

            transactionService.GetRawTransaction(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<int>()).Returns(rawTrans);
            transactionService.UserAgentProcessing(Arg.Any<long>(), Arg.Any<long>()).Returns(transactionService);
            transactionService.TransactionProcessing(Arg.Any<long>(), Arg.Any<long>()).Returns(transactionService);

            // Act
            var result = await service.Execute(1, 1);

            // Assert
            Assert.True(result);
            transactionService.Received(3).UserAgentProcessing(Arg.Any<long>(), Arg.Any<long>());
            transactionService.Received(3).TransactionProcessing(Arg.Any<long>(), Arg.Any<long>());

            await logService.DidNotReceiveWithAnyArgs().ErrorAsync(Arg.Any<Exception>());
        }

        [Fact]
        public async Task Execute_WhenThrowException_ShouldReturnFalseAndLogError()
        {
            // Arrange
            var rawTrans = fixture.CreateMany<RawTransactionEntity>();
            var service = GetService();

            var appSettings = Substitute.For<IAppSettings>();

            appSettings.GetValue<bool>("IsReadyDbExtra").Returns(true);

            transactionService.GetRawTransaction(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<int>()).Throws(new Exception("Test exception"));

            // Act
            var result = await service.Execute(1, 1);

            // Assert
            Assert.False(result);
            transactionService.DidNotReceiveWithAnyArgs().UserAgentProcessing(Arg.Any<long>(), Arg.Any<long>());
            transactionService.DidNotReceiveWithAnyArgs().TransactionProcessing(Arg.Any<long>(), Arg.Any<long>());

            await logService.Received(1).ErrorAsync(Arg.Any<Exception>());
        }

        [Fact]
        public void ProcessRawFingerprintQueue_WhenCalled_ShouldReturnTrueAndCallTransactionService()
        {
            // Arrange
            var batchSize = 1000;
            var service = GetService();

            transactionService.ProcessRawFingerprintQueue(Arg.Any<int>()).Returns(transactionService);

            // Act
            var result = service.ProcessRawFingerprintQueue(batchSize);

            // Assert
            Assert.True(result);
            transactionService.Received(1).ProcessRawFingerprintQueue(batchSize);
            logService.DidNotReceiveWithAnyArgs().Error(Arg.Any<Exception>());
        }

        [Fact]
        public void ProcessRawFingerprintQueue_WhenThrowException_ShouldReturnFalseAndLogError()
        {
            // Arrange
            var batchSize = 1000;
            var service = GetService();
            var exception = new Exception("Test exception");

            transactionService.ProcessRawFingerprintQueue(Arg.Any<int>()).Throws(exception);

            // Act
            var result = service.ProcessRawFingerprintQueue(batchSize);

            // Assert
            Assert.False(result);
            transactionService.Received(1).ProcessRawFingerprintQueue(batchSize);
            logService.Received(1).Error(exception);
        }

        [Fact]
        public void ProcessRawFingerprintQueue_WithZeroBatchSize_ShouldCallTransactionService()
        {
            // Arrange
            var batchSize = 0;
            var service = GetService();

            transactionService.ProcessRawFingerprintQueue(Arg.Any<int>()).Returns(transactionService);

            // Act
            var result = service.ProcessRawFingerprintQueue(batchSize);

            // Assert
            Assert.True(result);
            transactionService.Received(1).ProcessRawFingerprintQueue(batchSize);
        }

        [Fact]
        public void ProcessRawFingerprintQueue_WithNegativeBatchSize_ShouldCallTransactionService()
        {
            // Arrange
            var batchSize = -1;
            var service = GetService();

            transactionService.ProcessRawFingerprintQueue(Arg.Any<int>()).Returns(transactionService);

            // Act
            var result = service.ProcessRawFingerprintQueue(batchSize);

            // Assert
            Assert.True(result);
            transactionService.Received(1).ProcessRawFingerprintQueue(batchSize);
        }

        public TransactionJobService GetService()
        {

            logService = Substitute.For<ILogService>();
            transactionService = Substitute.For<ITransactionServices>();

            return new TransactionJobService(logService, transactionService);
        }
    }
}
