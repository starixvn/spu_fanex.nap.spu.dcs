namespace Fanex.NAP.SPU.DCS.JobService.Controllers
{
    using Fanex.NAP.SPU.DCS.JobService.Services;

    [ApiController]
    [Route("api/datatracejob")]
    public class DataTraceJobController : ControllerBase
    {
        private readonly IDataTraceJobService dataTraceJobService;

        private readonly string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        private readonly string keyHeader = "content-disposition";
        private readonly string keyValue = "attachment; filename=ExcelFile.xlsx";

        public DataTraceJobController(IDataTraceJobService dataTraceJobService)
        {
            this.dataTraceJobService = dataTraceJobService;
        }

        [HttpPost("summarylogintransaction")]
        public async Task<bool> SummaryLoginTransaction([FromForm] int batchSize = 50000)
        {
            return await dataTraceJobService.SummaryLoginTransactionsAsync(batchSize);
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

        [HttpPost("init-summary-bet-transaction")]
        public async Task<bool> InitSummaryBetTransaction([FromForm] DateTime transDate)
        {
            return await dataTraceJobService.InitBetTransactionAsync(transDate);
        }

        [HttpPost("count-init-dw-bet-transaction-by-date")]
        public async Task<int> CountInitDWBetTransactionByDate([FromForm] DateTime transDate)
        {
            return await dataTraceJobService.CountInitDWBetTransactionByDate(transDate);
        }

        [HttpPost("checktimeout")]
        public async Task<bool> CheckTimeoutAsync([FromForm] int delayTime = 10000)
        {
            await Task.Delay(delayTime);
            return true;
        }

        [HttpPost("updatesystemsetting")]
        public async Task<bool> UpdateSystemSetting([FromForm] int id, [FromForm] string value)
        {
            return await dataTraceJobService.UpdateSystemSetting(id, value);
        }

        [HttpPost("cook-data-login-transaction")]
        public async Task<bool> CookDataLoginTransaction([FromForm] int batchSize = 5000)
        {
            var result = await dataTraceJobService.CookDataLoginTransaction(batchSize);
            return result > 0;
        }

        #region Report
        [HttpGet("report-login-transaction-daily")]
        public async Task<IActionResult> ReportLoginTransactionDaily(DateTime fromDate, DateTime toDate)
        {
            var stream = await dataTraceJobService.ReportLoginTransaction(fromDate, toDate);

            if (stream != null)
            {
                Response.ContentType = contentType;
                Response.Headers.Add(keyHeader, keyValue);
                var file = File(stream, contentType, "ReportLoginTransactionDaily.xlsx");

                return file;
            }

            return null;
        }

        [HttpGet("report-botd-transaction-daily")]
        public async Task<IActionResult> ReportBotDTransactionDaily(DateTime fromDate, DateTime toDate)
        {
            var stream = await dataTraceJobService.ReportBotDTransaction(fromDate, toDate);

            if (stream != null)
            {
                Response.ContentType = contentType;
                Response.Headers.Add(keyHeader, keyValue);
                var file = File(stream, contentType, "ReportBotDTransactionDaily.xlsx");

                return file;
            }

            return null;
        }

        [HttpGet("report-missing-transaction-daily")]
        public async Task<IActionResult> ReportMissingTransactionDaily(DateTime fromDate, DateTime toDate)
        {
            var stream = await dataTraceJobService.ReportMissingTransaction(fromDate, toDate);

            if (stream != null)
            {
                Response.ContentType = contentType;
                Response.Headers.Add(keyHeader, keyValue);
                var file = File(stream, contentType, "ReportMissingTransactionDaily.xlsx");

                return file;
            }

            return null;
        }

        [HttpGet("report-login-transaction-detail")]
        public async Task<IActionResult> ReportLoginTransactionDetail(DateTime transDate)
        {
            var stream = await dataTraceJobService.ReportLoginTransactionDetail(transDate);

            if (stream != null)
            {
                Response.ContentType = contentType;
                Response.Headers.Add(keyHeader, keyValue);
                var file = File(stream, contentType, "ReportLoginTransactionDetail" + transDate.ToString("yyyymmdd") + ".xlsx");

                return file;
            }

            return null;
        }
        #endregion

    }
}
