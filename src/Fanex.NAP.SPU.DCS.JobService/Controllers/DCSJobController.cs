namespace Fanex.NAP.SPU.DCS.JobService.Controllers
{
    using Fanex.NAP.SPU.DCS.Features.Common;
    using Fanex.NAP.SPU.DCS.Features.SystemSetting;
    using Fanex.NAP.SPU.DCS.JobService.Services;
    using UAParser.FormFactor.Models;

    [ApiController]
    [Route("api/dcsjob")]
    public class DCSJobController : ControllerBase
    {
        private readonly DCSTransactionJobService transactionJobServices;
        private readonly IUserAgentJobService userAgentJobService;
        private readonly IDeviceJobService deviceJobService;
        private readonly IArchiveJobServices archiveJobService;
        private readonly ISystemSettingServices systemSettingServices;
        private readonly IMBDeviceJobService mbDeviceJobService;
        private readonly ICommonJobService commonJobService;


        public DCSJobController(
            DCSTransactionJobService transactionJobServices,
            IUserAgentJobService userAgentJobService,
            IDeviceJobService deviceJobService,
            IArchiveJobServices archiveJobService,
            ISystemSettingServices systemSettingServices,
            IMBDeviceJobService mbDeviceJobService,
            ICommonJobService commonJobService)
        {
            this.transactionJobServices = transactionJobServices;
            this.userAgentJobService = userAgentJobService;
            this.deviceJobService = deviceJobService;
            this.archiveJobService = archiveJobService;
            this.systemSettingServices = systemSettingServices;
            this.mbDeviceJobService = mbDeviceJobService;
            this.commonJobService = commonJobService;
        }

        [HttpPost("useragent")]
        public async Task<bool> UserAgent([FromForm] int size)
        {
            return await userAgentJobService.Execute(size, false);
        }

        [HttpGet("ConvertUserAgent")]
        public ClientInfo ConvertUserAgent(string userAgent)
        {
            return userAgentJobService.ConvertUserAgent(userAgent);
        }

        [HttpPost("transformtransaction")]
        public async Task<bool> TransformTransaction([FromForm] int size, [FromForm] int numberOfBatch)
        {
            return await transactionJobServices.TransactionJobService.Execute(size, numberOfBatch, false);
        }

        [HttpPost("transformdevice")]
        public async Task<bool> TransformDevice([FromForm] int size, [FromForm] int numberOfBatch)
        {
            return await deviceJobService.Execute(size, numberOfBatch, false);
        }

        [HttpPost("deviceevolution")]
        public async Task<bool> DeviceEvolution([FromForm] int batchSize)
        {
            return await deviceJobService.DeviceEvolution(batchSize, false);
        }

        [HttpPost("sumtransaction")]
        public async Task<bool> SumTransaction()
        {
            return await transactionJobServices.SumTransactionJobService.Execute();
        }

        [HttpPost("pushabnormaltransaction")]
        public async Task<bool> PushAbnormalTransaction([FromForm] int size)
        {
            return await transactionJobServices.AbnormalTransactionJobService.PushAbnormalTransaction(size);
        }

        [HttpPost("updatemodifieddate")]
        public async Task<bool> UpdateModifiedDate([FromForm] int batchSize)
        {
            return await archiveJobService.Execute(batchSize, false);
        }

        [HttpPost("updatedcsystemsettingbyid")]
        public bool UpdateDCSystemSettingByID([FromForm] int id, [FromForm] string value)
        {
            return  systemSettingServices.UpdateDCSystemSettingByID(id, value);
        }

        [HttpPost("deletebotcomponent")]
        public async Task<bool> DeleteBotComponent([FromForm] int retentionDays)
        {
            return await archiveJobService.DeleteBotComponent(retentionDays, false);
        }

        [HttpPost("insertidrangefpfingerprint")]
        public bool InsertIDRangeFPFingerPrint([FromForm] DateTime? fromDate, [FromForm] DateTime? toDate)
        {
            fromDate ??= DateTime.Now.AddDays(-1);
            toDate ??= fromDate;

            return commonJobService.InsertIDRangeFPFingerPrint(fromDate.Value, toDate.Value);
        }

        [HttpPost("updateidrangefpfingerprint")]
        public bool UpdateIDRangeFPFingerPrint([FromForm] DateTime date, [FromForm] ulong minID, [FromForm] ulong maxID)
        {
            return commonJobService.UpdateIDRangeFPFingerPrint(date, minID, maxID);
        }

        #region Mobile app
        [HttpPost("mbtransformtransaction")]
        public async Task<bool> MBTransformTransaction([FromForm] int batchSize, [FromForm] int numberOfBatch)
        {
            return await transactionJobServices.MBTransactionJobService.Execute(batchSize, numberOfBatch);
        }

        [HttpPost("mbtransformdevice")]
        public async Task<bool> MBTransformDevice([FromForm] int limitTotalRecord, [FromForm] int numberOfBatch)
        {
            return await mbDeviceJobService.Execute(limitTotalRecord, numberOfBatch);
        }

        [HttpPost("mbupdatelastaccesseddate")]
        public async Task<bool> UpdateMBLastAccessedDate([FromForm] int batchSize)
        {
            return await archiveJobService.UpdateMBLastAccessedDate(batchSize);
        }
        #endregion
    }
}
