using Microsoft.Extensions.Configuration;

namespace Fanex.NAP.SPU.DCS.JobService
{
    public interface IApiEndpoints
    {
        DataTraceEndpoints DataTrace { get; }
    }
    public class ApiEndpoints : IApiEndpoints
    {
        public DataTraceEndpoints DataTrace { get; set; }
    }

    public class DataTraceEndpoints
    {
        public string ProcessSummaryBetTransaction { get; set; }
        public string ProcessSummaryMissingBetTransaction { get; set; }
        public string ProcessSummaryMissingLoginTransaction { get; set; }
        public string UpdateSystemSetting { get; set; }
        public string CountDWBetTransactionByDate { get; set; }
        public string CookDataLoginTransaction { get; set; }
        public string GetTransactionLogstash { get; set; }
        public string GetReportMissingTransaction { get; set; }
        public string GetReportLoginTransactionBySubscriber { get; set; }
        public string GetReportLoginTransactionBySite { get; set; }
        public string GetReportLoginTransactionByAccount { get; set; }
        public string GetReportLoginTransactionByBrowser { get; set; }
        public string GetReportLoginTransactionByFlagged { get; set; }
    }
}
