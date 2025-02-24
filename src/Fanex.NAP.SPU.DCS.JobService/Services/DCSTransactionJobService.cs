
namespace Fanex.NAP.SPU.DCS.JobService.Services
{
    using Fanex.NAP.SPU.DCS.Features.SystemSetting;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage(Justification = "Excluded from code coverage")]
    public class DCSTransactionJobService
    {
        public ITransactionJobService TransactionJobService { get; }
        public ISumTransactionJobService SumTransactionJobService { get; }
        public IAbnormalTransactionJobService AbnormalTransactionJobService { get; }
        public IMBTransactionJobService MBTransactionJobService { get; }

        public DCSTransactionJobService(
            ITransactionJobService transactionJobService,
            ISumTransactionJobService sumTransactionJobService,
            IAbnormalTransactionJobService abnormalTransactionJobService,
            IMBTransactionJobService mbTransactionJobService)
        {
            TransactionJobService = transactionJobService;
            SumTransactionJobService = sumTransactionJobService;
            AbnormalTransactionJobService = abnormalTransactionJobService;
            MBTransactionJobService = mbTransactionJobService;
        }
    }
}
