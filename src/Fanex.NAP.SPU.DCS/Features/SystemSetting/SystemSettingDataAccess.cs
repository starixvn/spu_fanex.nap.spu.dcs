using Fanex.Data;
using Fanex.Data.Repository;
using Fanex.NAP.SPU.DCS.Features.SystemSetting.Criteria;
using System.Threading.Tasks;

namespace Fanex.NAP.SPU.DCS.Features.SystemSetting
{
    public interface ISystemSettingDataAccess
    {
        Task<string> GetDCSystemSettingByID(int parameterID);

        Task<int> UpdateDCSystemSettingByID(int id, string value);
    }
    public class SystemSettingDataAccess : ISystemSettingDataAccess
    {
        private readonly IDynamicRepository repository;

        public SystemSettingDataAccess(IDynamicRepository repository)
        {
            this.repository = repository ?? new DynamicRepository();
        }

        public async Task<string> GetDCSystemSettingByID(int parameterID)
        {
            var criteria = new GetDCSystemSettingByIDCriteria
            {
                ParameterID = parameterID
            };

            using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
            return await db.QueryEntityAsync<string>(criteria);
        }

        public async Task<int> UpdateDCSystemSettingByID(int id, string value)
        {
            var criteria = new UpdateDCSystemSettingParameterCriteria
            {
                ParameterValue = value,
                ParameterID = id
            };

            return await repository.ExecuteAsync(criteria);
        }
    }
}
