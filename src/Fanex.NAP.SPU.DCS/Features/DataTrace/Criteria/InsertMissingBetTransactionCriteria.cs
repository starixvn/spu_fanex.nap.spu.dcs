namespace Fanex.NAP.SPU.DCS.Features.DataTrace.Criteria
{
    using System;
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class InsertMissingBetTransactionCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_TransDate", AllowNull = true)]
        public DateTime TransDate { get; set; }

        [SpParam(SqlParamName = "ip_ListAccountID", AllowNull = true)]
        public string ListAccountID { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DT_Summary_MissingBetTransaction_Insert";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
