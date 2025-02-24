namespace Fanex.NAP.SPU.DCS.JobService.Controllers
{
    using Fanex.NAP.SPU.DCS.Features.SystemSetting;
    using Fanex.NAP.SPU.DCS.JobService.Services;
    using UAParser.FormFactor.Models;

    [ApiController]
    [Route("api/dcsjob")]
    public class DCSJobController : ControllerBase
    {
        private readonly DCSJobService jobServices;

        public DCSJobController(DCSJobService jobServices)
        {
            this.jobServices = jobServices;
        }

        [HttpPost("useragent")]
        public async Task<bool> UserAgent([FromForm] int size)
        {
            return await jobServices.UserAgentJobService.Execute(size, false);
        }

        [HttpGet("ConvertUserAgent")]
        public ClientInfo ConvertUserAgent(string userAgent)
        {
            return jobServices.UserAgentJobService.ConvertUserAgent(userAgent);
        }

        [HttpPost("transformtransaction")]
        public async Task<bool> TransformTransaction([FromForm] int size, [FromForm] int numberOfBatch)
        {
            return await jobServices.TransactionJobService.Execute(size, numberOfBatch, false);
        }

        [HttpPost("transformdevice")]
        public async Task<bool> TransformDevice([FromForm] int size, [FromForm] int numberOfBatch)
        {
            return await jobServices.DeviceJobService.Execute(size, numberOfBatch, false);
        }

        [HttpPost("deviceevolution")]
        public async Task<bool> DeviceEvolution([FromForm] int batchSize)
        {
            return await jobServices.DeviceJobService.DeviceEvolution(batchSize, false);
        }

        [HttpPost("sumtransaction")]
        public async Task<bool> SumTransaction()
        {
            return await jobServices.SumTransactionJobService.Execute();
        }

        [HttpPost("pushabnormaltransaction")]
        public async Task<bool> PushAbnormalTransaction([FromForm] int size)
        {
            return await jobServices.AbnormalTransactionJobService.PushAbnormalTransaction(size);
        }

        [HttpPost("updatemodifieddate")]
        public async Task<bool> UpdateModifiedDate([FromForm] int batchSize)
        {
            return await jobServices.ArchiveJobServices.Execute(batchSize, false);
        }

        [HttpPost("updatedcsystemsettingbyid")]
        public async Task<bool> UpdateDCSystemSettingByID([FromForm] int id, [FromForm] string value)
        {
            return await jobServices.SystemSettingServices.UpdateDCSystemSettingByID(id, value);
        }

        [HttpPost("deletebotcomponent")]
        public async Task<bool> DeleteBotComponent([FromForm] int retentionDays)
        {
            return await jobServices.ArchiveJobServices.DeleteBotComponent(retentionDays, false);
        }

        #region Mobile app
        [HttpPost("mbtransformtransaction")]
        public async Task<bool> MBTransformTransaction([FromForm] int batchSize, [FromForm] int numberOfBatch)
        {
            return await jobServices.MBTransactionJobService.Execute(batchSize, numberOfBatch);
        }

        [HttpPost("mbtransformdevice")]
        public async Task<bool> MBTransformDevice([FromForm] int limitTotalRecord, [FromForm] int numberOfBatch)
        {
            return await jobServices.MBDeviceJobService.Execute(limitTotalRecord, numberOfBatch);
        }

        [HttpPost("mbupdatelastaccesseddate")]
        public async Task<bool> UpdateMBLastAccessedDate([FromForm] int batchSize)
        {
            return await jobServices.ArchiveJobServices.UpdateMBLastAccessedDate(batchSize);
        }
        #endregion
    }
}
