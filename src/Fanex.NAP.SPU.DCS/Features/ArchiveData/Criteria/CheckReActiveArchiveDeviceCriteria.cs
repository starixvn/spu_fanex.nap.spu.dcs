using Fanex.Data.Repository;

namespace Fanex.NAP.SPU.DCS.Features.ArchiveData.Criteria
{
    public class CheckReActiveArchiveDeviceCriteria : NonQueryCommand
    {
        public override string GetSettingKey()
        {
            return "DCS_DC_ArchiveData_Device_CheckReActive";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}