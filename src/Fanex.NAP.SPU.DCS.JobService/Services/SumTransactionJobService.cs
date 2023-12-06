namespace Fanex.NAP.SPU.DCS.JobService.Services
{
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.SPU.DCS.Features.Monitoring;

    public interface ISumTransactionJobService
    {
        Task<bool> Execute();
    }

    public class SumTransactionJobService : ISumTransactionJobService
    {
        private readonly ISentryClient sentryClient;
        private readonly IMonitoringServices monitoringServices;

        public SumTransactionJobService(IMonitoringServices monitoringServices, ISentryClient sentryClient)
        {
            this.sentryClient = sentryClient;
            this.monitoringServices = monitoringServices;
        }

        public Task<bool> Execute()
        {
            try
            {
                var sumTransaction = this.monitoringServices.GetSumTransaction();

                var updateResult = this.monitoringServices.UpdateSumRawTransaction(sumTransaction);
            }
            catch (Exception ex)
            {
                sentryClient.CaptureException(ex);

                var logMessage = new LogMessage(ex.Message)
                        .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                        .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error($"DCS - Sum Transaction job Execute exeption message:  {logMessage}");

                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }
    }
}
