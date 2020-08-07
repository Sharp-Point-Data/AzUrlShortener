using System;
using System.Collections.Generic;
using shortenerTools.Domain;

namespace Cloud5mins.domain
{
    public class WebhookListResponse
    {
        public List<WebhookEntity> UrlList { get; set; }

        public WebhookListResponse(){}
        public WebhookListResponse(List<WebhookEntity> list)
        {
            UrlList = list;
        }
    }
}