namespace Fanex.NAP.SPU.DCS.Features.UserAgent
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.SPU.DCS.Features.UserAgent.Entities;
    using Fanex.NAP.SPU.DCS.Shares;
    using Fanex.NAP.SPU.DCS.Shares.Models;
    using UAParser.FormFactor;
    using UAParser.FormFactor.Models;

    public interface IUserAgentServices
    {
        List<UserAgentEntity> GetUserAgent(int size, bool isReadyDbExtra);

        DataResult UpdateUserAgent(string userAgentJson, bool isReadyDbExtra);

        ClientInfo ConvertUserAgent(string userAgent);

        List<UserAgentModel> TransformUserAgent(List<UserAgentEntity> userAgents);
    }

    public class UserAgentServices : IUserAgentServices
    {
        private readonly IUserAgentDataAccess dataAccess;
        private const string Unknown = "Unknown";

        public UserAgentServices(IUserAgentDataAccess dataAccess)
        {
            this.dataAccess = dataAccess;
        }

        public List<UserAgentEntity> GetUserAgent(int size, bool isReadyDbExtra)
        {
            return dataAccess.GetUserAgent(size, isReadyDbExtra).ToList();
        }

        public DataResult UpdateUserAgent(string userAgentJson, bool isReadyDbExtra)
        {
            if (!string.IsNullOrWhiteSpace(userAgentJson))
            {
                return dataAccess.UpdateUserAgent(userAgentJson, isReadyDbExtra);
            }

            return new DataResult
            {
                ErrorCode = -1,
                ErrorMessage = "Parameter is empty"
            };
        }

        public List<UserAgentModel> TransformUserAgent(List<UserAgentEntity> userAgents)
        {
            if (userAgents != null && userAgents.Any())
            {
                return userAgents.Select(x =>
                {
                    var clientInfo = ConvertUserAgent(x.UserAgent);

                    if (clientInfo == null)
                    {
                        return new UserAgentModel
                        {
                            OSName = Unknown,
                            BrowserName = Unknown,
                            DeviceTypeName = Enum.GetName(typeof(DeviceFormFactor), DeviceFormFactor.Unknown),
                            UserAgentKey = x.UserAgentKey,
                            CreatedDate = x.CreatedDate
                        };
                    }
                    else
                    {
                        return new UserAgentModel
                        {
                            OSName = clientInfo.OS != null ? clientInfo.OS.Family : string.Empty,
                            BrowserName = clientInfo.UA != null ? clientInfo.UA.Family : string.Empty,
                            DeviceTypeName = GetDeviceTypeName(clientInfo),
                            UserAgentKey = x.UserAgentKey,
                            CreatedDate = x.CreatedDate
                        };
                    }

                }).ToList();
            }

            return new List<UserAgentModel>();
        }

        public ClientInfo ConvertUserAgent(string userAgent)
        {
            if (!string.IsNullOrWhiteSpace(userAgent))
            {
                try
                {
                    var uaParser = Parser.GetDefault();

                    return uaParser.Parse(userAgent);
                }
                catch (Exception ex)
                {
                    var logMessage = new LogMessage(ex.Message)
                          .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                          .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                    LogService.Error($"DCS - Parse User Agent {userAgent} exception message:  {logMessage}");

                    return null;
                }
            }

            LogService.Info($"DCS - Parse User Agent null:");

            return null;
        }

        private static string GetDeviceTypeName(ClientInfo clientInfo)
        {
            if (clientInfo != null && clientInfo.Device != null)
            {
                var osName = clientInfo.OS != null ? clientInfo.OS.Family : string.Empty;
                var browserName = clientInfo.UA != null ? clientInfo.UA.Family : string.Empty;
                if (clientInfo.Device.FormFactor == DeviceFormFactor.Unknown && osName == "Mac OS X" && browserName == "Safari")
                {
                    return Enum.GetName(typeof(DeviceFormFactor), DeviceFormFactor.Desktop);
                }

                return Enum.GetName(typeof(DeviceFormFactor), clientInfo.Device.FormFactor);
            }

            return Enum.GetName(typeof(DeviceFormFactor), DeviceFormFactor.Unknown);
        }
    }
}
