using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace  adminBlazorWebsite.Data
{
    public class WebhookEntity
    {
        public string PartitionKey { get; set; }

        public string RowKey { get; set; }
        [Required]
        [Url]
        public string Url { get; set; }

        public WebhookEntity(){}

        public static ShortUrlEntity GetEntity(string url)
        {
            return new ShortUrlEntity
            {
                PartitionKey = "Webhook",
                RowKey = Guid.NewGuid().ToString(),
                Url = url
            };
        }

        public string GetDisplayableUrl(){

            var length = Url.ToString().Length;
            if (length >= 50){
                return string.Concat(Url.Substring(0,49), "...");
            }
            return Url;
        }
    }


}
