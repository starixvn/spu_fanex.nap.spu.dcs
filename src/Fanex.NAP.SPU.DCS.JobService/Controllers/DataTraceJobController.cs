namespace Fanex.NAP.SPU.DCS.JobService.Controllers
{
    using Fanex.NAP.SPU.DCS.Features.DataTrace.Model;
    using Fanex.NAP.SPU.DCS.JobService.Services;
    using System.Text;

    [ApiController]
    [Route("api/datatracejob")]
    public class DataTraceJobController : ControllerBase
    {
        private readonly IDataTraceJobService dataTraceJobService;

        public DataTraceJobController(IDataTraceJobService dataTraceJobService)
        {
            this.dataTraceJobService = dataTraceJobService;
        }

        [HttpPost("summarybettransaction")]
        public async Task<bool> SummaryBetTransaction([FromForm] DateTime? fromDate, [FromForm] DateTime? toDate, [FromForm] int batchSize = 25000)
        {
            return await dataTraceJobService.SummaryBetTransactionAsync(fromDate, toDate, batchSize);
        }

        [HttpPost("summarymissingbettransaction")]
        public async Task<bool> SummaryMissingBetTransaction([FromForm] DateTime? fromDate, [FromForm] DateTime? toDate, [FromForm] int batchSize = 25000)
        {
            return await dataTraceJobService.SummaryMissingBetTransactionAsync(fromDate, toDate, batchSize);
        }

        [HttpPost("summarymissinglogintransaction")]
        public async Task<bool> SummaryMissingLoginTransaction([FromForm] DateTime? fromDate, [FromForm] DateTime? toDate, [FromForm] int batchSize = 25000)
        {
            return await dataTraceJobService.SummaryMissingLoginTransactionAsync(fromDate, toDate, batchSize);
        }

        [HttpPost("count-dw-bet-transaction-by-date")]
        public async Task<int> CountDWBetTransactionByDate([FromForm] DateTime transDate)
        {
            return await dataTraceJobService.CountDWBetTransactionByDate(transDate);
        }

        [HttpPost("updatesystemsetting")]
        public async Task<bool> UpdateSystemSetting([FromForm] int id, [FromForm] string value)
        {
            return await dataTraceJobService.UpdateSystemSetting(id, value);
        }

        [HttpPost("cookdatalogintransaction")]
        public async Task<bool> CookDataLoginTransaction([FromForm] int batchSize = 5000)
        {
            return await dataTraceJobService.CookDataLoginTransaction(batchSize);
        }

        [HttpPost("sendtransactiontologstash")]
        public async Task<bool> SendTransactionToLogstash([FromForm] int batchSize = 5000)
        {
            return await dataTraceJobService.SendTransactionToLogstash(batchSize);
        }

        #region Report

        [HttpGet("report-missing-transaction")]
        public async Task<IEnumerable<ReportMissingTransactionModel>> ReportMissingTransaction(DateTime fromDate, DateTime toDate)
        {
            return await dataTraceJobService.GetReportMissingTransaction(fromDate, toDate);
        }

        [HttpGet("report-missing-transaction-csv")]
        public async Task<IActionResult> ReportMissingTransactionCSV(DateTime fromDate, DateTime toDate)
        {
            var data = await dataTraceJobService.GetReportMissingTransaction(fromDate, toDate);

            var builder = new StringBuilder();
            var keyName = "TransDate,SubscriberID,SubscriberName,TotalTrans,CountCustWithoutTrans,CountCustMissingTrans";
            builder.AppendLine(keyName);

            foreach (var item in data)
            {
                builder.AppendLine($"" +
                    $"{item.TransDate.ToString("yyyy-MM-dd")}" +
                    $",{item.SubscriberID}" +
                    $",{item.SubscriberName}" +
                    $",{item.TotalTrans}" +
                    $",{item.CountCustWithoutTrans}" +
                    $",{item.CountCustMissingTrans}");
            }

            return File(Encoding.UTF8.GetBytes(builder.ToString()), "text/csv", "ReportMissingTransaction.csv");
        }

        [HttpGet("report-login-transaction-by-subscriber")]
        public async Task<IEnumerable<ReportLoginTransactionBySubscriberModel>> ReportLoginTransactionBySubscriber(DateTime fromDate, DateTime toDate)
        {
            return await dataTraceJobService.GetReportLoginTransactionBySubscriber(fromDate, toDate);
        }

        [HttpGet("report-login-transaction-by-subscriber-csv")]
        public async Task<IActionResult> ReportLoginTransactionBySubscriberCSV(DateTime fromDate, DateTime toDate)
        {
            var data = await dataTraceJobService.GetReportLoginTransactionBySubscriber(fromDate, toDate);

            var builder = new StringBuilder();
            var keyName = "TransDate,SubscriberID,SubscriberName,TotalTrans,TotalInvalidBrowers,TotalBotTrans,TotalNoDeviceTrans";
            builder.AppendLine(keyName);

            foreach (var item in data)
            {
                builder.AppendLine($"" +
                    $"{item.TransDate.ToString("yyyy-MM-dd")}" +
                    $",{item.SubscriberID}" +
                    $",{item.SubscriberName}" +
                    $",{item.TotalTrans}" +
                    $",{item.TotalInvalidBrowers}" +
                    $",{item.TotalBotTrans}" +
                    $",{item.TotalNoDeviceTrans}");
            }

            return File(Encoding.UTF8.GetBytes(builder.ToString()), "text/csv", "ReportLoginTransactionBySubscriber.csv");
        }

        [HttpGet("report-login-transaction-by-site")]
        public async Task<IEnumerable<ReportLoginTransactionBySiteModel>> ReportLoginTransactionBySite(DateTime fromDate, DateTime toDate)
        {
            return await dataTraceJobService.GetReportLoginTransactionBySite(fromDate, toDate);
        }

        [HttpGet("report-login-transaction-by-site-csv")]
        public async Task<IActionResult> ReportLoginTransactionBySiteCSV(DateTime fromDate, DateTime toDate)
        {
            var data = await dataTraceJobService.GetReportLoginTransactionBySite(fromDate, toDate);

            var builder = new StringBuilder();
            var keyName = "TransDate,SubscriberID,SubscriberName,Site,PaymentType,LicenseeType,TotalTrans,TotalInvalidBrowers,TotalBotTrans,TotalNoDeviceTrans";
            builder.AppendLine(keyName);

            foreach (var item in data)
            {
                builder.AppendLine($"" +
                    $"{item.TransDate.ToString("yyyy-MM-dd")}" +
                    $",{item.SubscriberID}" +
                    $",{item.SubscriberName}" +
                    $",{item.Site}" +
                    $",{item.PaymentType}" +
                    $",{item.LicenseeType}" +
                    $",{item.TotalTrans}" +
                    $",{item.TotalInvalidBrowers}" +
                    $",{item.TotalBotTrans}" +
                    $",{item.TotalNoDeviceTrans}");
            }

            return File(Encoding.UTF8.GetBytes(builder.ToString()), "text/csv", "ReportLoginTransactionBySite.csv");
        }

        [HttpGet("report-login-transaction-by-account")]
        public async Task<IEnumerable<ReportLoginTransactionByAccountModel>> ReportLoginTransactionByAccount(DateTime fromDate, DateTime toDate)
        {
            return await dataTraceJobService.GetReportLoginTransactionByAccount(fromDate, toDate);
        }

        [HttpGet("report-login-transaction-by-account-csv")]
        public async Task<IActionResult> ReportLoginTransactionByAccountCSV(DateTime fromDate, DateTime toDate)
        {
            var data = await dataTraceJobService.GetReportLoginTransactionByAccount(fromDate, toDate);

            var builder = new StringBuilder();
            var keyName = "TransDate,SubscriberID,SubscriberName,AccountID,CustID,LoginName,Site,PaymentType,LicenseeType,TotalTrans,TotalInvalidBrowers,TotalBotTrans,TotalNoDeviceTrans";
            builder.AppendLine(keyName);

            foreach (var item in data)
            {
                builder.AppendLine($"" +
                    $"{item.TransDate.ToString("yyyy-MM-dd")}" +
                    $",{item.SubscriberID}" +
                    $",{item.SubscriberName}" +
                    $",{item.AccountID}" +
                    $",{item.CustID}" +
                    $",{item.LoginName}" +
                    $",{item.Site}" +
                    $",{item.PaymentType}" +
                    $",{item.LicenseeType}" +
                    $",{item.TotalTrans}" +
                    $",{item.TotalInvalidBrowers}" +
                    $",{item.TotalBotTrans}" +
                    $",{item.TotalNoDeviceTrans}");
            }

            return File(Encoding.UTF8.GetBytes(builder.ToString()), "text/csv", "ReportLoginTransactionByAccount.csv");
        }

        [HttpGet("report-login-transaction-by-browser")]
        public async Task<IEnumerable<ReportLoginTransactionByBrowserModel>> ReportLoginTransactionByBrowser(DateTime fromDate, DateTime toDate)
        {
            return await dataTraceJobService.GetReportLoginTransactionByBrowser(fromDate, toDate);
        }

        [HttpGet("report-login-transaction-by-browser-csv")]
        public async Task<IActionResult> ReportLoginTransactionByBrowserCSV(DateTime fromDate, DateTime toDate)
        {
            var data = await dataTraceJobService.GetReportLoginTransactionByBrowser(fromDate, toDate);

            var builder = new StringBuilder();
            var keyName = "TransDate,BrowserID,BrowserName,OSID,OSName,DeviceTypeID,DeviceType,TotalTrans,TotalInvalidBrowers,TotalBotTrans";
            builder.AppendLine(keyName);

            foreach (var item in data)
            {
                builder.AppendLine($"" +
                    $"{item.TransDate.ToString("yyyy-MM-dd")}" +
                    $",{item.BrowserID}" +
                    $",{item.BrowserName}" +
                    $",{item.OSID}" +
                    $",{item.OSName}" +
                    $",{item.DeviceTypeID}" +
                    $",{item.DeviceType}" +
                    $",{item.TotalTrans}" +
                    $",{item.TotalInvalidBrowers}" +
                    $",{item.TotalBotTrans}");
            }

            return File(Encoding.UTF8.GetBytes(builder.ToString()), "text/csv", "ReportLoginTransactionByBrowser.csv");
        }

        [HttpGet("report-login-transaction-by-flagged")]
        public async Task<IEnumerable<ReportLoginTransactionByFlaggedModel>> ReportLoginTransactionByFlagged(DateTime fromDate, DateTime toDate)
        {
            return await dataTraceJobService.GetReportLoginTransactionByFlagged(fromDate, toDate);
        }

        [HttpGet("report-login-transaction-by-flagged-csv")]
        public async Task<IActionResult> ReportLoginTransactionByFlaggedCSV(DateTime fromDate, DateTime toDate)
        {
            var data = await dataTraceJobService.GetReportLoginTransactionByFlagged(fromDate, toDate);

            var builder = new StringBuilder();
            var keyName = "TransDate,Flagged,FlaggedName,TotalTrans";
            builder.AppendLine(keyName);

            foreach (var item in data)
            {
                builder.AppendLine($"" +
                    $"{item.TransDate.ToString("yyyy-MM-dd")}" +
                    $",{item.Flagged}" +
                    $",{item.FlaggedName}" +
                    $",{item.TotalTrans}");
            }

            return File(Encoding.UTF8.GetBytes(builder.ToString()), "text/csv", "ReportLoginTransactionByFlagged.csv");
        }
        #endregion
    }
}
