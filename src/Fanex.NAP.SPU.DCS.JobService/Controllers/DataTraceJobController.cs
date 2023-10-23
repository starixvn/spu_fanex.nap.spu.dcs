namespace Fanex.NAP.SPU.DCS.JobService.Controllers
{
    using System.Threading.Tasks;
    using System;
    using Fanex.NAP.SPU.DCS.JobService.Services;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("api/datatracejob")]
    public class DataTraceJobController : ControllerBase
    {
        private readonly IDataTraceJobService dataTraceJobService;

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
        public async Task<bool> SummaryBetTransaction([FromForm] DateTime? fromDate, [FromForm] DateTime? toDate)
        {
            return await dataTraceJobService.SummaryBetTransactionAsync(fromDate, toDate);
        }

        [HttpPost("summarymissingbettransaction")]
        public async Task<bool> SummaryMissingBetTransaction([FromForm] DateTime? transDate, [FromForm] int batchSize = 25000)
        {
            return await dataTraceJobService.SummaryMissingBetTransactionAsync(transDate, batchSize);
        }

        [HttpPost("summarymissinglogintransaction")]
        public async Task<bool> SummaryMissingLoginTransaction([FromForm] DateTime? transDate, [FromForm] int batchSize = 25000)
        {
            return await dataTraceJobService.SummaryMissingLoginTransactionAsync(transDate, batchSize);
        }

        [HttpPost("deletemissingtransactionbydate")]
        public async Task<bool> DeleteMissingTransactionByDate([FromForm] DateTime transDate, [FromForm] int missingType)
        {
            return await dataTraceJobService.DeleteMissingTransactionByDate(transDate, missingType);
        }
        
        [HttpGet("report-login-transaction-daily")]
        public async Task<IActionResult> ReportLoginTransactionDaily(DateTime fromDate, DateTime toDate)
        {
            var stream = await dataTraceJobService.ReportLoginTransaction(fromDate, toDate);

            if(stream != null)
            {
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.Headers.Add("content-disposition", "attachment; filename=ExcelFile.xlsx");
                var file = File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ReportLoginTransactionDaily.xlsx");

                return file;
            }

            return null;
        }
        
        [HttpGet("report-botd-transaction-daily")]
        public async Task<IActionResult> ReportBotDTransactionDaily(DateTime fromDate, DateTime toDate)
        {
            var stream = await dataTraceJobService.ReportBotDTransaction(fromDate, toDate);

            if(stream != null)
            {
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.Headers.Add("content-disposition", "attachment; filename=ExcelFile.xlsx");
                var file = File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ReportBotDTransactionDaily.xlsx");

                return file;
            }

            return null;
        }
        
        [HttpGet("report-missing-transaction-daily")]
        public async Task<IActionResult> ReportMissingTransactionDaily(DateTime fromDate, DateTime toDate)
        {
            var stream = await dataTraceJobService.ReportMissingTransaction(fromDate, toDate);

            if(stream != null)
            {
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.Headers.Add("content-disposition", "attachment; filename=ExcelFile.xlsx");
                var file = File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ReportMissingTransactionDaily.xlsx");

                return file;
            }

            return null;
        }
        
        [HttpGet("report-login-transaction-detail")]
        public async Task<IActionResult> ReportLoginTransactionDetail(DateTime transDate)
        {
            var stream = await dataTraceJobService.GetReportLoginTransactionDetail(transDate);

            if(stream != null)
            {
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.Headers.Add("content-disposition", "attachment; filename=ExcelFile.xlsx");
                var file = File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ReportLoginTransactionDetail"+ transDate.ToString("yyyymmdd")+ ".xlsx");

                return file;
            }

            return null;
        }
        
        
        [HttpPost("checktimeout")]
        public bool CheckTimeout([FromForm] int delayTime = 10000)
        {
            Task.Delay(delayTime).Wait();
            return true;
        }

    }
}
