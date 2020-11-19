using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NewRelic.Azure.BindingExtension;

namespace FunctionApp1
{
    public static class DoNothing2API
    {
        [FunctionName("DoNothing2API")]
        public static IActionResult Run(
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
                name = req.ReadAsStringAsync().Result;
            }

            string correlationId = string.IsNullOrWhiteSpace(req.Query["id"]) ? Guid.NewGuid().ToString() : req.Query["id"].ToString();

            string responseMessage = string.IsNullOrEmpty(name)
                ? $"This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response. (CorrelationId: {correlationId})"
                : $"Hello, {name}. This HTTP triggered function executed successfully. (CorrelationId: {correlationId})";

            newRelicCollector.TraceCollector.AddTraceTag("Do", "Nothing");
            newRelicCollector.TraceCollector.AddTraceTag("correlationId", correlationId);

            return new OkObjectResult(responseMessage);
        }
    }
}
