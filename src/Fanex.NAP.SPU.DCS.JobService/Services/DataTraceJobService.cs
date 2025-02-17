
namespace Fanex.NAP.SPU.DCS.JobService.Services
{
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.SPU.DCS.Features.DataTrace;
    using Fanex.NAP.SPU.DCS.Features.DataTrace.Model;
    using Fanex.NAP.SPU.DCS.JobService.Models;

    public interface IDataTraceJobService
    {
        Task<bool> SummaryBetTransactionAsync(DateTime? fromDate, DateTime? toDate, int batchSize);

        Task<bool> SummaryMissingBetTransactionAsync(DateTime? fromDate, DateTime? toDate, int batchSize);

        Task<bool> SummaryMissingLoginTransactionAsync(DateTime? fromDate, DateTime? toDate, int batchSize);

        Task<bool> UpdateSystemSetting(int SystemSettingID, string SystemSettingValue);

        Task<int> CountDWBetTransactionByDate(DateTime transDate);

        Task<bool> CookDataLoginTransaction(int batchSize);

        Task<bool> SendTransactionToLogstash(int batchSize);

        Task<IEnumerable<ReportMissingTransactionModel>> GetReportMissingTransaction(DateTime fromDate, DateTime toDate);

        Task<IEnumerable<ReportLoginTransactionBySubscriberModel>> GetReportLoginTransactionBySubscriber(DateTime fromDate, DateTime toDate);

        Task<IEnumerable<ReportLoginTransactionBySiteModel>> GetReportLoginTransactionBySite(DateTime fromDate, DateTime toDate);

        Task<IEnumerable<ReportLoginTransactionByAccountModel>> GetReportLoginTransactionByAccount(DateTime fromDate, DateTime toDate);

        Task<IEnumerable<ReportLoginTransactionByBrowserModel>> GetReportLoginTransactionByBrowser(DateTime fromDate, DateTime toDate);

        Task<IEnumerable<ReportLoginTransactionByFlaggedModel>> GetReportLoginTransactionByFlagged(DateTime fromDate, DateTime toDate);
    }
    public class DataTraceJobService : IDataTraceJobService
    {
        private readonly IDataTraceServices dataTraceServices;
        private readonly ILogService logService;
        private readonly IAppSettings appSettings;
        private readonly ILogstashService logstashService;

        public DataTraceJobService(IDataTraceServices dataTraceServices, ILogService logService, IAppSettings appSettings, ILogstashService logstashService)
        {
            this.dataTraceServices = dataTraceServices;
            this.logService = logService;
            this.appSettings = appSettings;
            this.logstashService = logstashService;
        }

        public async Task<bool> SummaryBetTransactionAsync(DateTime? fromDate, DateTime? toDate, int batchSize)
        {
            try
            {
                return await dataTraceServices.ProcessSummaryBetTransactionAsync(fromDate, toDate, batchSize);
            }
            catch (Exception ex)
            {
                HandleException(ex, "DCS - DataTrace SummaryBetTransactionAsync execute exeption message: ");

                return false;
            }
        }

        public async Task<bool> SummaryMissingBetTransactionAsync(DateTime? fromDate, DateTime? toDate, int batchSize)
        {
            try
            {
                return await dataTraceServices.ProcessSummaryMissingBetTransactionAsync(fromDate, toDate, batchSize);
            }
            catch (Exception ex)
            {
                HandleException(ex, "DCS - DataTrace SummaryMissingBetTransactionAsync execute exeption message: ");

                return false;
            }
        }

        public async Task<bool> SummaryMissingLoginTransactionAsync(DateTime? fromDate, DateTime? toDate, int batchSize)
        {
            try
            {
                return await dataTraceServices.ProcessSummaryMissingLoginTransactionAsync(fromDate, toDate, batchSize);
            }
            catch (Exception ex)
            {
                HandleException(ex, "DCS - DataTrace ProcessSummaryMissingLoginTransactionAsync execute exeption message: ");

                return false;
            }
        }

