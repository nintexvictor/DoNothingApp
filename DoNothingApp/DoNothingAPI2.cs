using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace DoNothingApp
{
    public class DoNothingApi2
    {
        
        private static readonly ActivitySource ActivitySource = new ActivitySource("DoNothingApi");
        private static readonly TraceContextPropagator Propagator = new TraceContextPropagator();
        private string ParentContext { get; set; }
        ActivityContext activityContext = new ActivityContext();

        [FunctionName("DoNothing2API")]
        public IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log,
            ExecutionContext context)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            if (req.Headers.ContainsKey("parentTraceContext"))
            {
                ParentContext = req.Headers["parentTraceContext"];
                string[] parentContextVals = ParentContext.Split("-");
                activityContext = new ActivityContext(
                    ActivityTraceId.CreateFromString(parentContextVals[1]),
                    ActivitySpanId.CreateFromString(parentContextVals[2]),
                    ActivityTraceFlags.None);
            }
            
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
            
            var activity = ActivitySource.StartActivity("DoNothingApp2", ActivityKind.Consumer, activityContext);
            activity?.SetParentId(ParentContext);

            string name = req.Query["name"];

            if (req.Method.ToLower() == "post")
            {
                name = req.ReadAsStringAsync().Result;
            }

            string responseMessage = string.IsNullOrEmpty(name)
                ? $"This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response. (ParentId: {ParentContext})"
                : $"Hello, {name}. This HTTP triggered function executed successfully. (parentId: {ParentContext})";
            
            activity?.Stop();
            return new OkObjectResult(responseMessage);
        }
    }
}
