using Fanex.Data;
using Fanex.Data.Repository;
using System;

namespace Fanex.NAP.SPU.DCS.Features.DataTrace.Criteria
{
    public class DeleteMissingTransactionByDateCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_TransDate")]
        public DateTime TransDate { get; set; }

        [SpParam(SqlParamName = "ip_MissingType")]
        public int MissingType { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DT_Summary_MemberMissingTransaction_DeleteByDate";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
