namespace Fanex.NAP.SPU.DCS.Tests.JobService.Services
{
    using Fanex.NAP.SPU.DCS.Features.Transactions;
    using Fanex.NAP.SPU.DCS.JobService.Services;
    using Fanex.NAP.SPU.DCS.JobService;
    using NSubstitute;
    using AutoFixture;
    using System.Threading.Tasks;
    using System;
    using Xunit;
    using Fanex.NAP.SPU.DCS.Features.Transactions.Entities;
    using NSubstitute.ExceptionExtensions;

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

            appSettings.GetValue<bool>("IsReadyDbExtra").Returns(true);

            transactionService.GetRawTransaction(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<bool>()).Returns(rawTrans);
            transactionService.UserAgentProcessing(Arg.Any<long>(), Arg.Any<long>(), Arg.Any<bool>()).Returns(transactionService);
            transactionService.TransactionProcessing(Arg.Any<long>(), Arg.Any<long>(), Arg.Any<bool>()).Returns(transactionService);

            // Act
            var result = await service.Execute(1, 1, true);

            // Assert
            Assert.True(result);
            transactionService.Received(3).UserAgentProcessing(Arg.Any<long>(), Arg.Any<long>(), Arg.Any<bool>());
            transactionService.Received(3).TransactionProcessing(Arg.Any<long>(), Arg.Any<long>(), Arg.Any<bool>());

            await logService.DidNotReceiveWithAnyArgs().ErrorAsync(Arg.Any<Exception>());
        }

        [Fact]
        public async Task Execute_WhenThrowException_ShouldReturnTrueAndCallExactFunction()
        {
            // Arrange
            var rawTrans = fixture.CreateMany<RawTransactionEntity>();
            var service = GetService();

            appSettings.GetValue<bool>("IsReadyDbExtra").Returns(true);

            transactionService.GetRawTransaction(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<bool>()).Returns(rawTrans);

            // Act
            var result = await service.Execute(1, 1, true);

            // Assert
            Assert.False(result);
            transactionService.DidNotReceiveWithAnyArgs().UserAgentProcessing(Arg.Any<long>(), Arg.Any<long>(), Arg.Any<bool>());
            transactionService.DidNotReceiveWithAnyArgs().TransactionProcessing(Arg.Any<long>(), Arg.Any<long>(), Arg.Any<bool>());

            await logService.Received().ErrorAsync(Arg.Any<Exception>());
        }

        public TransactionJobService GetService()
        {

            logService = Substitute.For<ILogService>();
            transactionService = Substitute.For<ITransactionServices>();
            appSettings = Substitute.For<IAppSettings>();

            return new TransactionJobService(logService, transactionService, appSettings);
        }
    }
}
