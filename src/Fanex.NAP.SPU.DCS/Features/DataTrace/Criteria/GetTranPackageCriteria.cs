
namespace Fanex.NAP.SPU.DCS.Features.DataTrace.Criteria
{
    using System;
    using Fanex.Data.Repository;
    using Fanex.Data;
    public class GetTranPackageCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_FromDate")]
        public DateTime FromDate { get; set; }

        [SpParam(SqlParamName = "ip_ToDate")]
        public DateTime ToDate { get; set; }

        [SpParam(SqlParamName = "ip_BatchSize")]
        public int BatchSize { get; set; }

        [SpParam(SqlParamName = "ip_SubscriberName")]
        public string SubscriberName { get; set; }

        [SpParam(SqlParamName = "ip_AccountName")]
        public string AccountName { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DT_SendTransactionToLogstash_GetPackage";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
