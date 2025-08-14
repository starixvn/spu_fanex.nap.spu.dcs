using Fanex.NAP.SPU.DCS.Features.Common;

namespace Fanex.NAP.SPU.DCS.JobService.Services
{
    public interface ICommonJobService
    {
        bool InsertIDRangeFPFingerPrint(DateTime fromDate, DateTime toDate);

        bool UpdateIDRangeFPFingerPrint(DateTime date, ulong minID, ulong maxID);
    }

    public class CommonJobService : ICommonJobService 
    {
        private readonly ICommonServices commonServices;
        private readonly ILogService logService;

        private const string DatabaseName = "DCS_DataCenter";
        private const string FingerPrintTable = "FP_FingerPrint";
        private const string DateField = "InsertedTime";
        private const string IdField = "ID";

        public CommonJobService(ICommonServices commonServices, ILogService logService)
        {
            this.commonServices = commonServices;
            this.logService = logService;
        }

        public bool InsertIDRangeFPFingerPrint(DateTime fromDate, DateTime toDate)
        {
            if (!ValidateDate(fromDate, toDate))
            {
                return false;
            }

            for (var date = fromDate; date <= toDate; date = date.AddDays(1))
            {
                var result = commonServices.InsertTableDailyIDRange(DatabaseName, FingerPrintTable, DateField, IdField, date);
                if (result < 0)
                {
                    return false;
                }
            }

            return true;
        }

        public bool UpdateIDRangeFPFingerPrint(DateTime date, ulong minID, ulong maxID)
        {
            var result = commonServices.UpdateTableDailyIDRange(DatabaseName, FingerPrintTable, date, minID, maxID);
            if (result < 0)
            {
                return false;
            }

            return true;
        }

        private bool ValidateDate(DateTime fromDate, DateTime toDate)
        {
            if (fromDate > toDate)
            {
                logService.ErrorMessageAsync("FromDate is greater than ToDate");
                return false;
            }
            return true;
        }
    }
}
