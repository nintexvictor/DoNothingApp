using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
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
            var activitySource = new ActivitySource("DoNothingApp");

            builder.Services.AddSingleton(activitySource);

            var openTelemetry = Sdk.CreateTracerProviderBuilder()
                .AddSource("DoNothingApp")
                .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("DoNothingApp"))
                .AddConsoleExporter()
                .SetSampler(new AlwaysOnSampler())
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .Build();

            builder.Services.AddSingleton(openTelemetry);
        }
    }
}
