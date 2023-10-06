
namespace Fanex.NAP.SPU.DCS.JobService.Services
{
    using System;
    using System.Threading.Tasks;
    using Fanex.Logging.Extensions.Database;
    using Fanex.Logging.Extensions;
    using Fanex.Logging;
    using Sentry;
    using Fanex.NAP.SPU.DCS.Features.DataTrace;

    public interface IDataTraceJobService
    {
        Task<bool> SummaryLoginTransactions(int batchSize);

        Task<bool> SummaryBetTickets(DateTime? fromDate, DateTime? toDate);

        Task<bool> SummaryMissingLoginTransBetTicket();
    }
    public class DataTraceJobService : IDataTraceJobService
    {
        private readonly ISentryClient sentryClient;
        private readonly IDataTraceServices dataTraceServices;

        public DataTraceJobService(ISentryClient sentryClient, IDataTraceServices dataTraceServices)
        {
            this.sentryClient = sentryClient;
            this.dataTraceServices = dataTraceServices;
        }

        public async Task<bool> SummaryLoginTransactions(int batchSize)
        {
            try
            {
                if (batchSize > 0)
                {
                    await dataTraceServices.InsertSummaryLoginTransactionAsync(batchSize);
                    return true;
                }
                else
                {
                    sentryClient.CaptureException(new Exception("BatchSize is invalid!"));
                }
            }
            catch (Exception ex)
            {
                sentryClient.CaptureException(ex);

                var logMessage = new LogMessage(ex.Message)
                        .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                        .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error($"DCS - DataTrace summary transaction job execute exeption message:  {logMessage}");
            }
            return false;
        }

        public async Task<bool> SummaryBetTickets(DateTime? fromDate, DateTime? toDate)
        {
            try
            {
                return await dataTraceServices.ProcessSummaryBetTicketAsync(fromDate, toDate);
            }
            catch (Exception ex)
            {
                sentryClient.CaptureException(ex);

                var logMessage = new LogMessage(ex.Message)
                        .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                        .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error($"DCS - DataTrace summary bet tickets job execute exeption message:  {logMessage}");
                return false;
            }
        }

        public async Task<bool> SummaryMissingLoginTransBetTicket()
        {
            try
            {
                await dataTraceServices.InsertSummaryMissingLoginTransBetTicketAsync();
                return true;
            }
            catch (Exception ex)
            {
                sentryClient.CaptureException(ex);

                var logMessage = new LogMessage(ex.Message)
                        .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                        .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error($"DCS - DataTrace summary missing login tran bet ticket job execute exeption message:  {logMessage}");
                return false;
            }
        }
    }
}
