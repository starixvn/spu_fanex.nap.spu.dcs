
namespace Fanex.NAP.SPU.DCS.JobService.Services
{
    using System.Net.Http;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.SPU.DCS.Features.DataTrace;
    using Fanex.NAP.SPU.DCS.Features.DataTrace.Model;
    using Fanex.NAP.SPU.DCS.JobService;
    using Microsoft.AspNetCore.Http.Extensions;
    using Microsoft.AspNetCore.Mvc.Routing;
    using Microsoft.Extensions.Options;

    public interface IDataTraceJobService
    {
        Task<bool> SummaryBetTransactionAsync(DateTime? fromDate, DateTime? toDate, int batchSize);

        Task<bool> SummaryMissingBetTransactionAsync(DateTime? fromDate, DateTime? toDate, int batchSize);

        Task<bool> SummaryMissingLoginTransactionAsync(DateTime? fromDate, DateTime? toDate, int batchSize);

        Task<bool> UpdateSystemSetting(int SystemSettingID, string SystemSettingValue);

        Task<int> CountDWBetTransactionByDate(DateTime transDate);

        Task<bool> CookDataLoginTransaction(int batchSize);

        Task<bool> SendTransactionToLogstash(string subscriberName, string accountName, DateTime fromDate, DateTime toDate, int batchSize);

        Task<IEnumerable<ReportMissingTransactionModel>> GetReportMissingTransaction(DateTime fromDate, DateTime toDate);

        Task<IEnumerable<ReportLoginTransactionBySubscriberModel>> GetReportLoginTransactionBySubscriber(DateTime fromDate, DateTime toDate);

        Task<IEnumerable<ReportLoginTransactionBySiteModel>> GetReportLoginTransactionBySite(DateTime fromDate, DateTime toDate);

        Task<IEnumerable<ReportLoginTransactionByAccountModel>> GetReportLoginTransactionByAccount(DateTime fromDate, DateTime toDate);

        Task<IEnumerable<ReportLoginTransactionByBrowserModel>> GetReportLoginTransactionByBrowser(DateTime fromDate, DateTime toDate);

        Task<IEnumerable<ReportLoginTransactionByFlaggedModel>> GetReportLoginTransactionByFlagged(DateTime fromDate, DateTime toDate);
    }
    public class DataTraceJobService : IDataTraceJobService
    {
        private readonly HttpClient httpClient;
        private readonly ILogService logService;
        private readonly IAppSettings appSettings;
        private readonly ILogstashService logstashService;
        private readonly ApiEndpoints apiEndpoints;

        public DataTraceJobService(HttpClient httpClient, ILogService logService, IAppSettings appSettings, ILogstashService logstashService, IOptions<ApiEndpoints> apiEndpointsOptions)
        {
            this.httpClient = httpClient;
            this.logService = logService;
            this.appSettings = appSettings;
            this.logstashService = logstashService;
            this.apiEndpoints = apiEndpointsOptions.Value;
        }

        public async Task<bool> SummaryBetTransactionAsync(DateTime? fromDate, DateTime? toDate, int batchSize)
        {
            var queryParams = new Dictionary<string, string>
            {
                { "fromDate", fromDate?.ToString("o") },
                { "toDate", toDate?.ToString("o") },
                { "batchSize", batchSize.ToString() }
            };
            return await CallApiAsync<bool>(ApiHelper.GetFullUrl(appSettings.DataAccessUrl, apiEndpoints.DataTrace.ProcessSummaryBetTransaction), queryParams);
        }

        public async Task<bool> SummaryMissingBetTransactionAsync(DateTime? fromDate, DateTime? toDate, int batchSize)
        {
            var queryParams = new Dictionary<string, string>
            {
                { "fromDate", fromDate?.ToString("o") },
                { "toDate", toDate?.ToString("o") },
                { "batchSize", batchSize.ToString() }
            };
            return await CallApiAsync<bool>(ApiHelper.GetFullUrl(appSettings.DataAccessUrl, apiEndpoints.DataTrace.ProcessSummaryMissingBetTransaction), queryParams);
        }

        public async Task<bool> SummaryMissingLoginTransactionAsync(DateTime? fromDate, DateTime? toDate, int batchSize)
        {
            var queryParams = new Dictionary<string, string>
            {
                { "fromDate", fromDate?.ToString("o") },
                { "toDate", toDate?.ToString("o") },
                { "batchSize", batchSize.ToString() }
            };
            return await CallApiAsync<bool>(ApiHelper.GetFullUrl(appSettings.DataAccessUrl, apiEndpoints.DataTrace.ProcessSummaryMissingLoginTransaction), queryParams);
        }

        public async Task<bool> UpdateSystemSetting(int SystemSettingID, string SystemSettingValue)
        {
            var queryParams = new Dictionary<string, string>
            {
                { "SystemSettingID", SystemSettingID.ToString() },
                { "SystemSettingValue", SystemSettingValue }
            };
            return await CallApiAsync<bool>(ApiHelper.GetFullUrl(appSettings.DataAccessUrl, apiEndpoints.DataTrace.UpdateSystemSetting), queryParams);
        }

        public async Task<int> CountDWBetTransactionByDate(DateTime transDate)
        {
            return await CallApiAsync<int>(ApiHelper.GetFullUrl(appSettings.DataAccessUrl, $"{apiEndpoints.DataTrace.CountDWBetTransactionByDate}?transDate={transDate:o}"));
        }

