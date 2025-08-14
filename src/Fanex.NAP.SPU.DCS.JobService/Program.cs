namespace Fanex.NAP.SPU.DCS.JobService
{
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage(Justification = "Excluded from code coverage")]
    // for setting up and running the ASP.NET Core application, which is not typically unit tested.
    public static class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            SentrySdk.CaptureMessage(string.Format("{0} has been started at {1}", typeof(Program).FullName, DateTimeOffset.Now));

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var env = hostingContext.HostingEnvironment;

                    config.SetBasePath(env.ContentRootPath);
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

                    config.AddEnvironmentVariables();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    // Use HTTP for container - SSL termination handled by HAProxy/load balancer
                    // This is a common pattern in microservices architecture
                    // SonarQube: Suppressing HTTP warning as SSL termination is handled by HAProxy
                    webBuilder.UseUrls("http://*:5000"); // NOSONAR

                    webBuilder
                    .UseSentry()
                    .UseStartup<Startup>();
                });
        }

    }
}
