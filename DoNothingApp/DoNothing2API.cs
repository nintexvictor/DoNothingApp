using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace FunctionApp1
{
    public class DoNothing2API
    {
        private readonly ILogger<DoNothing2API> _logger;

        public DoNothing2API(ILogger<DoNothing2API> logger)
        {
            _logger = logger;
        }

        [FunctionName("DoNothing2API")]
        public IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ExecutionContext context)
        {
            this._logger.LogInformation("C# HTTP trigger function processed a request.");

            string correlationId = string.IsNullOrWhiteSpace(req.Query["id"]) ? Guid.NewGuid().ToString() : req.Query["id"].ToString();

            string name = req.Query["name"];

            if (req.Method.ToLower() == "post")
            {
                name = req.ReadAsStringAsync().Result;
            }

            string responseMessage = string.IsNullOrEmpty(name)
                ? $"This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response. (CorrelationId: {correlationId})"
                : $"Hello, {name}. This HTTP triggered function executed successfully. (CorrelationId: {correlationId})";

            return new OkObjectResult(responseMessage);
        }
    }
}
