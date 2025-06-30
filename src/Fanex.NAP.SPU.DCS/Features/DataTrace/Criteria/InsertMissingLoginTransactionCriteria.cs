namespace Fanex.NAP.SPU.DCS.Features.DataTrace.Criteria
{
    using System;
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class InsertMissingLoginTransactionCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_TransDate", AllowNull = true)]
        public DateTime TransDate { get; set; }

        [SpParam(SqlParamName = "ip_FromID", AllowNull = true)]
        public int MinID { get; set; }

        [SpParam(SqlParamName = "ip_ToID", AllowNull = true)]
        public int MaxID { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DT_Summary_MissingLoginTransaction_Insert";
        }

        public override bool IsValid() => MaxID >= MinID;
    }
}
