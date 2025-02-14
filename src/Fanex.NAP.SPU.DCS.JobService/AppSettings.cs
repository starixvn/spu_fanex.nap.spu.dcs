namespace Fanex.NAP.SPU.DCS.JobService
{
    using System.ComponentModel;
    using Microsoft.Extensions.Configuration;

    public interface IAppSettings
    {
        int Environment { get; }

        string EnvironmentString { get; }

        string SPName { get; }

        string SentryDsn { get; }

        string AbnormalTransactionUrl { get; }

        bool AbnormalTransactionIgnorePushData { get; }

        string TelegramProxy { get; }

        string TelegramGroupIDMonitorTransaction { get; }

        bool IsReadyDbExtra { get; }

        string RatioUrl { get; }

        string ELKUrl { get; }

        bool IsEnableLogstash { get; }

        string LogstashTcpHostname { get; }

        int LogstashTcpPort { get; }

        string DataAccessUrl { get; }

        List<string> LogstashIgnoreSubscribers { get; }

        T GetValue<T>(string key);

        T GetSection<T>(string key);
    }

    public class AppSettings : IAppSettings
    {
        private readonly IConfiguration configuration;

        private readonly string environment;
        private readonly string spName;
        private readonly string sentryDsn;
        private readonly string abnormalTransactionUrl;
        private readonly bool abnormalTransactionIgnorePushData;
        private readonly string telegramProxy;
        private readonly string telegramGroupIDMonitorTransaction;
        private readonly bool isReadyDbExtra;
        private readonly string ratioUrl;
        private readonly string elkUrl;
        private readonly bool isEnableLogstash;
        private readonly string logstashTcpHostname;
        private readonly int logstashTcpPort;
        private readonly string dataAccessUrl;
        private readonly List<string> logstashIgnoreSubscribers;

        public AppSettings(IConfiguration configuration)
        {
            this.configuration = configuration;

            environment = GetValue<string>(nameof(Environment));

            spName = GetValue<string>(nameof(SPName));

            sentryDsn = GetValue<string>("Sentry:Dsn");

            abnormalTransactionUrl = GetValue<string>("AbnormalTransaction:Url");

            abnormalTransactionIgnorePushData = GetValue<bool>("AbnormalTransaction:IgnorePushData");

            telegramProxy = GetValue<string>("Telegram:Proxy");

            telegramGroupIDMonitorTransaction = GetValue<string>("Telegram:GroupID:MonitorTransaction");

            isReadyDbExtra = GetValue<bool>("IsReadyDbExtra");

            ratioUrl = GetValue<string>("RatioUrl");

            elkUrl = GetValue<string>("ELK:Url");

            isEnableLogstash = !string.IsNullOrEmpty(GetValue<string>("Logstash:IsEnable")) && Convert.ToBoolean(GetValue<string>("Logstash:IsEnable"));

            logstashTcpHostname = GetValue<string>("Logstash:Tcp:Hostname") ?? string.Empty;

            logstashTcpPort = Convert.ToInt32(string.IsNullOrEmpty(GetValue<string>("Logstash:Tcp:Port")) ? "32705" : GetValue<string>("Logstash:Tcp:Port"));

            logstashIgnoreSubscribers = GetSection<List<string>>("Logstash:IgnoreSubscribers") ?? new List<string>();

            dataAccessUrl = GetValue<string>("DataAccessUrl");
        }

        public int Environment
        {
            get
            {
                switch (environment)
                {
                    case "LOCAL":
                        return 1;
                    case "UAT":
                        return 2;
                    default:
                        return 3;
                }
            }
        }

        public string EnvironmentString
        {
            get
            {
                return environment;
            }
        }

        public string SPName => spName;

        public string SentryDsn => sentryDsn;

        public string AbnormalTransactionUrl => abnormalTransactionUrl;

        public bool AbnormalTransactionIgnorePushData => abnormalTransactionIgnorePushData;

        public string TelegramProxy => telegramProxy;

        public string TelegramGroupIDMonitorTransaction => telegramGroupIDMonitorTransaction;

        public bool IsReadyDbExtra => isReadyDbExtra;

        public string RatioUrl => ratioUrl;       

        public string ELKUrl => elkUrl;

        public bool IsEnableLogstash => isEnableLogstash;

        public string LogstashTcpHostname => logstashTcpHostname;

        public int LogstashTcpPort => logstashTcpPort;

        public string DataAccessUrl => dataAccessUrl;

        public List<string> LogstashIgnoreSubscribers => logstashIgnoreSubscribers;

        public T GetValue<T>(string key)
        {
            try
            {
                var value = configuration[key];

                var typeConverter = TypeDescriptor.GetConverter(typeof(T));

                return (T)typeConverter.ConvertFromString(value);
            }
            catch (Exception ex)
            {
                throw new InvalidCastException($"Key: {key}", ex);
            }
        }

        public T GetSection<T>(string key)
        {
            var schedulerSection = configuration.GetSection(key);

            return schedulerSection.Get<T>();
        }
    }
}
