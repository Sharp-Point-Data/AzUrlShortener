using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.Cosmos.Table;

namespace shortenerTools.Domain
{
    public class WebhookEntity : TableEntity
    {
        public string Url { get; set; }

        public WebhookEntity()
        {
        }


        public WebhookEntity(string url)
        {
            PartitionKey = "Webhooks";
            RowKey = Guid.NewGuid().ToString();
            Url = url;
        }
    }
}
