namespace Fanex.NAP.SPU.DCS.JobService
{
    using Fanex.Data;
    using Fanex.Data.MySql;
    using Fanex.Data.Repository;
    using Fanex.Data.SqlServer;
    using Fanex.Logging;
    using Fanex.Logging.Sentry;
    using Fanex.NAP.SPU.DCS.JobService.Services;
    using HealthChecks.UI.Client;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Diagnostics.HealthChecks;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Options;
    using Microsoft.OpenApi.Models;
    using System.Net.Http;
    using System.Threading;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var appSettings = new AppSettings(this.Configuration);

            services.AddSingleton<IAppSettings>(appSettings);
            services.AddSingleton<IDynamicRepository>(new DynamicRepository());
            services.Configure<ApiEndpoints>(Configuration.GetSection("ApiEndpoints"));
            services.AddSingleton<IApiEndpoints>(sp => sp.GetRequiredService<IOptions<ApiEndpoints>>().Value);

            services.AddControllers();
            services.AddHealthChecks();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Fanex.NAP.SPU.DCS.JobService", Version = "v1" });
            });

            services.RegisterService();

            FanexDataConfig(appSettings);

            RegisterLogging(appSettings);

            services.AddHttpClient<DeviceJobService>().ConfigurePrimaryHttpMessageHandler(() =>
            {
                return new SocketsHttpHandler
                {
                    PooledConnectionLifetime = TimeSpan.FromMinutes(15)
                };
            })
            .SetHandlerLifetime(Timeout.InfiniteTimeSpan);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Fanex.NAP.SPU.DCS.JobService v1"));

            app.UseRouting();

            app.UseHealthChecks("/healthz", new HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void FanexDataConfig(IAppSettings appSettings)
        {
            var connections = Configuration
                                  .GetSection("Connections")
                                  .GetChildren()
                                  .AsEnumerable()
                                  .ToDictionary(
                                           x => x.GetValue<string>("Name"),
                                           y => new ConnectionConfiguration(y.GetValue<string>("Name"), y.GetValue<string>("ConnectionString"), y.GetValue<string>("ProviderName"))
                                   );

            DbSettingProviderManager
              .StartNewSession()
              .Use(connections)
              .WithMySql()
              .WithSqlServer()
              .UseDefaultDbSettingProvider(resourcePath: appSettings.SPName)
              .Run();
        }

        private void RegisterLogging(IAppSettings appSettings)
        {
            LogManager
                .SetDefaultLogCategory("")
                .Use(new List<ILogEngine>
                {
                     new SentryLogging(new SentryEngineOptions {
                        Dsn = new Dsn(appSettings.SentryDsn),
                        Environment = "DEV"
                     })
                });
        }
    }
}
