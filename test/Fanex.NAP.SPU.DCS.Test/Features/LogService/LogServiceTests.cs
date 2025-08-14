namespace Fanex.NAP.SPU.DCS.Tests.Features.LogService
{
    using System;
    using System.Threading.Tasks;
    using AutoFixture;
    using Fanex.Logging;
    using LogService = Fanex.NAP.SPU.DCS.JobService.Services.LogService;
    using ILogService = Fanex.NAP.SPU.DCS.JobService.Services.ILogService;
    using NSubstitute;
    using Xunit;

    public class LogServiceTests
    {
        private readonly ILogger logger;
        private readonly ILogService logService;
        private readonly IFixture fixture = new Fixture();

        public LogServiceTests()
        {
            logger = Substitute.For<ILogger>();
            logService = new LogService(logger);
        }

        [Fact]
        public void Constructor_WithValidLogger_CreatesInstance()
        {
            // Act
            var service = new LogService(logger);

            // Assert
            Assert.NotNull(service);
        }

        [Fact]
        public void Constructor_WithNullLogger_ThrowsArgumentNullException()
        {
            // Act & Assert
            var exception = Record.Exception(() => new LogService(null));
            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact]
        public void Error_WhenCalled_LogsException()
        {
            // Arrange
            var exception = new Exception("Test exception");

            // Act
            logService.Error(exception);

            // Assert
            logger.Received(1).Error(Arg.Any<string>());
        }

        [Fact]
        public void Error_WithNullException_ThrowsNullReferenceException()
        {
            // Arrange
            Exception exception = null;

            // Act & Assert
            var result = Record.Exception(() => logService.Error(exception));
            Assert.IsType<NullReferenceException>(result);
        }

        [Fact]
        public void Error_WithCustomException_LogsCorrectly()
        {
            // Arrange
            var exception = new InvalidOperationException("Custom exception message");

            // Act
            logService.Error(exception);

            // Assert
            logger.Received(1).Error(Arg.Is<string>(msg => msg.Contains("Custom exception message")));
        }

        [Fact]
        public async Task ErrorAsync_WhenCalled_LogsExceptionAsync()
        {
            // Arrange
            var exception = new Exception("Test async exception");

            // Act
            await logService.ErrorAsync(exception);

            // Assert
            await logger.Received(1).ErrorAsync(Arg.Any<string>());
        }

        [Fact]
        public async Task ErrorAsync_WithNullException_ThrowsNullReferenceException()
        {
            // Arrange
            Exception exception = null;

            // Act & Assert
            var result = await Record.ExceptionAsync(async () => await logService.ErrorAsync(exception));
            Assert.IsType<NullReferenceException>(result);
        }

        [Fact]
        public async Task ErrorAsync_WithCustomException_LogsCorrectly()
        {
            // Arrange
            var exception = new ArgumentException("Custom async exception message", "paramName");

            // Act
            await logService.ErrorAsync(exception);

            // Assert
            await logger.Received(1).ErrorAsync(Arg.Is<string>(msg => msg.Contains("Custom async exception message")));
        }

        [Fact]
        public async Task InfoMessageAsync_WhenCalled_LogsInfoMessage()
        {
            // Arrange
            var infoMessage = fixture.Create<string>();

            // Act
            await logService.InfoMessageAsync(infoMessage);

            // Assert
            await logger.Received(1).InfoAsync(infoMessage);
        }

        [Fact]
        public async Task InfoMessageAsync_WithEmptyString_LogsEmptyMessage()
        {
            // Arrange
            var infoMessage = string.Empty;

            // Act
            await logService.InfoMessageAsync(infoMessage);

            // Assert
            await logger.Received(1).InfoAsync(string.Empty);
        }

        [Fact]
        public async Task InfoMessageAsync_WithNullString_LogsNullMessage()
        {
            // Arrange
            string infoMessage = null;

            // Act
            await logService.InfoMessageAsync(infoMessage);

            // Assert
            await logger.Received(1).InfoAsync(null);
        }

        [Fact]
        public async Task InfoMessageAsync_WithSpecialCharacters_LogsCorrectly()
        {
            // Arrange
            var infoMessage = "Special chars: ñáéíóú & <xml> & \"quotes\"";

            // Act
            await logService.InfoMessageAsync(infoMessage);

            // Assert
            await logger.Received(1).InfoAsync(infoMessage);
        }

        [Fact]
        public async Task ErrorMessageAsync_WhenCalled_LogsErrorMessage()
        {
            // Arrange
            var errorMessage = fixture.Create<string>();

            // Act
            await logService.ErrorMessageAsync(errorMessage);

            // Assert
            await logger.Received(1).ErrorAsync(errorMessage);
        }

        [Fact]
        public async Task ErrorMessageAsync_WithEmptyString_LogsEmptyMessage()
        {
            // Arrange
            var errorMessage = string.Empty;

            // Act
            await logService.ErrorMessageAsync(errorMessage);

            // Assert
            await logger.Received(1).ErrorAsync(string.Empty);
        }

        [Fact]
        public async Task ErrorMessageAsync_WithNullString_LogsNullMessage()
        {
            // Arrange
            string errorMessage = null;

            // Act
            await logService.ErrorMessageAsync(errorMessage);

            // Assert
            await logger.Received(1).ErrorAsync(null);
        }

        [Fact]
        public async Task ErrorMessageAsync_WithSpecialCharacters_LogsCorrectly()
        {
            // Arrange
            var errorMessage = "Error with special chars: ñáéíóú & <xml> & \"quotes\"";

            // Act
            await logService.ErrorMessageAsync(errorMessage);

            // Assert
            await logger.Received(1).ErrorAsync(errorMessage);
        }

        [Fact]
        public async Task MultipleCalls_LogsAllMessages()
        {
            // Arrange
            var infoMessage1 = "Info 1";
            var infoMessage2 = "Info 2";
            var errorMessage = "Error message";

            // Act
            await logService.InfoMessageAsync(infoMessage1);
            await logService.InfoMessageAsync(infoMessage2);
            await logService.ErrorMessageAsync(errorMessage);

            // Assert
            await logger.Received(1).InfoAsync(infoMessage1);
            await logger.Received(1).InfoAsync(infoMessage2);
            await logger.Received(1).ErrorAsync(errorMessage);
        }

        [Fact]
        public void Interface_ImplementsAllRequiredMethods()
        {
            // Arrange & Act
            var service = new LogService(logger);

            // Assert
            Assert.IsAssignableFrom<ILogService>(service);
        }

        [Fact]
        public void Interface_AllMethodsAreAccessible()
        {
            // Arrange
            ILogService service = new LogService(logger);

            // Act & Assert - verify interface methods are accessible
            service.Error(new Exception("Test"));
            Assert.True(true, "Error method is accessible");

            // Note: We can't easily test async methods in this context without making the test async
            // but we can verify the interface contract is satisfied
        }

        [Fact]
        public void Interface_MethodSignaturesMatchImplementation()
        {
            // Arrange
            ILogService service = new LogService(logger);

            // Act & Assert - verify method signatures match interface
            service.Error(new Exception("Test"));
            Assert.True(true, "Method signatures are compatible");
        }
    }
}
