namespace Fanex.NAP.SPU.DCS.ServiceAPI
{
    using Fanex.Data.Repository;
    using Fanex.Data;
    using Fanex.Logging.Sentry;
    using Fanex.Logging;
    using Fanex.NAP.SPU.DCS.ServiceAPI.Services;
    using Microsoft.AspNetCore.Diagnostics.HealthChecks;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;
    using Sentry;
    using HealthChecks.UI.Client;
    using Fanex.Data.MySql;
    using Fanex.Data.SqlServer;

    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Configuration
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            var appSettings = new AppSettings(builder.Configuration);
            builder.Services.AddSingleton<IAppSettings>(appSettings);
            builder.Services.AddSingleton<IDynamicRepository>(new DynamicRepository());

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "Fanex.NAP.SPU.DCS.ServiceAPI",
                    Version = "v1"
                });
            });
            builder.Services.AddHealthChecks();
            builder.Services.RegisterService();

            FanexDataConfig(builder.Configuration, appSettings);
            RegisterLogging(appSettings);

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Fanex.NAP.SPU.DCS.ServiceAPI v1"));

            app.UseRouting();
            app.UseHttpsRedirection();
            app.UseAuthorization();

            app.UseHealthChecks("/healthz", new HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            app.MapControllers();

            SentrySdk.CaptureMessage($"{typeof(Program).FullName} has been started at {DateTimeOffset.Now}");

            app.Run("http://*:5001");
        }

        private static void FanexDataConfig(IConfiguration configuration, IAppSettings appSettings)
        {
            var connections = configuration
                .GetSection("Connections")
                .GetChildren()
                .ToDictionary(
                    x => x.GetValue<string>("Name"),
                    y => new ConnectionConfiguration(
                        y.GetValue<string>("Name"),
                        y.GetValue<string>("ConnectionString"),
                        y.GetValue<string>("ProviderName"))
                );
            DbSettingProviderManager
                  .StartNewSession()
                  .Use(connections)
                  .WithMySql()
                  .WithSqlServer()
                  .UseDefaultDbSettingProvider(resourcePath: appSettings.SPName)
                  .Run();

        }

        private static void RegisterLogging(IAppSettings appSettings)
        {
            LogManager
                .SetDefaultLogCategory("")
                .Use(new List<ILogEngine>
                {
                new SentryLogging(new SentryEngineOptions
                {
                    Dsn = new Dsn(appSettings.SentryDsn),
                    Environment = "DEV"
                })
                });
        }
    }
}
