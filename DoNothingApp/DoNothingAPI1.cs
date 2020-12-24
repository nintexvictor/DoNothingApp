using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Context.Propagation;

namespace DoNothingApp
{
    public static class DoNothingApi1
    {
        private static readonly ActivitySource ActivitySource = new ActivitySource("DoNothingApi");
        private static readonly TraceContextPropagator Propagator = new TraceContextPropagator();
        static HttpClient Client = new HttpClient();
        
        [FunctionName("DoNothingAPI1")]
        public static async Task<string> RunAsync(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]
            HttpRequest req, ILogger log)
        {
            Sdk.CreateTracerProviderBuilder()
                .AddSource("DoNothingApi")
                .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("DoNothingApi"))
                .SetSampler(new AlwaysOnSampler())
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddNewRelicExporter(config =>
                {
                    config.ApiKey = "your api key";
                })
                .Build();
                
            var activity = ActivitySource.StartActivity("DoNothingApp1");

            Propagator.Inject(new PropagationContext(activity.Context, Baggage.Current), Client.DefaultRequestHeaders, AddHeader);

            string name = req.Query["name"];
            string result = await Client.GetStringAsync($"https://donothingapi.azurewebsites.net/api/DoNothingAPI2?name={name}");

            activity?.Stop();
            
            return result;
        }
        
        public static void AddHeader(HttpRequestHeaders headers, string key, string value)
        {
            headers.Remove("parentTraceContext");
            headers.Add("parentTraceContext", value);
        }  
    }
}