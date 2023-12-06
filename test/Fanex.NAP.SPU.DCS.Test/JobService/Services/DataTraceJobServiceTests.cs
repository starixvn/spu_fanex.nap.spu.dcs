namespace Fanex.NAP.SPU.DCS.Tests.JobService.Services
{
    using System.Threading.Tasks;
    using Fanex.NAP.SPU.DCS.Features.DataTrace;
    using Fanex.NAP.SPU.DCS.JobService.Services;
    using NSubstitute;
    using Xunit;

    public class DataTraceJobServiceTests
    {
        private IDataTraceServices dataTraceServices;

        [Fact]
        public async Task SummaryLoginTransactionsAsync_SuccessfulExecution_ReturnsTrue()
        {
            // Arrange
            var service = GetService();
            int batchSize = 10;

            dataTraceServices.InsertSummaryLoginTransactionAsync(batchSize).Returns(1);

            // Act
            var result = await service.SummaryLoginTransactionsAsync(batchSize);

            // Assert
            Assert.True(result);
            await dataTraceServices.Received(1).InsertSummaryLoginTransactionAsync(batchSize);
        }
        


        [Fact]
        public async Task UpdateSystemSetting_SuccessfulExecution_ReturnsTrue()
        {
            // Arrange
            var service = GetService();

            dataTraceServices.UpdateSystemSetting(Arg.Any<int>(), Arg.Any<string>()).Returns(true);

            // Act
            var result = await service.UpdateSystemSetting(1, "NewValue");

            // Assert
            Assert.True(result);
        }

        public DataTraceJobService GetService()
        {
            dataTraceServices = Substitute.For<IDataTraceServices>();
            return new DataTraceJobService(dataTraceServices);
        }
    }
}
