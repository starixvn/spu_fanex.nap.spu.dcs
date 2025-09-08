namespace Fanex.NAP.SPU.DCS.Features.Transactions.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class ProcessRawFingerprintQueueCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_BatchSize", AllowNull = true)]
        public long BatchSize { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DC_Transform_RawFingerprintQueue_Process";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
