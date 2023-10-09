
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
        Task<bool> SummaryLoginTransactionsAsync(int batchSize);

        Task<bool> SummaryBetTicketsAndMissingLoginTranAsync(DateTime? fromDate, DateTime? toDate);

        Task<bool> SummaryMissingLoginTransBetTicketAsync();
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

        public async Task<bool> SummaryLoginTransactionsAsync(int batchSize)
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

        public async Task<bool> SummaryBetTicketsAndMissingLoginTranAsync(DateTime? fromDate, DateTime? toDate)
        {
            try
            {
                return await dataTraceServices.ProcessSummaryBetTicketAndMissingLoginTranAsync(fromDate, toDate);
            }
            catch (Exception ex)
            {
                sentryClient.CaptureException(ex);

                var logMessage = new LogMessage(ex.Message)
                        .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                        .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error($"DCS - DataTrace summary bet tickets and missing login tran job execute exeption message:  {logMessage}");

                return false;
            }
        }
    }
}
