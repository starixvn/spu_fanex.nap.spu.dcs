using Fanex.NAP.SPU.DCS.Features.Common;

namespace Fanex.NAP.SPU.DCS.JobService.Services
{
    public interface ICommonJobService
    {
        bool InsertIDRangeFPFingerPrint(DateTime fromDate, DateTime toDate);
    }

    public class CommonJobService : ICommonJobService
    {
        public readonly ICommonServices commonServices;
        public readonly ILogService logService;

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

            var database = "DCS_DataCenter";
            var table = "FP_FingerPrint";
            var dateField = "InsertedTime";
            var idField = "ID";

            for (var date = fromDate; date <= toDate; date = date.AddDays(1))
            {
                var result = commonServices.InsertTableIDRange(database, table, dateField, idField, date);
                if (result < 0)
                {
                    return false;
                }
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
