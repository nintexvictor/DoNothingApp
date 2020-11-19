using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NewRelic.Azure.BindingExtension;
using System.Net.Http;

namespace FunctionApp1
{
    public static class DoNothingAPI
    {
        [FunctionName("DoNothingAPI")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log,
            ExecutionContext context,
            [NewRelic.Azure.BindingExtension.NewRelicTelemetry(NewRelicAppName = "DoNothing")] ICollector<NewRelic.Azure.BindingExtension.NewRelicCollector> collector)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var newRelicCollector = new NewRelic.Azure.BindingExtension.NewRelicCollector(context);
            newRelicCollector.GenerateMetrics();
            newRelicCollector.GenerateTraces(req);

            collector.Add(newRelicCollector);

            string name = req.Query["name"];

            if (req.Method.ToLower() == "post")
            {
                name = await req.ReadAsStringAsync();
            }

            string correlationId = string.IsNullOrWhiteSpace(req.Query["id"]) ? Guid.NewGuid().ToString() : req.Query["id"].ToString();

            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync($"http://{Environment.GetEnvironmentVariable("WEBSITE_HOSTNAME")}/api/DoNothing2API?name={name}&id={correlationId}");

            string responseMessage = await response.Content.ReadAsStringAsync();

            newRelicCollector.TraceCollector.AddTraceTag("Do", "Nothing2");
            newRelicCollector.TraceCollector.AddTraceTag("correlationId", correlationId);

            return new OkObjectResult(responseMessage);
        }
    }
}
