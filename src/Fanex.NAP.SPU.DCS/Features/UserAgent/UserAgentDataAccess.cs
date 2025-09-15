namespace Fanex.NAP.SPU.DCS.Features.UserAgent
{
    using System.Collections.Generic;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.DCS.Features.UserAgent.Criteria;
    using Fanex.NAP.SPU.DCS.Features.UserAgent.Entities;
    using Fanex.NAP.SPU.DCS.Shares;

    public interface IUserAgentDataAccess
    {
        IEnumerable<UserAgentEntity> GetUserAgent(int size);

        DataResult UpdateUserAgent(string userAgentJson);
    }

    public class UserAgentDataAccess : IUserAgentDataAccess
    {
        private readonly IDynamicRepository repository;

        public UserAgentDataAccess(IDynamicRepository repository)
        {
            this.repository = repository ?? new DynamicRepository();
        }

        public IEnumerable<UserAgentEntity> GetUserAgent(int size)
        {
            var criteria = new GetUserAgentCriteria
            {
                Size = size
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<UserAgentEntity>(criteria);
            }
        }

        public DataResult UpdateUserAgent(string userAgentJson)
        {
            var criteria = new UpdateUserAgentCriteria
            {
                UserAgentJson = userAgentJson
            };

            var errCode = repository.Execute(criteria);

            return new DataResult
            {
                ErrorCode = errCode
            };
        }
    }
}
