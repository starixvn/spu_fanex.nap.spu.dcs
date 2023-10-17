
namespace Fanex.NAP.SPU.DCS.JobService.Services
{
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.SPU.DCS.Features.DataTrace;
    using Sentry;
    using System;
    using System.Threading.Tasks;

    public interface IDataTraceJobService
    {
        Task<bool> SummaryLoginTransactionsAsync(int batchSize);

        Task<bool> SummaryBetTransactionAsync(DateTime? fromDate, DateTime? toDate);

        Task<bool> SummaryMissingBetTransactionAsync(int batchSize);

        Task<bool> SummaryMissingLoginTransactionAsync(int batchSize);

        Task<bool> InitBetTransactionAsync(DateTime fromDate, DateTime toDate);

        Task<bool> UpdateSystemSetting(int SystemSettingID, string SystemSettingValue);

        Task<bool> DeleteMissingTransactionByDate(DateTime transDate, int missingType);
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
                await dataTraceServices.InsertSummaryLoginTransactionAsync(batchSize);

                return true;
            }
            catch (Exception ex)
            {
                sentryClient.CaptureException(ex);

                var logMessage = new LogMessage(ex.Message)
                        .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                        .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error($"DCS - DataTrace summary login transaction job execute exeption message:  {logMessage}");

                return false;
            }
        }

        public async Task<bool> SummaryBetTransactionAsync(DateTime? fromDate, DateTime? toDate)
        {
            return await dataTraceServices.ProcessSummaryBetTransactionAsync(fromDate, toDate);
        }

        public async Task<bool> SummaryMissingBetTransactionAsync(int batchSize)
        {
            try
            {
                return await dataTraceServices.ProcessSummaryMissingBetTransactionAsync(batchSize);
            }
            catch (Exception ex)
            {
                sentryClient.CaptureException(ex);

                var logMessage = new LogMessage(ex.Message)
                        .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                        .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error($"DCS - DataTrace summary missing bet ticket job execute exeption message:  {logMessage}");

                return false;
            }
        }

        public async Task<bool> SummaryMissingLoginTransactionAsync(int batchSize)
        {
            try
            {
                return await dataTraceServices.ProcessSummaryMissingLoginTransactionAsync(batchSize);
            }
            catch (Exception ex)
            {
                sentryClient.CaptureException(ex);

                var logMessage = new LogMessage(ex.Message)
                        .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                        .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error($"DCS - DataTrace summary missing login transaction job execute exeption message:  {logMessage}");

                return false;
            }
        }

        public async Task<bool> InitBetTransactionAsync(DateTime fromDate, DateTime toDate)
        {
            return await dataTraceServices.ProcessInitBetTransactionAsync(fromDate, toDate);
        }

        public async Task<bool> UpdateSystemSetting(int SystemSettingID, string SystemSettingValue)
        {
            return await dataTraceServices.UpdateSystemSetting(SystemSettingID, SystemSettingValue);
        }

        public async Task<bool> DeleteMissingTransactionByDate(DateTime transDate, int missingType)
        {
            return await dataTraceServices.DeleteMissingTransactionByDate(transDate, missingType);
        }
    }
}
