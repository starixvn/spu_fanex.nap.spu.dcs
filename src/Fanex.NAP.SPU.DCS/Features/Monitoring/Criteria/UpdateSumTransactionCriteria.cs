namespace Fanex.NAP.SPU.DCS.Features.Monitoring.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;    

    public class UpdateSumTransactionCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_MaxTransID")]
        public long MaxTransID { get; set; }

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
