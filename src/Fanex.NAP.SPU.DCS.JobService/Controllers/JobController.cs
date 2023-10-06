namespace Fanex.NAP.SPU.DCS.JobService.Controllers
{
    using Fanex.NAP.SPU.DCS.JobService.Services;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Threading.Tasks;
    using UAParser.FormFactor.Models;

    [ApiController]
    [Route("api/job")]
    public class JobController : ControllerBase
    {
        private readonly IUserAgentJobService userAgentJobService;
        private readonly IDeviceJobService deviceJobService;
        private readonly ITransactionJobService transactionJobService;
        private readonly ISumTransactionJobService sumTransactionJobService;
        private readonly INotifyTransactionJobService notifyTransactionJobService;
        private readonly IAbnormalTransactionJobService abnormalTransactionJobService;
        private readonly IDCSExtraService dcsExtraService;
        private readonly IArchiveJobServices archiveJobServices;
        private readonly IDataTraceJobService dataTraceJobService;

        public JobController(IUserAgentJobService userAgentJobService,
            IDeviceJobService deviceJobService,
            ITransactionJobService transactionJobService,
            ISumTransactionJobService sumTransactionJobService,
            INotifyTransactionJobService notifyTransactionJobService,
            IAbnormalTransactionJobService abnormalTransactionJobService,
            IDCSExtraService dcsExtraService,
            IArchiveJobServices archiveJobServices,
            IDataTraceJobService dataTraceJobService)
        {
            this.userAgentJobService = userAgentJobService;
            this.deviceJobService = deviceJobService;
            this.transactionJobService = transactionJobService;
            this.sumTransactionJobService = sumTransactionJobService;
            this.notifyTransactionJobService = notifyTransactionJobService;
            this.abnormalTransactionJobService = abnormalTransactionJobService;
            this.dcsExtraService = dcsExtraService;
            this.archiveJobServices = archiveJobServices;
            this.dataTraceJobService = dataTraceJobService;
        }

        [HttpPost("useragent")]
        public async Task<bool> UserAgent([FromForm] int size)
        {
            return await this.userAgentJobService.Execute(size, false);
        }

        [HttpGet("ConvertUserAgent")]
        public ClientInfo ConvertUserAgent(string userAgent)
        {
            return this.userAgentJobService.ConvertUserAgent(userAgent);
        }

        [HttpPost("transformdevice")]
        public async Task<bool> TransformDevice([FromForm] int size, [FromForm] int numberOfBatch)
        {
            return await this.deviceJobService.Execute(size, numberOfBatch, false);
        }

        [HttpPost("transformtransaction")]
        public async Task<bool> TransformTransaction([FromForm] int size, [FromForm] int numberOfBatch)
        {
            return await this.transactionJobService.Execute(size, numberOfBatch, false);
        }

        [HttpPost("sumtransaction")]
        public async Task<bool> SumTransaction()
        {
            return await this.sumTransactionJobService.Execute();
        }

        [HttpPost("notifytransaction")]
        public async Task<bool> NotifyTransaction()
        {
            return await this.notifyTransactionJobService.Execute();
        }

        [HttpPost("testtelegramnotifymessage")]
        public async Task<bool> TestTelegramNotifyMessage()
        {
            return await this.notifyTransactionJobService.TestTelegramNotifyMessage();
        }

        [HttpPost("pushabnormaltransaction")]
        public async Task<bool> PushAbnormalTransaction([FromForm] int size)
        {
            return await abnormalTransactionJobService.PushAbnormalTransaction(size);
        }

        [HttpPost("monitortransaction")]
        public async Task<bool> MonitorTransaction([FromForm] string rptDateStr, [FromForm] int batchSize, [FromForm] int limitShowCust)
        {
            return await notifyTransactionJobService.MonitorTransactionAndNotify(rptDateStr, batchSize, limitShowCust);
        }

        [HttpPost("updatemodifieddate")]
        public async Task<bool> UpdateModifiedDate([FromForm] int batchSize)
        {
            return await archiveJobServices.Execute(batchSize, false);
        }

        [HttpPost("deletebotcomponent")]
        public async Task<bool> DeleteBotComponent([FromForm] int retentionDays)
        {
            return await archiveJobServices.DeleteBotComponent(retentionDays, false);
        }

        [HttpPost("notifymessagebyid")]
        public async Task<bool> NotifyMessageByID([FromForm] uint notifyMessageID)
        {
            return await notifyTransactionJobService.NotifyMessageByID(notifyMessageID);
        }

        [HttpPost("notifymessagebynotifytype")]
        public async Task<bool> NotifyMessageByNotifyType([FromForm] string notifyType, [FromForm] int batchSize)
        {
            return await notifyTransactionJobService.NotifyMessageByNotifyType(notifyType, batchSize);
        }

        [HttpPost("testSentry")]
        public async Task<bool> TestSentry([FromForm] string message)
        {
            return await notifyTransactionJobService.TestSentryCaptureMessage(message);
        }


        [HttpPost("useragentextra")]
        public async Task<bool> UserAgentExtra([FromForm] int size)
        {
            return await this.userAgentJobService.Execute(size, true);
        }

        [HttpPost("transformtransactionextra")]
        public async Task<bool> TransformTransactionExtra([FromForm] int size, [FromForm] int numberOfBatch)
        {
            return await this.transactionJobService.Execute(size, numberOfBatch, true);
        }

        [HttpPost("transformdeviceextra")]
        public async Task<bool> TransformDeviceExtra([FromForm] int size, [FromForm] int numberOfBatch)
        {
            return await this.deviceJobService.Execute(size, numberOfBatch, true);
        }

        [HttpPost("updatemodifieddateextra")]
        public async Task<bool> UpdateModifiedDateExtra([FromForm] int batchSize)
        {
            return await archiveJobServices.Execute(batchSize, true);
        }

        [HttpPost("deletebotcomponentextra")]
        public async Task<bool> DeleteBotComponentExtra([FromForm] int retentionDays)
        {
            return await archiveJobServices.DeleteBotComponent(retentionDays, true);
        }

        [HttpPost("migratedcsextra")]
        public async Task<bool> MigrateDCSExtra([FromForm] int rangeMinutes, [FromForm] int stopMinutes, [FromForm] int batchSizeGetData)
        {
            return await dcsExtraService.MigrateDCSExtra(rangeMinutes, stopMinutes, batchSizeGetData);
        }

        [HttpPost("initialtransipinfoextra")]
        public async Task<bool> InitialTransIpInfo([FromForm] int batchSize, [FromForm] int numberOfBatch)
        {
            return await dcsExtraService.InitialTransIpInfo(batchSize, numberOfBatch);
        }

        [HttpPost("dtsummarylogintransaction")]
        public async Task<bool> DTSummaryLoginTransaction([FromForm] int batchSize)
        {
           return await dataTraceJobService.SummaryLoginTransactions(batchSize);
        }

        [HttpPost("dtsummarybettickets")]
        public async Task<bool> DTSummaryBetTickets([FromForm] DateTime? fromDate, [FromForm] DateTime? toDate)
        {
            return await dataTraceJobService.SummaryBetTickets(fromDate, toDate);
        }

        [HttpPost("dtsummarymissinglogintranbetticket")]
        public async Task<bool> DTSummaryMissingLoginTransBetTicket()
        {
            return await dataTraceJobService.SummaryMissingLoginTransBetTicket();
        }
    }
}
