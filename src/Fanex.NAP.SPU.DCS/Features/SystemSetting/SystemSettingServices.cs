using System.Threading.Tasks;

namespace Fanex.NAP.SPU.DCS.Features.SystemSetting
{
    public interface ISystemSettingServices
    {
        Task<bool> UpdateDCSystemSettingByID(int SystemSettingID, string SystemSettingValue);
    }

    public class SystemSettingServices : ISystemSettingServices
    {
        private readonly ISystemSettingDataAccess dataAccess;

        public SystemSettingServices(ISystemSettingDataAccess dataAccess)
        {
            this.dataAccess = dataAccess;
        }

        public async Task<bool> UpdateDCSystemSettingByID(int SystemSettingID, string SystemSettingValue)
        {
            await dataAccess.UpdateDCSystemSettingByID(SystemSettingID, SystemSettingValue);

            return true;
        }
    }
}
