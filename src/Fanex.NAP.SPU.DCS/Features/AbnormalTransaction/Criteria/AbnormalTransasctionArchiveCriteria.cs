using Fanex.Data.Repository;

namespace Fanex.NAP.SPU.DCS.Features.AbnormalTransaction.Criteria
{
    internal class AbnormalTransasctionArchiveCriteria : NonQueryCommand
    {
        public override string GetSettingKey()
        {
            return "DCS_DC_ArchiveData_NewAccountDevice";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
