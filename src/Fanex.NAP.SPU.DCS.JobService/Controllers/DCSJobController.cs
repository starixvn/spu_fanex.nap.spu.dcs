namespace Fanex.NAP.SPU.DCS.JobService.Controllers
{
    using Fanex.NAP.SPU.DCS.Features.SystemSetting;
    using Fanex.NAP.SPU.DCS.JobService.Models;
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
        public async Task<bool> UserAgent([FromBody] TransformRequestModel transformRequest)
        {
            return await userAgentJobService.Execute(transformRequest.BatchSize, false);
        }

        [HttpGet("ConvertUserAgent")]
        public ClientInfo ConvertUserAgent(string userAgent)
        {
            return userAgentJobService.ConvertUserAgent(userAgent);
        }

        [HttpPost("transformtransaction")]
        public async Task<bool> TransformTransaction([FromBody] TransformRequestModel transformRequest)
        {
            return await transactionJobServices.TransactionJobService.Execute(transformRequest.BatchSize, transformRequest.NumberOfBatch, false);
        }

        [HttpPost("transformdevice")]
        public async Task<bool> TransformDevice([FromBody] TransformRequestModel transformRequest)
        {
            return await deviceJobService.Execute(transformRequest.BatchSize, transformRequest.NumberOfBatch, false);
        }

        [HttpPost("deviceevolution")]
        public async Task<bool> DeviceEvolution([FromBody] TransformRequestModel transformRequest)
        {
            return await deviceJobService.DeviceEvolution(transformRequest.BatchSize, false);
        }

        [HttpPost("sumtransaction")]
        public async Task<bool> SumTransaction()
        {
            return await transactionJobServices.SumTransactionJobService.Execute();
        }

        [HttpPost("pushabnormaltransaction")]
        public async Task<bool> PushAbnormalTransaction([FromBody] TransformRequestModel transformRequest)
        {
            return await transactionJobServices.AbnormalTransactionJobService.PushAbnormalTransaction(transformRequest.BatchSize);
        }

        [HttpPost("updatemodifieddate")]
        public async Task<bool> UpdateModifiedDate([FromBody] TransformRequestModel transformRequest)
        {
            return await archiveJobService.Execute(transformRequest.BatchSize, false);
        }

        [HttpPost("updatedcsystemsettingbyid")]
        public bool UpdateDCSystemSettingByID([FromForm] int id, [FromForm] string value)
        {
            return systemSettingServices.UpdateDCSystemSettingByID(id, value);
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
        public async Task<bool> MBTransformTransaction([FromBody] TransformRequestModel transformRequest)
        {
            return await transactionJobServices.MBTransactionJobService.Execute(transformRequest.BatchSize, transformRequest.NumberOfBatch);
        }

        [HttpPost("mbtransformdevice")]
        public async Task<bool> MBTransformDevice([FromBody] TransformRequestModel transformRequest)
        {
            return await mbDeviceJobService.Execute(transformRequest.LimitTotalRecord, transformRequest.NumberOfBatch);
        }

        [HttpPost("mbupdatelastaccesseddate")]
        public async Task<bool> UpdateMBLastAccessedDate([FromBody] TransformRequestModel transformRequest)
        {
            return await archiveJobService.UpdateMBLastAccessedDate(transformRequest.BatchSize);
        }
        #endregion
    }
}
