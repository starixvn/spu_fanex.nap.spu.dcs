using Fanex.NAP.SPU.DCS.Features.Transactions;
using Fanex.NAP.SPU.DCS.Features.Transactions.Entities;
using FluentAssertions;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Fanex.NAP.SPU.DCS.Tests.Features.Transaction
{
    public class TransactionTests
    {
        private readonly ITransactionDataAccess transactionDataAccess;
        private readonly ITransactionServices transactionServices;

        public TransactionTests()
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
    }
}
