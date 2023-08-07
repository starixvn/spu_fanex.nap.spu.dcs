using Fanex.NAP.SPU.DCS.Features.ArchiveData;
using Sentry;
using System;
using System.Threading.Tasks;

namespace Fanex.NAP.SPU.DCS.JobService.Services
{
    public interface IArchiveJobServices
    {
        Task<bool> Execute(int batchSize);

        Task<bool> DeleteBotComponent(int retentionDays);
    }

    public class ArchiveJobServices : IArchiveJobServices
    {
        private readonly IArchiveDataService archiveDataService;
        private readonly ISentryClient _sentryClient;

        public ArchiveJobServices(IArchiveDataService archiveDataService, ISentryClient sentryClient)
        {
            this.archiveDataService = archiveDataService;
            _sentryClient = sentryClient;
        }

        public Task<bool> Execute(int batchSize)
        {
            try
            {
                if (batchSize > 0)
                {
                    archiveDataService.UpdateModifiedDate(batchSize);
                }
                else
                {
                    _sentryClient.CaptureException(new Exception("ArchiveData invalid Batch size"));
                    return Task.FromResult(false);
                }
            }
            catch (Exception ex)
            {
                _sentryClient.CaptureException(ex);
                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        public Task<bool> DeleteBotComponent(int retentionDays)
        {
            try
            {
                if (retentionDays >= 0)
                {
                    archiveDataService.DeleteBotComponent(retentionDays);
                }
                else
                {
                    _sentryClient.CaptureException(new Exception("DeleteBotComponent invalid Retention days"));
                    return Task.FromResult(false);
                }
            }
            catch (Exception ex)
            {
                _sentryClient.CaptureException(ex);
                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }
    }
}
