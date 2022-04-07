namespace Fanex.NAP.SPU.DCS.Features.Monitoring.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class UpdateSumTransactionCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_MaxTransID")]
        public UInt64 MaxTransID { get; set; }

        [SpParam(SqlParamName = "ip_DateScan")]
        public DateTime DateScan { get; set; }

        [SpParam(SqlParamName = "ip_AccountID")]
        public UInt64 AccountID { get; set; }

        [SpParam(SqlParamName = "ip_SumSubscriber")]
        public string SumSubscriberJson { get; set; }

        [SpParam(SqlParamName = "ip_SumDeviceType")]
        public string SumDeviceTypeJson { get; set; }

        [SpParam(SqlParamName = "ip_AccountIP")]
        public string AccountIPJson { get; set; }

        [SpParam(SqlParamName = "ip_AccountDevice")]
        public string AccountDeviceJson { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DC_SumTransaction_Update";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
