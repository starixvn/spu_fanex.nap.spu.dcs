using Fanex.NAP.SPU.DCS.Features.AbnormalTransaction;
using Sentry;
using System;
using System.Threading.Tasks;

namespace Fanex.NAP.SPU.DCS.JobService.Services
{
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

        public Task<bool> PushAbnormalTransaction(int size)
        {
            if (size > 0)
            {
                try
                {
                    _abnormalTransactionServices.PushAbnormalTransaction(size, _abnormalTransactionUrl);

                    return Task.FromResult(true);
                }
                catch (Exception ex)
                {
                    _sentryClient.CaptureException(ex);                    
                }                
            }

            return Task.FromResult(false);
        }
    }
}
