namespace Fanex.NAP.SPU.DCS.JobService.Services
{
    using Fanex.NAP.SPU.DCS.Features.AbnormalTransaction;

    public interface IAbnormalTransactionJobService
    {
        Task<bool> PushAbnormalTransaction(int size);
    }

    public class AbnormalTransactionJobService : IAbnormalTransactionJobService
    {
        private readonly ISentryClient _sentryClient;
        private readonly IAbnormalTransactionServices _abnormalTransactionServices;

        private readonly string _abnormalTransactionUrl;

        public AbnormalTransactionJobService(ISentryClient sentryClient, IAppSettings appSettings, IAbnormalTransactionServices abnormalTransactionJobServices)
        {
            _sentryClient = sentryClient;
            _abnormalTransactionServices = abnormalTransactionJobServices;

            _abnormalTransactionUrl = appSettings.AbnormalTransactionUrl;
        }

        public async Task<bool> PushAbnormalTransaction(int size)
        {
            if (size > 0)
            {
                try
                {
                    var result = await _abnormalTransactionServices.PushAbnormalTransaction(size, _abnormalTransactionUrl);

                    return result;
                }
                catch (Exception ex)
                {
                    _sentryClient.CaptureException(ex);
                    return false;
                }
            }

            _sentryClient.CaptureMessage("PushAbnormalTransaction size = 0");
            return false;
        }
    }
}
