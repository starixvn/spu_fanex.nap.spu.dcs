using Microsoft.Extensions.Logging;
using NSubstitute;
using Fanex.NAP.SPU.DCS.JobService.Services;
using Fanex.NAP.SPU.Notification.Services;
using Xunit;
using System;

namespace Fanex.NAP.SPU.DCS.Test.JobService.Services
{
    public class NotificationServiceTests
    {
        private readonly Fanex.NAP.SPU.DCS.JobService.Services.NotificationService _service;

        public NotificationServiceTests()
        {
            _service = new Fanex.NAP.SPU.DCS.JobService.Services.NotificationService();
        }

        #region Send Tests

        [Fact]
        public void Send_WithValidParameters_DoesNotThrowException()
        {
            // Arrange
            var token = "test_token";
            var groupId = "test_group_id";
            var proxy = "test_proxy";
            var message = new MessageModel("Test Title", DateTime.Now, "Test Environment", "Test Message");

            // Act & Assert
            var exception = Record.Exception(() => _service.Send(token, groupId, proxy, message));
            Assert.Null(exception);
        }

        [Fact]
        public void Send_WithNullToken_ThrowsArgumentException()
        {
            // Arrange
            string token = null;
            var groupId = "test_group_id";
            var proxy = "test_proxy";
            var message = new MessageModel("Test Title", DateTime.Now, "Test Environment", "Test Message");

            // Act & Assert
            var exception = Record.Exception(() => _service.Send(token, groupId, proxy, message));
            Assert.NotNull(exception);
            Assert.IsType<ArgumentException>(exception);
            Assert.Contains("Token cannot be null", exception.Message);
        }

        [Fact]
        public void Send_WithNullGroupId_ThrowsArgumentException()
        {
            // Arrange
            var token = "test_token";
            string groupId = null;
            var proxy = "test_proxy";
            var message = new MessageModel("Test Title", DateTime.Now, "Test Environment", "Test Message");

            // Act & Assert
            var exception = Record.Exception(() => _service.Send(token, groupId, proxy, message));
            Assert.NotNull(exception);
            Assert.IsType<ArgumentException>(exception);
            Assert.Contains("Group Id cannot be null", exception.Message);
        }

        [Fact]
        public void Send_WithNullProxy_DoesNotThrowException()
        {
            // Arrange
            var token = "test_token";
            var groupId = "test_group_id";
            string proxy = null;
            var message = new MessageModel("Test Title", DateTime.Now, "Test Environment", "Test Message");

            // Act & Assert
            var exception = Record.Exception(() => _service.Send(token, groupId, proxy, message));
            Assert.Null(exception);
        }

        [Fact]
        public void Send_WithNullMessage_ThrowsArgumentException()
        {
            // Arrange
            var token = "test_token";
            var groupId = "test_group_id";
            var proxy = "test_proxy";
            MessageModel message = null;

            // Act & Assert
            var exception = Record.Exception(() => _service.Send(token, groupId, proxy, message));
            Assert.NotNull(exception);
            Assert.IsType<ArgumentException>(exception);
            Assert.Contains("Parameter cannot be null", exception.Message);
        }

        [Fact]
        public void Send_WithEmptyToken_ThrowsArgumentException()
        {
            // Arrange
            var token = "";
            var groupId = "test_group_id";
            var proxy = "test_proxy";
            var message = new MessageModel("Test Title", DateTime.Now, "Test Environment", "Test Message");

            // Act & Assert
            var exception = Record.Exception(() => _service.Send(token, groupId, proxy, message));
            Assert.NotNull(exception);
            Assert.IsType<ArgumentException>(exception);
            Assert.Contains("Token cannot be null", exception.Message);
        }

