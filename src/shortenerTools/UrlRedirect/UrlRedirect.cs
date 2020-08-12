using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http;
using System.Text;
using Cloud5mins.domain;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using shortenerTools.Domain;

namespace Cloud5mins.Function
{
    public static class UrlRedirect
    {
        [FunctionName("UrlRedirect")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "UrlRedirect/{shortUrl}")] HttpRequestMessage req,
            string shortUrl, 
            ExecutionContext context,
            ILogger log)
        {
            log.LogInformation($"C# HTTP trigger function processed for Url: {shortUrl}");

            string redirectUrl = "https://azure.com";

            if (!String.IsNullOrWhiteSpace(shortUrl))
            {
                var config = new ConfigurationBuilder()
                    .SetBasePath(context.FunctionAppDirectory)
                    .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .Build();

                redirectUrl = config["defaultRedirectUrl"];

                StorageTableHelper stgHelper = new StorageTableHelper(config["UlsDataStorage"]); 

                var tempUrl = new ShortUrlEntity(string.Empty, shortUrl);
                
                var newUrl = await stgHelper.GetShortUrlEntity(tempUrl);

                if (newUrl != null)
                {
                    log.LogInformation($"Found it: {newUrl.Url}");
                    newUrl.Clicks++;

                    var host = req.RequestUri.GetLeftPart(UriPartial.Authority);
                    var shortUrlAddress = Utility.GetShortUrl(host, shortUrl);

                    await RunWebhooks(stgHelper, new ClickStatsResponse(newUrl.Title, shortUrlAddress, newUrl.Url, newUrl.Clicks));

                    stgHelper.SaveClickStatsEntity(new ClickStatsEntity(newUrl.RowKey));
                    await stgHelper.SaveShortUrlEntity(newUrl);
                    redirectUrl = newUrl.Url;
                }
            }
            else
            {
                log.LogInformation("Bad Link, resorting to fallback.");
            }

            var res = req.CreateResponse(HttpStatusCode.Redirect);
            res.Headers.Add("Location", redirectUrl);
            return res;
        }

        private static async Task RunWebhooks(StorageTableHelper stgHelper, ClickStatsResponse response)
        {
            var hooks = await stgHelper.GetAllWebhooks();
            using (var client = new HttpClient())
            using (var httpContent = CreateHttpContent(response))
            {
                foreach (var hook in hooks)
                {
                    using (var request = new HttpRequestMessage(HttpMethod.Post, hook.Url))
                    {
                        request.Content = httpContent;
                        await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                    }
                }
            }
        }

        private static StringContent CreateHttpContent(object content)
        {
            StringContent httpContent = null;

            if (content != null)
            {
                var jsonString = JsonConvert.SerializeObject(content);
                httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
            }

            return httpContent;
        }
    }
}