        public async Task<bool> CookDataLoginTransaction(int batchSize)
        {
            var queryParams = new Dictionary<string, string> { { "batchSize", batchSize.ToString() } };
            return await CallApiAsync<bool>(ApiHelper.GetFullUrl(appSettings.DataAccessUrl, apiEndpoints.DataTrace.CookDataLoginTransaction), queryParams);
        }

        public async Task<bool> SendTransactionToLogstash(string subscriberName, string accountName, DateTime fromDate, DateTime toDate, int batchSize)
        {
            var queryParams = new Dictionary<string, string>
            {
                { "subscriberName", subscriberName },
                { "accountName", accountName },
                { "fromDate", fromDate.ToString("o") },
                { "toDate", toDate.ToString("o") },
                { "batchSize", batchSize.ToString() }
            };
            var transactionLogsTash = await CallApiAsync<IEnumerable<ReportTransactionLogstashModel>>(ApiHelper.GetFullUrl(appSettings.DataAccessUrl, apiEndpoints.DataTrace.GetTransactionLogstash), queryParams);
            await InsertLogstash(transactionLogsTash);
            return true;
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
                    HandleException(ex, "DCS - DataTrace InsertLogstash execute exception message: ");
                }
            }
        }

        #region Report

        public async Task<IEnumerable<ReportMissingTransactionModel>> GetReportMissingTransaction(DateTime fromDate, DateTime toDate)
        {
            return await CallApiAsync<IEnumerable<ReportMissingTransactionModel>>(ApiHelper.GetFullUrl(appSettings.DataAccessUrl, $"{apiEndpoints.DataTrace.GetReportMissingTransaction}?fromDate={fromDate:o}&toDate={toDate:o}"));
        }

        public async Task<IEnumerable<ReportLoginTransactionBySubscriberModel>> GetReportLoginTransactionBySubscriber(DateTime fromDate, DateTime toDate)
        {
            return await CallApiAsync<IEnumerable<ReportLoginTransactionBySubscriberModel>>(ApiHelper.GetFullUrl(appSettings.DataAccessUrl, $"{apiEndpoints.DataTrace.GetReportLoginTransactionBySubscriber}?fromDate={fromDate:o}&toDate={toDate:o}"));
        }

        public async Task<IEnumerable<ReportLoginTransactionBySiteModel>> GetReportLoginTransactionBySite(DateTime fromDate, DateTime toDate)
        {
            return await CallApiAsync<IEnumerable<ReportLoginTransactionBySiteModel>>(ApiHelper.GetFullUrl(appSettings.DataAccessUrl, $"{apiEndpoints.DataTrace.GetReportLoginTransactionBySite}?fromDate={fromDate:o}&toDate={toDate:o}"));
        }

        public async Task<IEnumerable<ReportLoginTransactionByAccountModel>> GetReportLoginTransactionByAccount(DateTime fromDate, DateTime toDate)
        {
            return await CallApiAsync<IEnumerable<ReportLoginTransactionByAccountModel>>(ApiHelper.GetFullUrl(appSettings.DataAccessUrl, $"{apiEndpoints.DataTrace.GetReportLoginTransactionByAccount}?fromDate={fromDate:o}&toDate={toDate:o}"));
        }

        public async Task<IEnumerable<ReportLoginTransactionByBrowserModel>> GetReportLoginTransactionByBrowser(DateTime fromDate, DateTime toDate)
        {
            return await CallApiAsync<IEnumerable<ReportLoginTransactionByBrowserModel>>(ApiHelper.GetFullUrl(appSettings.DataAccessUrl, $"{apiEndpoints.DataTrace.GetReportLoginTransactionByBrowser}?fromDate={fromDate:o}&toDate={toDate:o}"));
        }

        public async Task<IEnumerable<ReportLoginTransactionByFlaggedModel>> GetReportLoginTransactionByFlagged(DateTime fromDate, DateTime toDate)
        {
            return await CallApiAsync<IEnumerable<ReportLoginTransactionByFlaggedModel>>(ApiHelper.GetFullUrl(appSettings.DataAccessUrl, $"{apiEndpoints.DataTrace.GetReportLoginTransactionByFlagged}?fromDate={fromDate:o}&toDate={toDate:o}"));
        }

        #endregion

        private async Task<TResponse> CallApiAsync<TResponse>(string url)
        {
            try
            {
                return await ApiHelper.GetAsync<TResponse>(httpClient, url);
            }
            catch (Exception ex)
            {
                HandleException(ex, $"DCS - DataTrace API call to {url} failed: ");
                return default;
            }
        }

        private async Task<TResponse> CallApiAsync<TResponse>(string url, Dictionary<string, string> queryParams)
        {
            try
            {
                return await ApiHelper.PostAsync<TResponse>(httpClient, url, queryParams);
            }
            catch (Exception ex)
            {
                HandleException(ex, $"DCS - DataTrace API call to {url} failed: ");
                return default;
            }
        }
        private void HandleException(Exception ex, string extraLogMessage)
        {
            string logMessage = new LogMessage(ex.Message)
                .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

            logService.ErrorMessageAsync(extraLogMessage + logMessage);
        }
    }
}
