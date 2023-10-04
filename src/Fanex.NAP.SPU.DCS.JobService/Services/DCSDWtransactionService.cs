
namespace Fanex.NAP.SPU.DCS.JobService.Services
{
    using System;
    using System.Threading.Tasks;
    using Fanex.Logging.Extensions.Database;
    using Fanex.Logging.Extensions;
    using Fanex.Logging;
    using Sentry;

    public interface IDCSDWtransactionService
    {
        Task<bool> SummaryTransactionsByRangeDate(DateTime? fromDate, DateTime? toDate);
        Task<bool> SummaryBetTicketsByRangeDate(DateTime? fromDate, DateTime? toDate);
    }
    public class DCSDWtransactionService: IDCSDWtransactionService
    {
        private readonly ISentryClient sentryClient;

        public Task<bool> SummaryTransactionsByRangeDate(DateTime? fromDate, DateTime? toDate)
        {
            try
            {
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                sentryClient.CaptureException(ex);

                var logMessage = new LogMessage(ex.Message)
                        .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                        .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error($"DCS - DW Summary Transaction job Execute exeption message:  {logMessage}");

                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        public Task<bool> SummaryBetTicketsByRangeDate(DateTime? fromDate, DateTime? toDate)
        {
            try
            {
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                sentryClient.CaptureException(ex);

                var logMessage = new LogMessage(ex.Message)
                        .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                        .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error($"DCS - DW Summary bet tickets job Execute exeption message:  {logMessage}");

                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }
    }
}
