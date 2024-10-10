namespace Fanex.NAP.SPU.DCS.JobService.Services
{
    using Fanex.NAP.SPU.DCS.Features.AbnormalTransaction;

    public interface IAbnormalTransactionJobService
    {
        Task<bool> PushAbnormalTransaction(int size);
    }

    public class AbnormalTransactionJobService : IAbnormalTransactionJobService
    {
        private readonly ILogService _logService;
        private readonly IAbnormalTransactionServices _abnormalTransactionServices;
        private readonly string _abnormalTransactionUrl;
        private readonly bool _ignorePushData;

        public AbnormalTransactionJobService(ILogService logService, IAppSettings appSettings, IAbnormalTransactionServices abnormalTransactionJobServices)
        {
            _logService = logService;

            _abnormalTransactionServices = abnormalTransactionJobServices;

            _abnormalTransactionUrl = appSettings.AbnormalTransactionUrl;

            _ignorePushData = appSettings.AbnormalTransactionIgnorePushData;
        }

        public async Task<bool> PushAbnormalTransaction(int size)
        {
            if (size > 0)
            {
                try
                {
                    var result = await _abnormalTransactionServices.PushAbnormalTransaction(size, _abnormalTransactionUrl, _ignorePushData);

                    return result;
                }
                catch (Exception ex)
                {
                    await _logService.ErrorAsync(ex);
                    return false;
                }
            }

            await _logService.InfoMessageAsync("PushAbnormalTransaction size = 0");
            return false;
        }
    }
}
