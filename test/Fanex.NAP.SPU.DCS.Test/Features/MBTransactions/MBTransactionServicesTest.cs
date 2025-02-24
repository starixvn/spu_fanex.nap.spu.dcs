
namespace Fanex.NAP.SPU.DCS.Tests.Features.MBTransactions
{
    using AutoFixture;
    using Fanex.NAP.SPU.DCS.Features.MBTransactions;
    using Fanex.NAP.SPU.DCS.Features.MBTransactions.Entities;
    using NSubstitute;
    using Xunit;

    public class MBTransactionServicesTest
    {
        private readonly IMBTransactionDataAccess transactionDataAccess;
        private readonly IMBTransactionServices transactionServices;
        private readonly IFixture fixture = new Fixture();

        public MBTransactionServicesTest()
        {
            transactionDataAccess = Substitute.For<IMBTransactionDataAccess>();
            transactionServices = new MBTransactionServices(transactionDataAccess);
        }

        [Fact]
        public void GetMBRawTransaction_WhenCalled_ReturnsExpectedData()
        {
            // Arrange
            int numberOfTrans = 10;

            int numberOfBatch = 2;

            var expectedTransactions = fixture.CreateMany<MBRawTransactionEntity>();

            transactionDataAccess.GetMBRawTransaction(numberOfTrans, numberOfBatch).Returns(expectedTransactions);

            // Act
            var actual = transactionServices.GetMBRawTransaction(numberOfTrans, numberOfBatch);

            // Assert
            Assert.Equal(expectedTransactions, actual);
        }

        [Fact]
        public void InsertMBTransaction_WhenCalled_ExecutesWithCorrectCriteria()
        {
            // Arrange
            var fromID = fixture.Create<ulong>();

            var toID = fixture.Create<ulong>();

            // Act
            transactionServices.InsertMBTransaction(fromID, toID);

            // Assert
            transactionDataAccess.Received(1).InsertMBTransaction(fromID, toID);
        }
    }
}

