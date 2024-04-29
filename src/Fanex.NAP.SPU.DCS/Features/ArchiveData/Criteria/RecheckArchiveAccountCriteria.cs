using Fanex.Data.Repository;

namespace Fanex.NAP.SPU.DCS.Features.ArchiveData.Criteria
{
    public class RecheckArchiveAccountCriteria : NonQueryCommand
    {
        public override string GetSettingKey()
        {
            return "DCS_DC_ArchiveData_Account_Recheck";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}