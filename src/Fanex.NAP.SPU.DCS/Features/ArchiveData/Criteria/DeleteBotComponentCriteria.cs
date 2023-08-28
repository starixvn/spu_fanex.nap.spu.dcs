using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.NAP.SPU.DCS.Features.ArchiveData.Criteria
{
    public class DeleteBotComponentCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_RetentionDays")]
        public int RetentionDays { get; set; }

        [SpParam(Ignored = true)]
        public bool IsReadyDbExtra { get; set; }

        public override string GetSettingKey()
        {
            if (IsReadyDbExtra)
            {
                return "DCS_ET_ArchiveData_BotComponent_Delete";
            }
            else
            {
                return "DCS_DC_ArchiveData_BotComponent_Delete";
            }
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
