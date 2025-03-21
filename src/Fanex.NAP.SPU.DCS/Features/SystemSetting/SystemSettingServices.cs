using System.Threading.Tasks;

namespace Fanex.NAP.SPU.DCS.Features.SystemSetting
{
    public interface ISystemSettingServices
    {
        bool UpdateDCSystemSettingByID(int SystemSettingID, string SystemSettingValue);
    }

    public class SystemSettingServices : ISystemSettingServices
    {
        private readonly ISystemSettingDataAccess dataAccess;

        public SystemSettingServices(ISystemSettingDataAccess dataAccess)
        {
            this.dataAccess = dataAccess;
        }

        public bool UpdateDCSystemSettingByID(int SystemSettingID, string SystemSettingValue)
        {
            var result = dataAccess.UpdateDCSystemSettingByID(SystemSettingID, SystemSettingValue);

            return result > 0;
        }
    }
}
