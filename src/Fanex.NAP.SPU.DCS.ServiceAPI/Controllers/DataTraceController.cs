using Fanex.NAP.SPU.DCS.Features.DataTrace.Model;
using Fanex.NAP.SPU.DCS.ServiceAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace Fanex.NAP.SPU.DCS.ServiceAPI.Controllers
{
    [ApiController]
    [Route("api/datatrace")]
    public class DataTraceController : ControllerBase
    {
        private readonly IDataTraceServices dataTraceServices;

        public DataTraceController(IDataTraceServices dataTraceServices)
        {
            this.dataTraceServices = dataTraceServices;
        }

        [HttpPost("summarybettransaction")]
        public async Task SummaryBetTransaction(DateTime? fromDate, DateTime? toDate, int batchSize = 25000)
        {
            await dataTraceServices.ProcessSummaryBetTransactionAsync(fromDate, toDate, batchSize);
        }

        [HttpPost("summarymissingbettransaction")]
        public async Task SummaryMissingBetTransaction(DateTime? fromDate, DateTime? toDate, int batchSize = 25000)
        {
            await dataTraceServices.ProcessSummaryMissingBetTransactionAsync(fromDate, toDate, batchSize);
        }

        [HttpPost("summarymissinglogintransaction")]
        public async Task SummaryMissingLoginTransaction(DateTime? fromDate, DateTime? toDate, int batchSize = 25000)
        {
            await dataTraceServices.ProcessSummaryMissingLoginTransactionAsync(fromDate, toDate, batchSize);
        }

        [HttpPost("updatesystemsetting")]
        public async Task UpdateSystemSetting(int id, string value)
        {
            await dataTraceServices.UpdateSystemSetting(id, value);
        }

        [HttpPost("count-dw-bet-transaction-by-date")]
        public async Task<int> CountDWBetTransactionByDate(DateTime transDate)
        {
            return await dataTraceServices.CountDWBetTransactionByDate(transDate);
        }

        [HttpPost("cookdatalogintransaction")]
        public async Task<int> CookDataLoginTransaction(int batchSize = 5000)
        {
            return await dataTraceServices.CookDataLoginTransaction(batchSize);
        }

        [HttpPost("sendtransactiontologstash")]
        public async Task<IEnumerable<ReportTransactionLogstashModel>> SendTransactionToLogstash(string subscriberName, string accountName, DateTime fromDate, DateTime toDate, int batchSize = 25000)
        {
            return await dataTraceServices.GetTransactionLogstash(subscriberName, accountName, fromDate, toDate, batchSize);
        }

        #region Report

        [HttpGet("report-missing-transaction")]
        public async Task<IEnumerable<ReportMissingTransactionModel>> GetReportMissingTransaction(DateTime fromDate, DateTime toDate)
        {
            return await dataTraceServices.GetReportMissingTransaction(fromDate, toDate);
        }

        [HttpGet("report-login-transaction-by-subscriber")]
        public async Task<IEnumerable<ReportLoginTransactionBySubscriberModel>> GetReportLoginTransactionBySubscriber(DateTime fromDate, DateTime toDate)
        {
            return await dataTraceServices.GetReportLoginTransactionBySubscriber(fromDate, toDate);
        }

        [HttpGet("report-login-transaction-by-site")]
        public async Task<IEnumerable<ReportLoginTransactionBySiteModel>> GetReportLoginTransactionBySite(DateTime fromDate, DateTime toDate)
        {
            return await dataTraceServices.GetReportLoginTransactionBySite(fromDate, toDate);
        }

        [HttpGet("report-login-transaction-by-account")]
        public async Task<IEnumerable<ReportLoginTransactionByAccountModel>> GetReportLoginTransactionByAccount(DateTime fromDate, DateTime toDate)
        {
            return await dataTraceServices.GetReportLoginTransactionByAccount(fromDate, toDate);
        }

        [HttpGet("report-login-transaction-by-browser")]
        public async Task<IEnumerable<ReportLoginTransactionByBrowserModel>> GetReportLoginTransactionByBrowser(DateTime fromDate, DateTime toDate)
        {
            return await dataTraceServices.GetReportLoginTransactionByBrowser(fromDate, toDate);
        }

        [HttpGet("report-login-transaction-by-flagged")]
        public async Task<IEnumerable<ReportLoginTransactionByFlaggedModel>> GetReportLoginTransactionByFlagged(DateTime fromDate, DateTime toDate)
        {
            return await dataTraceServices.GetReportLoginTransactionByFlagged(fromDate, toDate);
        }

        #endregion
    }
}
