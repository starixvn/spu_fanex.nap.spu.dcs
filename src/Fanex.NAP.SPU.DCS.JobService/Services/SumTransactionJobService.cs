namespace Fanex.NAP.SPU.DCS.JobService.Services
{
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.SPU.DCS.Features.Monitoring;

    public interface ISumTransactionJobService
    {
        Task<bool> Execute();
    }

    public class SumTransactionJobService : ISumTransactionJobService
    {
        private readonly ILogService logService;
        private readonly IMonitoringServices monitoringServices;

        public SumTransactionJobService(IMonitoringServices monitoringServices, ILogService logService)
        {
            this.logService = logService;
            this.monitoringServices = monitoringServices;
        }

        public Task<bool> Execute()
        {
            try
            {
                var sumTransaction = this.monitoringServices.GetSumTransaction();

                this.monitoringServices.UpdateSumRawTransaction(sumTransaction);
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                        .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                        .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                logService.ErrorMessageAsync($"DCS - Sum Transaction job Execute exeption message:  {logMessage}");

                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }
    }
}
