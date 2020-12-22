using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

[assembly: FunctionsStartup(typeof(FunctionApp1.Startup))]
namespace FunctionApp1
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddOpenTelemetryTracing((serviceProvider, tracerBuilder) =>
            {
                // Make the logger factory available to the dependency injection
                // container so that it may be injected into the OpenTelemetry Tracer.
                var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
                var configuration = serviceProvider.GetRequiredService<IConfiguration>();

                // Adds the New Relic Exporter loading settings from the appsettings.json
                tracerBuilder
                    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(configuration["NewRelicAppName"]))
                    .AddConsoleExporter()
                    .AddNewRelicExporter(options =>
                    {
                        options.ApiKey = configuration["NewRelicInsertApiKey"];
                    })
                    .SetSampler(new AlwaysOnSampler())
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation();

                ILogger logger = loggerFactory.CreateLogger<Startup>();

                logger.LogInformation("Starting up, wiring up OpenTelemtry...");
            });
        }
    }
}
