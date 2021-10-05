namespace Fanex.NAP.SPU.DCS.Features.Monitoring.Criteria
{
    using Fanex.Data.Repository;    

    public class GetSumTransactionCriteria : NonQueryCommand
    {
        public override string GetSettingKey()
        {
            return "DCS_DC_SumTransaction_Get";
        }

        public override bool IsValid()
        {
            return true;
        }

    }
}
