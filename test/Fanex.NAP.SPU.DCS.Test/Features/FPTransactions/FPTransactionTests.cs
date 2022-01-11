namespace Fanex.NAP.SPU.DCS.Tests.Features.FPTransactions
{
    using Fanex.NAP.SPU.DCS.Features.FPTransactions;
    using Fanex.NAP.SPU.DCS.Features.FPTransactions.Entities;
    using FluentAssertions;
    using NSubstitute;    
    using System.Collections.Generic;

    public class FPTransactionTests
    {
        private readonly IFPTransactionDataAccess fPTransactionDataAccess;
        private readonly IFPTransactionServices fPTransactionServices;

        public FPTransactionTests()
        {
            fPTransactionDataAccess = Substitute.For<IFPTransactionDataAccess>();

            fPTransactionServices = new FPTransactionServices(fPTransactionDataAccess);
        }

        public void GetFPRawTransaction_ReturnListRawTrans(int isProcessed, int numberOfTrans, int numberOfBatch)
        {
            var result = new List<FPRawTransactionEntity> {
                new FPRawTransactionEntity
                {
                    MinTransId = 1,
                    MaxTransId = 2
                },
                new FPRawTransactionEntity
                {
                    MinTransId = 3,
                    MaxTransId = 4
                },
                new FPRawTransactionEntity
                {
                    MinTransId = 5,
                    MaxTransId = 6
                },
                new FPRawTransactionEntity
                {
                    MinTransId = 7,
                    MaxTransId = 8
                }
            };

            fPTransactionDataAccess.GetFPRawTransaction(isProcessed, numberOfTrans, numberOfBatch).Returns(result);

            var expectedResult = fPTransactionServices.GetFPRawTransaction(isProcessed, numberOfTrans, numberOfBatch);

            expectedResult.Should().BeEquivalentTo(result);
        }

    }
}
