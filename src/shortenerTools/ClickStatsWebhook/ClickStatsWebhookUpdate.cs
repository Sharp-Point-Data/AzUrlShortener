using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Cloud5mins.domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using shortenerTools.Domain;

namespace shortenerTools.ClickStatsWebhook
{
    public static class ClickStatsWebhookUpdate
    {
        [FunctionName("WebhookUpdate")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequestMessage req,
            ILogger log,
            ExecutionContext context)
        {
            log.LogInformation($"C# HTTP trigger function processed this request: {req}");

            var input = await req.Content.ReadAsAsync<WebhookEntity>();

            if (input == null)
            {
                return req.CreateResponse(HttpStatusCode.NotFound);
            }

            // If the Url parameter only contains whitespaces or is empty return with BadRequest.
            if (string.IsNullOrWhiteSpace(input.Url))
            {
                return req.CreateErrorResponse(HttpStatusCode.BadRequest, "The url parameter can not be empty.");
            }

            // Validates if input.url is a valid aboslute url, aka is a complete refrence to the resource, ex: http(s)://google.com
            if (!Uri.IsWellFormedUriString(input.Url, UriKind.Absolute))
            {
                return req.CreateErrorResponse(HttpStatusCode.BadRequest, $"{input.Url} is not a valid absolute Url. The Url parameter must start with 'http://' or 'http://'.");
            }

            WebhookResponse response = new WebhookResponse {Url = input.Url};
            var config = new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            StorageTableHelper stgHelper = new StorageTableHelper(config["UlsDataStorage"]);

            try
            {
                await stgHelper.UpdateWebhookEntity(input);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "An unexpected error was encountered.");
                return req.CreateResponse(HttpStatusCode.BadRequest, ex);
            }

            return req.CreateResponse(HttpStatusCode.OK, response);
        }
    }
}