        [Fact]
        public void Send_WithEmptyGroupId_ThrowsArgumentException()
        {
            // Arrange
            var token = "test_token";
            var groupId = "";
            var proxy = "test_proxy";
            var message = new MessageModel("Test Title", DateTime.Now, "Test Environment", "Test Message");

            // Act & Assert
            var exception = Record.Exception(() => _service.Send(token, groupId, proxy, message));
            Assert.NotNull(exception);
            Assert.IsType<ArgumentException>(exception);
            Assert.Contains("Group Id cannot be null", exception.Message);
        }

        [Fact]
        public void Send_WithEmptyProxy_DoesNotThrowException()
        {
            // Arrange
            var token = "test_token";
            var groupId = "test_group_id";
            var proxy = "";
            var message = new MessageModel("Test Title", DateTime.Now, "Test Environment", "Test Message");

            // Act & Assert
            var exception = Record.Exception(() => _service.Send(token, groupId, proxy, message));
            Assert.Null(exception);
        }

        #endregion

        #region Edge Cases Tests

        [Fact]
        public void Send_WithLongToken_DoesNotThrowException()
        {
            // Arrange
            var token = new string('A', 1000);
            var groupId = "test_group_id";
            var proxy = "test_proxy";
            var message = new MessageModel("Test Title", DateTime.Now, "Test Environment", "Test Message");

            // Act & Assert
            var exception = Record.Exception(() => _service.Send(token, groupId, proxy, message));
            Assert.Null(exception);
        }

        [Fact]
        public void Send_WithLongGroupId_DoesNotThrowException()
        {
            // Arrange
            var token = "test_token";
            var groupId = new string('B', 1000);
            var proxy = "test_proxy";
            var message = new MessageModel("Test Title", DateTime.Now, "Test Environment", "Test Message");

            // Act & Assert
            var exception = Record.Exception(() => _service.Send(token, groupId, proxy, message));
            Assert.Null(exception);
        }

        [Fact]
        public void Send_WithLongProxy_ThrowsUriFormatException()
        {
            // Arrange
            var token = "test_token";
            var groupId = "test_group_id";
            var proxy = new string('C', 1000);
            var message = new MessageModel("Test Title", DateTime.Now, "Test Environment", "Test Message");

            // Act & Assert
            var exception = Record.Exception(() => _service.Send(token, groupId, proxy, message));
            Assert.NotNull(exception);
            Assert.IsType<UriFormatException>(exception);
            Assert.Contains("Invalid URI", exception.Message);
        }

        [Fact]
        public void Send_WithSpecialCharacters_ThrowsUriFormatException()
        {
            // Arrange
            var token = "test_token_with_special_chars_!@#$%^&*()";
            var groupId = "test_group_id_with_special_chars_!@#$%^&*()";
            var proxy = "test_proxy_with_special_chars_!@#$%^&*()";
            var message = new MessageModel("Test Title", DateTime.Now, "Test Environment", "Test Message");

            // Act & Assert
            var exception = Record.Exception(() => _service.Send(token, groupId, proxy, message));
            Assert.NotNull(exception);
            Assert.IsType<UriFormatException>(exception);
            Assert.Contains("Invalid URI", exception.Message);
        }

        [Fact]
        public void Send_WithUnicodeCharacters_DoesNotThrowException()
        {
            // Arrange
            var token = "test_token_with_unicode_中文日本語한국어";
            var groupId = "test_group_id_with_unicode_中文日本語한국어";
            var proxy = "test_proxy_with_unicode_中文日本語한국어";
            var message = new MessageModel("Test Title", DateTime.Now, "Test Environment", "Test Message");

            // Act & Assert
            var exception = Record.Exception(() => _service.Send(token, groupId, proxy, message));
            Assert.Null(exception);
        }

        #endregion

        #region MessageModel Tests

        [Fact]
        public void Send_WithMessageModelWithNullTitle_DoesNotThrowException()
        {
            // Arrange
            var token = "test_token";
            var groupId = "test_group_id";
            var proxy = "test_proxy";
            var message = new MessageModel(null, DateTime.Now, "Test Environment", "Test Message");

            // Act & Assert
            var exception = Record.Exception(() => _service.Send(token, groupId, proxy, message));
            Assert.Null(exception);
        }

