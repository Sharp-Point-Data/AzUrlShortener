using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace adminBlazorWebsite.Data
{
    public class WebhookRequest
    {
        [Required]
        [Url]
        public string Url { get; set; }
    }
}
