using Fanex.Logging.Extensions.Database;
using Fanex.Logging.Extensions;
using Fanex.Logging;

namespace Fanex.NAP.SPU.DCS.ServiceAPI
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (IOException ex)
            {
                var logMessage = new LogMessage(ex.Message)
                   .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex))
                   .ToString();

                await LogService.ErrorAsync(logMessage);
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                   .AddSection("Database Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                   .AddSection("Server Info", CommonInfoGetters.GetServerInfo())
                   .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex))
                   .ToString();

                await LogService.ErrorAsync(logMessage);

            }
        }
    }
}
