using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net.Http;

namespace FunctionApp1
{
    public class DoNothingAPI
    {
        private readonly ILogger<DoNothingAPI> _logger;

        public DoNothingAPI(ILogger<DoNothingAPI> logger)
        {
            _logger = logger;
        }

        [FunctionName("DoNothingAPI")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ExecutionContext context)
        {
            this._logger.LogInformation("C# HTTP trigger function processed a request.");

            string correlationId = string.IsNullOrWhiteSpace(req.Query["id"]) ? Guid.NewGuid().ToString() : req.Query["id"].ToString();

            string name = req.Query["name"];

            if (req.Method.ToLower() == "post")
            {
                name = await req.ReadAsStringAsync();
            }

            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync($"http://{Environment.GetEnvironmentVariable("WEBSITE_HOSTNAME")}/api/DoNothing2API?name={name}&id={correlationId}");

            string responseMessage = await response.Content.ReadAsStringAsync();

            return new OkObjectResult(responseMessage);
        }
    }
}
