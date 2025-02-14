namespace Fanex.NAP.SPU.DCS.ServiceAPI.Services
{
    using System;
    using System.Threading.Tasks;
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;

    public interface ILogService
    {
        Task ErrorAsync(Exception exception);

        Task InfoMessageAsync(string info);

        Task ErrorMessageAsync(string error);
    }
    public class LogService : ILogService
    {
        private readonly ILogger logger;

        public LogService(ILogger logger)
        {
            this.logger = logger;
        }

        public async Task ErrorAsync(Exception exception)
        {
            var logMessage = new LogMessage(exception.Message)
                       .AddSection("Database Info", DatabaseInfoGetters.GetDbExceptionInfo(exception))
                       .AddSection("Server Info", CommonInfoGetters.GetServerInfo())
                       .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(exception))
                       .ToString();

            await logger.ErrorAsync(logMessage);
        }

        public async Task InfoMessageAsync(string info)
        {
            await logger.InfoAsync(info);
        }

        public async Task ErrorMessageAsync(string error)
        {
            await logger.ErrorAsync(error);
        }
    }
}
