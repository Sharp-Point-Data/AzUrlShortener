using System;
using System.Collections.Generic;
using System.Text;

namespace shortenerTools.Domain
{
    public class ClickStatsResponse
    {
        public ClickStatsResponse(string title, string url, string fullUrl, int count) =>
            (Title,Url,FullUrl,Count) = (title,url, fullUrl,count);

        public int Count { get; private set; }
        public string FullUrl { get; private set; }
        public string Url { get; private set; }
        public string Title { get; private set; }
    }
}
