namespace Fanex.NAP.SPU.DCS.Features.UserAgent
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.DCS.Features.UserAgent.Criteria;
    using Fanex.NAP.SPU.DCS.Features.UserAgent.Entities;
    using Fanex.NAP.SPU.DCS.Shares;
    using System.Collections.Generic;

    public interface IUserAgentDataAccess
    {
        IEnumerable<UserAgentEntity> GetUserAgent(int size, bool isReadyDbExtra);

        DataResult UpdateUserAgent(string userAgentJson, bool isReadyDbExtra);
    }

    public class UserAgentDataAccess : IUserAgentDataAccess
    {
        public IEnumerable<UserAgentEntity> GetUserAgent(int size, bool isReadyDbExtra)
        {
            var criteria = new GetUserAgentCriteria
            {
                Size = size,
                IsReadyDbExtra = isReadyDbExtra
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<UserAgentEntity>(criteria);
            }
        }

        public DataResult UpdateUserAgent(string userAgentJson, bool isReadyDbExtra)
        {
            var repository = new DynamicRepository();

            var criteria = new UpdateUserAgentCriteria
            {
                UserAgentJson = userAgentJson,
                IsReadyDbExtra = isReadyDbExtra
            };

            var errCode = repository.Execute(criteria);

            return new DataResult
            {
                ErrorCode = errCode
            };
        }
    }   
}
