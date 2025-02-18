
namespace Fanex.NAP.SPU.DCS.Tests.Features.AbnormalTransaction
{
    using AutoFixture;
    using Fanex.NAP.SPU.DCS.Features.AbnormalTransaction;
    using Fanex.NAP.SPU.DCS.JobService.Services;
    using NSubstitute;
    public class AbnormalTransactionServicesTest
    {
        private IAbnormalTransactionDataAccess dataAccess;
        private ILogService logService;
        private IAbnormalTransactionServices abnormalServices;
        private IFixture fixture = new Fixture();

        public AbnormalTransactionServicesTest()
        {
            dataAccess = Substitute.For<IAbnormalTransactionDataAccess>();
            logService = Substitute.For<ILogService>();

            abnormalServices = new AbnormalTransactionServices(logService, dataAccess);
        }
    }
}
