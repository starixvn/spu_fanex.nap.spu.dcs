namespace Fanex.NAP.SPU.DCS.Features.Monitoring.Criteria
{
    using System;
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class UpdateSumTransactionCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_MaxTransID", AllowNull = true)]
        public UInt64 MaxTransID { get; set; }

        [SpParam(SqlParamName = "ip_DateScan", AllowNull = true)]
        public DateTime DateScan { get; set; }

        [SpParam(SqlParamName = "ip_AccountID", AllowNull = true)]
        public UInt64 AccountID { get; set; }

        [SpParam(SqlParamName = "ip_SumSubscriber", AllowNull = true)]
        public string SumSubscriberJson { get; set; }

        [SpParam(SqlParamName = "ip_SumDeviceType", AllowNull = true)]
        public string SumDeviceTypeJson { get; set; }

        [SpParam(SqlParamName = "ip_AccountIP", AllowNull = true)]
        public string AccountIPJson { get; set; }

        [SpParam(SqlParamName = "ip_AccountDevice", AllowNull = true)]
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
