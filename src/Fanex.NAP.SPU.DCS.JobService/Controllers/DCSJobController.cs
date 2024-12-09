namespace Fanex.NAP.SPU.DCS.JobService.Controllers
{
    using Fanex.NAP.SPU.DCS.Features.MBTransactions;
    using Fanex.NAP.SPU.DCS.Features.SystemSetting;
    using Fanex.NAP.SPU.DCS.JobService.Services;
    using UAParser.FormFactor.Models;

    [ApiController]
    [Route("api/dcsjob")]
    public class DCSJobController : ControllerBase
    {
        private readonly IUserAgentJobService userAgentJobService;
        private readonly IDeviceJobService deviceJobService;
        private readonly ITransactionJobService transactionJobService;
        private readonly ISumTransactionJobService sumTransactionJobService;
        private readonly IAbnormalTransactionJobService abnormalTransactionJobService;
        private readonly IArchiveJobServices archiveJobServices;
        private readonly ISystemSettingServices systemSettingServices;
        private readonly IMBTransactionJobService mbTransactionJobService;
        private readonly IMBDeviceJobService mbDeviceJobService;

        public DCSJobController(
            IUserAgentJobService userAgentJobService,
            IDeviceJobService deviceJobService,
            ITransactionJobService transactionJobService,
            ISumTransactionJobService sumTransactionJobService,
            IAbnormalTransactionJobService abnormalTransactionJobService,
            IArchiveJobServices archiveJobServices,
            ISystemSettingServices systemSettingServices,
            IMBTransactionJobService mbTransactionJobService,
            IMBDeviceJobService mbDeviceJobService)
        {
            this.userAgentJobService = userAgentJobService;
            this.deviceJobService = deviceJobService;
            this.transactionJobService = transactionJobService;
            this.sumTransactionJobService = sumTransactionJobService;
            this.abnormalTransactionJobService = abnormalTransactionJobService;
            this.archiveJobServices = archiveJobServices;
            this.systemSettingServices = systemSettingServices;
            this.mbTransactionJobService = mbTransactionJobService;
            this.mbDeviceJobService = mbDeviceJobService;
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
            return await transactionJobService.Execute(size, numberOfBatch, false);
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
            return await sumTransactionJobService.Execute();
        }

        [HttpPost("pushabnormaltransaction")]
        public async Task<bool> PushAbnormalTransaction([FromForm] int size)
        {
            return await abnormalTransactionJobService.PushAbnormalTransaction(size);
        }

        [HttpPost("updatemodifieddate")]
        public async Task<bool> UpdateModifiedDate([FromForm] int batchSize)
        {
            return await archiveJobServices.Execute(batchSize, false);
        }

        [HttpPost("updatedcsystemsettingbyid")]
        public async Task<bool> UpdateDCSystemSettingByID([FromForm] int id, [FromForm] string value)
        {
            return await systemSettingServices.UpdateDCSystemSettingByID(id, value);
        }

        [HttpPost("deletebotcomponent")]
        public async Task<bool> DeleteBotComponent([FromForm] int retentionDays)
        {
            return await archiveJobServices.DeleteBotComponent(retentionDays, false);
        }

        #region Mobile app
        [HttpPost("MB/TransformTransaction")]
        public async Task<bool> TransformMBTransaction([FromForm] int size, [FromForm] int numberOfBatch)
        {
            return await mbTransactionJobService.Execute(size, numberOfBatch);
        }

        [HttpPost("MB/TransformDevice")]
        public async Task<bool> TransformMBDevice([FromForm] int size, [FromForm] int numberOfBatch)
        {
            return await mbDeviceJobService.Execute(size, numberOfBatch);
        }

        [HttpPost("MB/UpdateLastAccessedDate")]
        public async Task<bool> UpdateLastAccessedDate([FromForm] int batchSize)
        {
            return await archiveJobServices.UpdateMBLastAccessedDate(batchSize);
        }
        #endregion
    }
}
