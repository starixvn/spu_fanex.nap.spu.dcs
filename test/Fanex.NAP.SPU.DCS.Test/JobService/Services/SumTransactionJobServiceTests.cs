namespace Fanex.NAP.SPU.DCS.Tests.JobService.Services
{
    using AutoFixture;
    using Fanex.NAP.SPU.DCS.Features.Monitoring.Model;
    using Fanex.NAP.SPU.DCS.Features.Monitoring;
    using NSubstitute;
    using System.Threading.Tasks;
    using System;
    using Xunit;
    using Fanex.NAP.SPU.DCS.JobService.Services;
    using NSubstitute.ExceptionExtensions;

    public class SumTransactionJobServiceTests
    {
        private ILogService logService;
        private IMonitoringServices monitoringServices;

        private readonly Fixture fixture = new Fixture();

        [Fact]
        public async Task Execute_WhenCalled_ShouldReturnTrue()
        {
            // Arrange
            var sumTransaction = fixture.Create<SumTransactionModel>();
            var service = GetService();

            monitoringServices.GetSumTransaction().Returns(sumTransaction);
            monitoringServices.UpdateSumRawTransaction(sumTransaction).Returns(1);

            // Act
            var result = await service.Execute();

            // Assert
            Assert.True(result);
            await logService.DidNotReceiveWithAnyArgs().ErrorAsync(Arg.Any<Exception>());
        }

        [Fact]
        public async Task Execute_WhenThrowException_ShouldReturnFalseAndCallExactFunction()
        {
            // Arrange
            var sumTransaction = fixture.Create<SumTransactionModel>();
            var service = GetService();

            monitoringServices.GetSumTransaction().Throws(new Exception("Simulate exception"));

            // Act
            var result = await service.Execute();

            // Assert
            Assert.False(result);
            await logService.Received().ErrorMessageAsync(Arg.Any<string>());
        }

        public SumTransactionJobService GetService()
        {

            logService = Substitute.For<ILogService>();
            monitoringServices = Substitute.For<IMonitoringServices>();
            return new SumTransactionJobService(monitoringServices, logService);
        }
    }
}