        public async Task<bool> UpdateSystemSetting(int SystemSettingID, string SystemSettingValue)
        {
            try
            {
                return await dataTraceServices.UpdateSystemSetting(SystemSettingID, SystemSettingValue);
            }
            catch (Exception ex)
            {
                HandleException(ex, "DCS - DataTrace UpdateSystemSetting job execute exeption message: ");
            }

            return false;
        }

        public async Task<int> CountDWBetTransactionByDate(DateTime transDate)
        {
            try
            {
                return await dataTraceServices.CountDWBetTransactionByDate(transDate);
            }
            catch (Exception ex)
            {
                HandleException(ex, "DCS - DataTrace CountDWBetTransactionByDate execute exeption message: ");

                return 0;
            }
        }

        public async Task<bool> CookDataLoginTransaction(int batchSize)
        {
            try
            {
                if (batchSize > 0)
                {
                    await dataTraceServices.CookDataLoginTransaction(batchSize);

                    return true;
                }

                await logService.ErrorMessageAsync("DCS - DataTrace CookDataLoginTransaction has batchSize = 0!");

                return false;
            }
            catch (Exception ex)
            {
                HandleException(ex, "DCS - DataTrace CookDataLoginTransaction execute exeption message: ");

                return false;
            }
        }

        public async Task<bool> SendTransactionToLogstash(int batchSize)
        {
            try
            {
                var transactionLogsTash = await dataTraceServices.GetTransactionLogstash(batchSize);
                await InsertLogstash(transactionLogsTash);
                return true;
            }
            catch (Exception ex)
            {
                HandleException(ex, "DCS - DataTrace SendTransactionLogsToLogstash execute exeption message: ");

                return false;
            }
        }

        private async Task InsertLogstash(IEnumerable<ReportTransactionLogstashModel> transactions)
        {
            if (appSettings.IsEnableLogstash)
            {
                try
                {
                    await logstashService.PushLogAsync(transactions);
                }
                catch (Exception ex)
                {
                    HandleException(ex, "DCS - DataTrace InsertLogstash execute exeption message: ");
                }
            }
        }


        #region Report

        public async Task<IEnumerable<ReportMissingTransactionModel>> GetReportMissingTransaction(DateTime fromDate, DateTime toDate)
        {
            try
            {
                return await dataTraceServices.GetReportMissingTransaction(fromDate, toDate);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<IEnumerable<ReportLoginTransactionBySubscriberModel>> GetReportLoginTransactionBySubscriber(DateTime fromDate, DateTime toDate)
        {
            try
            {
                return await dataTraceServices.GetReportLoginTransactionBySubscriber(fromDate, toDate);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<IEnumerable<ReportLoginTransactionBySiteModel>> GetReportLoginTransactionBySite(DateTime fromDate, DateTime toDate)
        {
            try
            {
                return await dataTraceServices.GetReportLoginTransactionBySite(fromDate, toDate);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<IEnumerable<ReportLoginTransactionByAccountModel>> GetReportLoginTransactionByAccount(DateTime fromDate, DateTime toDate)
        {
            try
            {
                return await dataTraceServices.GetReportLoginTransactionByAccount(fromDate, toDate);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<IEnumerable<ReportLoginTransactionByBrowserModel>> GetReportLoginTransactionByBrowser(DateTime fromDate, DateTime toDate)
        {
            try
            {
                return await dataTraceServices.GetReportLoginTransactionByBrowser(fromDate, toDate);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<IEnumerable<ReportLoginTransactionByFlaggedModel>> GetReportLoginTransactionByFlagged(DateTime fromDate, DateTime toDate)
        {
            try
            {
                return await dataTraceServices.GetReportLoginTransactionByFlagged(fromDate, toDate);
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion

        private void HandleException(Exception ex, string extraLogMessage)
        {
            string logMessage = new LogMessage(ex.Message)
                .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

            logService.ErrorMessageAsync(extraLogMessage + logMessage);
        }
    }
}
