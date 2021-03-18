using Fanex.NAP.SPU.DCS.Features.UserAgent.Entities;
using Fanex.NAP.SPU.DCS.Shares;
using Fanex.NAP.SPU.DCS.Shares.Models;
using System.Collections.Generic;
using System.Linq;
using UAParser;

namespace Fanex.NAP.SPU.DCS.Features.UserAgent
{
    public interface IService
    {
        List<UserAgentEntity> GetUserAgent(int size);

        DataResult UpdateUserAgent(string userAgentJson);

        ClientInfo ConvertUserAgent(string userAgent);

        List<UserAgentModel> TransformUserAgent(List<UserAgentEntity> userAgents);
    }

    public class Services : IService
    {
        private readonly DataAccess dataAccess;

        public Services()
        {
            dataAccess = new DataAccess();
        }

        public List<UserAgentEntity> GetUserAgent(int size)
        {
            return dataAccess.GetUserAgent(size).ToList();
        }

        public DataResult UpdateUserAgent(string userAgentJson)
        {
            return dataAccess.UpdateUserAgent(userAgentJson);
        }

        public List<UserAgentModel> TransformUserAgent(List<UserAgentEntity> userAgents)
        {
            if (userAgents != null && userAgents.Any())
            {
                return userAgents.Select(x =>
                {
                    var clientInfo = ConvertUserAgent(x.UserAgent);                                        

                    return new UserAgentModel
                    {
                        OSName = clientInfo.OS.Family,
                        BrowserName = clientInfo.UA.Family,
                        UserAgentKey = x.UserAgentKey,
                        CreatedDate = x.CreatedDate
                    };

                }).ToList();
            }

            return new List<UserAgentModel>();
        }

        public ClientInfo ConvertUserAgent(string userAgent)
        {
            if (!string.IsNullOrWhiteSpace(userAgent))
            {
                var uaParser = Parser.GetDefault();

                return uaParser.Parse(userAgent);
            }

            return null;
        }
    }
}
