using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.NAP.SPU.DCS.Features.ArchiveData.Criteria
{
    public class DeleteBotComponentCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_RetentionDays", AllowNull = true)]
        public int RetentionDays { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DC_ArchiveData_BotComponent_Delete";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