        [Fact]
        public void Send_WithMessageModelWithNullEnvironment_DoesNotThrowException()
        {
            // Arrange
            var token = "test_token";
            var groupId = "test_group_id";
            var proxy = "test_proxy";
            var message = new MessageModel("Test Title", DateTime.Now, null, "Test Message");

            // Act & Assert
            var exception = Record.Exception(() => _service.Send(token, groupId, proxy, message));
            Assert.Null(exception);
        }

        [Fact]
        public void Send_WithMessageModelWithNullMessage_DoesNotThrowException()
        {
            // Arrange
            var token = "test_token";
            var groupId = "test_group_id";
            var proxy = "test_proxy";
            var message = new MessageModel("Test Title", DateTime.Now, "Test Environment", null);

            // Act & Assert
            var exception = Record.Exception(() => _service.Send(token, groupId, proxy, message));
            Assert.Null(exception);
        }

        [Fact]
        public void Send_WithMessageModelWithNullMoreInfo_DoesNotThrowException()
        {
            // Arrange
            var token = "test_token";
            var groupId = "test_group_id";
            var proxy = "test_proxy";
            var message = new MessageModel("Test Title", DateTime.Now, "Test Environment", "Test Message", null);

            // Act & Assert
            var exception = Record.Exception(() => _service.Send(token, groupId, proxy, message));
            Assert.Null(exception);
        }

        [Fact]
        public void Send_WithMessageModelWithMinDateTime_DoesNotThrowException()
        {
            // Arrange
            var token = "test_token";
            var groupId = "test_group_id";
            var proxy = "test_proxy";
            var message = new MessageModel("Test Title", DateTime.MinValue, "Test Environment", "Test Message");

            // Act & Assert
            var exception = Record.Exception(() => _service.Send(token, groupId, proxy, message));
            Assert.Null(exception);
        }

        [Fact]
        public void Send_WithMessageModelWithMaxDateTime_DoesNotThrowException()
        {
            // Arrange
            var token = "test_token";
            var groupId = "test_group_id";
            var proxy = "test_proxy";
            var message = new MessageModel("Test Title", DateTime.MaxValue, "Test Environment", "Test Message");

            // Act & Assert
            var exception = Record.Exception(() => _service.Send(token, groupId, proxy, message));
            Assert.Null(exception);
        }

        #endregion

        #region Integration Tests

        [Fact]
        public void Send_WithAllValidParameters_DoesNotThrowException()
        {
            // Arrange
            var token = "valid_token_123";
            var groupId = "valid_group_id_456";
            var proxy = "valid_proxy_789";
            var message = new MessageModel("Integration Test Title", DateTime.Now, "Production", "This is an integration test message", "Additional info");

            // Act & Assert
            var exception = Record.Exception(() => _service.Send(token, groupId, proxy, message));
            Assert.Null(exception);
        }

        [Fact]
        public void Send_WithAllNullParameters_ThrowsArgumentException()
        {
            // Arrange
            string token = null;
            string groupId = null;
            string proxy = null;
            MessageModel message = null;

            // Act & Assert
            var exception = Record.Exception(() => _service.Send(token, groupId, proxy, message));
            Assert.NotNull(exception);
            Assert.IsType<ArgumentException>(exception);
            Assert.Contains("Token cannot be null", exception.Message);
        }

        [Fact]
        public void Send_WithAllEmptyParameters_ThrowsArgumentException()
        {
            // Arrange
            var token = "";
            var groupId = "";
            var proxy = "";
            var message = new MessageModel("", DateTime.Now, "", "");

            // Act & Assert
            var exception = Record.Exception(() => _service.Send(token, groupId, proxy, message));
            Assert.NotNull(exception);
            Assert.IsType<ArgumentException>(exception);
            Assert.Contains("Token cannot be null", exception.Message);
        }

        #endregion
    }
}
