
namespace Fanex.NAP.SPU.DCS.Tests.Features.MBTransactions
{
    using AutoFixture;
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.DCS.Features.MBTransactions.Criteria;
    using Fanex.NAP.SPU.DCS.Features.MBTransactions;
    using NSubstitute;
    using Xunit;

    public class MBTransactionDataAccessTests
    {
        private readonly IDynamicRepository repository;
        private readonly IMBTransactionDataAccess mbTransactionDataAccess;
        private readonly IFixture fixture = new Fixture();

        public MBTransactionDataAccessTests()
        {
            repository = Substitute.For<IDynamicRepository>();
            mbTransactionDataAccess = new MBTransactionDataAccess(repository);
        }

        [Fact]
        public void InsertMBTransaction_WhenCalled_ExecutesWithCorrectCriteria()
        {
            // Arrange
            var fromID = fixture.Create<ulong>();

            var toID = fixture.Create<ulong>();

            var criteria = fixture.Create<InsertMBTransactionCriteria>();

            // Act
            mbTransactionDataAccess.InsertMBTransaction(fromID, toID);

            // Assert
            repository.Received(1).Execute(Arg.Is<InsertMBTransactionCriteria>(c =>
                c.FromTransID == fromID &&
                c.ToTransID == toID &&
                c.GetSettingKey() == "DCS_DC_Transform_MBTransaction_Insert"));
        }
    }
}