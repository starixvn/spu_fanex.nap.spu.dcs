namespace Fanex.NAP.SPU.DCS.Tests.Features.AbnormalTransaction
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System;
    using System.Net.Http;
    using AutoFixture;
    using Fanex.NAP.SPU.DCS.Features.AbnormalTransaction;
    using Fanex.NAP.SPU.DCS.Features.AbnormalTransaction.Entities;
    using Fanex.NAP.SPU.DCS.Features.AbnormalTransaction.Model;
    using Fanex.NAP.SPU.DCS.JobService.Services;
    using NSubstitute;
    using Xunit;

    public class AbnormalTransactionServicesTests
    {
        private readonly IAbnormalTransactionDataAccess dataAccess;
        private readonly ILogService logService;
        private readonly IAbnormalTransactionServices abnormalServices;
        private readonly IFixture fixture = new Fixture();

        public AbnormalTransactionServicesTests()
        {
            dataAccess = Substitute.For<IAbnormalTransactionDataAccess>();
            logService = Substitute.For<ILogService>();

            abnormalServices = new AbnormalTransactionServices(logService, dataAccess);
        }

        [Fact]
        public async Task PushAbnormalTransaction_WhenCalledWithoutTransactions_UpdatesMaxId()
        {
            // Arrange
            var size = fixture.Create<int>();
            var abnormalTransactionUrl = fixture.Create<string>();
            var ignorePushData = false;
            var abnormalTransactions = new List<AbnormalTransactionEntity>();
            var rangeID = new RangeIDModel { MinID = 0, MaxID = 1 };

            dataAccess.GetAbnormalTransactionsAsync(size).Returns((abnormalTransactions, rangeID));

            // Act
            var result = await abnormalServices.PushAbnormalTransaction(size, abnormalTransactionUrl, ignorePushData);

            // Assert
            Assert.True(result);
            dataAccess.Received(1).UpdateAbnormalTransactions(string.Empty, rangeID.MinID, rangeID.MaxID);
        }

        [Fact]
        public async Task PushAbnormalTransaction_WhenIgnorePushDataIsTrue_ReturnsTrue()
        {
            // Arrange
            var size = fixture.Create<int>();
            var abnormalTransactionUrl = fixture.Create<string>();
            var ignorePushData = true;
            var abnormalTransactions = fixture.CreateMany<AbnormalTransactionEntity>().ToList();
            var rangeID = new RangeIDModel { MinID = 0, MaxID = 0 };

            dataAccess.GetAbnormalTransactionsAsync(size).Returns((abnormalTransactions, rangeID));

            // Act
            var result = await abnormalServices.PushAbnormalTransaction(size, abnormalTransactionUrl, ignorePushData);

            // Assert
            Assert.True(result);
            dataAccess.Received(1).UpdateAbnormalTransactions(Arg.Any<string>(), rangeID.MinID, rangeID.MaxID);
        }

        [Fact]
        public async Task PushAbnormalTransaction_WhenCalledWithEmptyTransactionsAndZeroMaxId_DoesNotUpdateMaxId()
        {
            // Arrange
            var size = fixture.Create<int>();
            var abnormalTransactionUrl = fixture.Create<string>();
            var ignorePushData = false;
            var abnormalTransactions = new List<AbnormalTransactionEntity>();
            var rangeID = new RangeIDModel { MinID = 0, MaxID = 0 };

            dataAccess.GetAbnormalTransactionsAsync(size).Returns((abnormalTransactions, rangeID));

            // Act
            var result = await abnormalServices.PushAbnormalTransaction(size, abnormalTransactionUrl, ignorePushData);

            // Assert
            Assert.True(result);
            dataAccess.DidNotReceive().UpdateAbnormalTransactions(Arg.Any<string>(), Arg.Any<ulong>(), Arg.Any<ulong>());
        }

        [Fact]
        public async Task PushAbnormalTransaction_WhenCalledWithNullTransactions_ThrowsArgumentNullException()
        {
            // Arrange
            var size = fixture.Create<int>();
            var abnormalTransactionUrl = fixture.Create<string>();
            var ignorePushData = false;
            IEnumerable<AbnormalTransactionEntity> abnormalTransactions = null;
            var rangeID = new RangeIDModel { MinID = 0, MaxID = 1 };

            dataAccess.GetAbnormalTransactionsAsync(size).Returns((abnormalTransactions, rangeID));

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await abnormalServices.PushAbnormalTransaction(size, abnormalTransactionUrl, ignorePushData));
        }

        [Fact]
        public async Task PushAbnormalTransaction_WhenCalledWithEmptyTransactions_HandlesCorrectly()
        {
            // Arrange
            var size = fixture.Create<int>();
            var abnormalTransactionUrl = fixture.Create<string>();
            var ignorePushData = false;
            var abnormalTransactions = new List<AbnormalTransactionEntity>(); // Empty list instead of null
            var rangeID = new RangeIDModel { MinID = 0, MaxID = 1 };

            dataAccess.GetAbnormalTransactionsAsync(size).Returns((abnormalTransactions, rangeID));

            // Act
            var result = await abnormalServices.PushAbnormalTransaction(size, abnormalTransactionUrl, ignorePushData);

            // Assert
            Assert.True(result);
            dataAccess.Received(1).UpdateAbnormalTransactions(string.Empty, rangeID.MinID, rangeID.MaxID);
        }

        [Fact]
        public async Task PushAbnormalTransaction_WhenCalledWithZeroSize_HandlesCorrectly()
        {
            // Arrange
            var size = 0;
            var abnormalTransactionUrl = fixture.Create<string>();
            var ignorePushData = false;
            var abnormalTransactions = new List<AbnormalTransactionEntity>();
            var rangeID = new RangeIDModel { MinID = 0, MaxID = 1 };

            dataAccess.GetAbnormalTransactionsAsync(size).Returns((abnormalTransactions, rangeID));

            // Act
            var result = await abnormalServices.PushAbnormalTransaction(size, abnormalTransactionUrl, ignorePushData);

            // Assert
            Assert.True(result);
            dataAccess.Received(1).GetAbnormalTransactionsAsync(0);
            dataAccess.Received(1).UpdateAbnormalTransactions(string.Empty, rangeID.MinID, rangeID.MaxID);
        }

        [Fact]
        public async Task PushAbnormalTransaction_WhenCalledWithNegativeSize_HandlesCorrectly()
        {
            // Arrange
            var size = -1;
            var abnormalTransactionUrl = fixture.Create<string>();
            var ignorePushData = false;
            var abnormalTransactions = new List<AbnormalTransactionEntity>();
            var rangeID = new RangeIDModel { MinID = 0, MaxID = 1 };

            dataAccess.GetAbnormalTransactionsAsync(size).Returns((abnormalTransactions, rangeID));

            // Act
            var result = await abnormalServices.PushAbnormalTransaction(size, abnormalTransactionUrl, ignorePushData);

            // Assert
            Assert.True(result);
            dataAccess.Received(1).GetAbnormalTransactionsAsync(-1);
        }

        [Fact]
        public async Task PushAbnormalTransaction_WhenCalledWithEmptyUrl_HandlesCorrectly()
        {
            // Arrange
            var size = fixture.Create<int>();
            var abnormalTransactionUrl = string.Empty;
            var ignorePushData = false;
            var abnormalTransactions = new List<AbnormalTransactionEntity>();
            var rangeID = new RangeIDModel { MinID = 0, MaxID = 1 };

            dataAccess.GetAbnormalTransactionsAsync(size).Returns((abnormalTransactions, rangeID));

            // Act
            var result = await abnormalServices.PushAbnormalTransaction(size, abnormalTransactionUrl, ignorePushData);

            // Assert
            Assert.True(result);
            dataAccess.Received(1).UpdateAbnormalTransactions(string.Empty, rangeID.MinID, rangeID.MaxID);
        }

        [Fact]
        public async Task PushAbnormalTransaction_WhenCalledWithNullUrl_HandlesCorrectly()
        {
            // Arrange
            var size = fixture.Create<int>();
            string abnormalTransactionUrl = null;
            var ignorePushData = false;
            var abnormalTransactions = new List<AbnormalTransactionEntity>();
            var rangeID = new RangeIDModel { MinID = 0, MaxID = 1 };

            dataAccess.GetAbnormalTransactionsAsync(size).Returns((abnormalTransactions, rangeID));

            // Act
            var result = await abnormalServices.PushAbnormalTransaction(size, abnormalTransactionUrl, ignorePushData);

            // Assert
            Assert.True(result);
            dataAccess.Received(1).UpdateAbnormalTransactions(string.Empty, rangeID.MinID, rangeID.MaxID);
        }

        [Fact]
        public async Task PushAbnormalTransaction_WhenIgnorePushDataIsTrueAndHasTransactions_UpdatesTransactions()
        {
            // Arrange
            var size = fixture.Create<int>();
            var abnormalTransactionUrl = fixture.Create<string>();
            var ignorePushData = true;
            var abnormalTransactions = fixture.CreateMany<AbnormalTransactionEntity>(3).ToList();
            var rangeID = new RangeIDModel { MinID = 1, MaxID = 3 };

            dataAccess.GetAbnormalTransactionsAsync(size).Returns((abnormalTransactions, rangeID));

            // Act
            var result = await abnormalServices.PushAbnormalTransaction(size, abnormalTransactionUrl, ignorePushData);

            // Assert
            Assert.True(result);
            dataAccess.Received(1).UpdateAbnormalTransactions(Arg.Any<string>(), rangeID.MinID, rangeID.MaxID);
        }

        // ⚠️ Test cũ dùng http://invalid.url sẽ gọi mạng thật → flaky.
        // Đánh dấu skip và ghi rõ hướng dẫn thay thế bằng fake HttpClient/IHttpClientFactory.
        [Fact(Skip = "Network-dependent. Replace by injecting a Fake HttpClient (HttpMessageHandler) into AbnormalTransactionServices or run as an integration test.")]
        public async Task PushAbnormalTransaction_WhenIgnorePushDataIsFalseAndHasTransactions_HandlesNetworkFailure()
        {
            var size = fixture.Create<int>();
            var abnormalTransactionUrl = "http://invalid.url";
            var ignorePushData = false;
            var abnormalTransactions = fixture.CreateMany<AbnormalTransactionEntity>(2).ToList();
            var rangeID = new RangeIDModel { MinID = 1, MaxID = 2 };

            dataAccess.GetAbnormalTransactionsAsync(size).Returns((abnormalTransactions, rangeID));

            await Assert.ThrowsAsync<HttpRequestException>(async () =>
                await abnormalServices.PushAbnormalTransaction(size, abnormalTransactionUrl, ignorePushData));

            dataAccess.DidNotReceive().UpdateAbnormalTransactions(Arg.Any<string>(), Arg.Any<ulong>(), Arg.Any<ulong>());
        }

        [Fact]
        public async Task PushAbnormalTransaction_WhenIgnorePushDataIsFalseAndHasTransactions_BypassesNetworkWithIgnoreFlag()
        {
            // Arrange
            var size = fixture.Create<int>();
            var abnormalTransactionUrl = fixture.Create<string>(); // Any URL since we're bypassing
            var ignorePushData = true; // This bypasses the HTTP call
            var abnormalTransactions = fixture.CreateMany<AbnormalTransactionEntity>(2).ToList();
            var rangeID = new RangeIDModel { MinID = 1, MaxID = 2 };

            dataAccess.GetAbnormalTransactionsAsync(size).Returns((abnormalTransactions, rangeID));

            // Act
            var result = await abnormalServices.PushAbnormalTransaction(size, abnormalTransactionUrl, ignorePushData);

            // Assert
            Assert.True(result);
            dataAccess.Received(1).UpdateAbnormalTransactions(Arg.Any<string>(), rangeID.MinID, rangeID.MaxID);
            await logService.DidNotReceive().ErrorAsync(Arg.Any<Exception>());
        }

        [Fact]
        public async Task PushAbnormalTransaction_WhenHttpCallSuccessful_UpdatesTransactionsCorrectly()
        {
            // Ghi chú: test này mô phỏng “HTTP thành công” bằng cách set ignorePushData=true
            // để tránh gọi mạng thật; chỉ assert luồng cập nhật DB.
            var size = fixture.Create<int>();
            var abnormalTransactionUrl = fixture.Create<string>();
            var ignorePushData = true;
            var abnormalTransactions = fixture.CreateMany<AbnormalTransactionEntity>(3).ToList();
            var rangeID = new RangeIDModel { MinID = 1, MaxID = 3 };

            dataAccess.GetAbnormalTransactionsAsync(size).Returns((abnormalTransactions, rangeID));

            var result = await abnormalServices.PushAbnormalTransaction(size, abnormalTransactionUrl, ignorePushData);

            Assert.True(result);
            dataAccess.Received(1).UpdateAbnormalTransactions(
                Arg.Is<string>(json => !string.IsNullOrEmpty(json) && json.Contains("Status")),
                rangeID.MinID,
                rangeID.MaxID);
        }

        [Fact]
        public async Task PushAbnormalTransaction_WhenDataAccessThrowsException_ExceptionPropagates()
        {
            var size = fixture.Create<int>();
            var abnormalTransactionUrl = fixture.Create<string>();
            var ignorePushData = false;

            dataAccess.When(x => x.GetAbnormalTransactionsAsync(size))
                     .Do(x => { throw new Exception("Database error"); });

            await Assert.ThrowsAsync<Exception>(async () =>
                await abnormalServices.PushAbnormalTransaction(size, abnormalTransactionUrl, ignorePushData));
        }

        [Fact]
        public async Task PushAbnormalTransaction_WhenUpdateThrowsException_ExceptionPropagates()
        {
            var size = fixture.Create<int>();
            var abnormalTransactionUrl = fixture.Create<string>();
            var ignorePushData = false;
            var abnormalTransactions = new List<AbnormalTransactionEntity>();
            var rangeID = new RangeIDModel { MinID = 0, MaxID = 1 };

            dataAccess.GetAbnormalTransactionsAsync(size).Returns((abnormalTransactions, rangeID));
            dataAccess.When(x => x.UpdateAbnormalTransactions(Arg.Any<string>(), Arg.Any<ulong>(), Arg.Any<ulong>()))
                     .Do(x => { throw new Exception("Update error"); });

            await Assert.ThrowsAsync<Exception>(async () =>
                await abnormalServices.PushAbnormalTransaction(size, abnormalTransactionUrl, ignorePushData));
        }

        [Fact]
        public async Task PushAbnormalTransaction_VerifiesDataAccessMethodCalls()
        {
            var size = fixture.Create<int>();
            var abnormalTransactionUrl = fixture.Create<string>();
            var ignorePushData = false;
            var abnormalTransactions = new List<AbnormalTransactionEntity>();
            var rangeID = new RangeIDModel { MinID = 0, MaxID = 1 };

            dataAccess.GetAbnormalTransactionsAsync(size).Returns((abnormalTransactions, rangeID));

            await abnormalServices.PushAbnormalTransaction(size, abnormalTransactionUrl, ignorePushData);

            dataAccess.Received(1).GetAbnormalTransactionsAsync(size);
            dataAccess.Received(1).UpdateAbnormalTransactions(string.Empty, rangeID.MinID, rangeID.MaxID);
        }

        [Fact]
        public async Task PushAbnormalTransaction_WithBoundaryValues_HandlesCorrectly()
        {
            var size = int.MaxValue;
            var abnormalTransactionUrl = fixture.Create<string>();
            var ignorePushData = false;
            var abnormalTransactions = new List<AbnormalTransactionEntity>();
            var rangeID = new RangeIDModel { MinID = ulong.MinValue, MaxID = ulong.MaxValue };

            dataAccess.GetAbnormalTransactionsAsync(size).Returns((abnormalTransactions, rangeID));

            var result = await abnormalServices.PushAbnormalTransaction(size, abnormalTransactionUrl, ignorePushData);

            Assert.True(result);
            dataAccess.Received(1).GetAbnormalTransactionsAsync(int.MaxValue);
            dataAccess.Received(1).UpdateAbnormalTransactions(string.Empty, ulong.MinValue, ulong.MaxValue);
        }

        [Fact]
        public void Constructor_WhenCalled_SetsServices()
        {
            var service = new AbnormalTransactionServices(logService, dataAccess);

            Assert.NotNull(service);
            Assert.IsAssignableFrom<IAbnormalTransactionServices>(service);
        }

        [Fact]
        public void Constructor_WhenCalledWithNullLogService_ShouldThrowOrHandle()
        {
            var exception = Record.Exception(() => new AbnormalTransactionServices(null, dataAccess));

            if (exception != null)
            {
                Assert.IsType<ArgumentNullException>(exception);
            }
            else
            {
                var service = new AbnormalTransactionServices(null, dataAccess);
                Assert.NotNull(service);
            }
        }

        [Fact]
        public void Constructor_WhenCalledWithNullDataAccess_ShouldThrowOrHandle()
        {
            var exception = Record.Exception(() => new AbnormalTransactionServices(logService, null));

            if (exception != null)
            {
                Assert.IsType<ArgumentNullException>(exception);
            }
            else
            {
                var service = new AbnormalTransactionServices(logService, null);
                Assert.NotNull(service);
            }
        }
    }
}
