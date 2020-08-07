using System;
using System.Linq;
using System.Threading.Tasks;

namespace Cloud5mins.domain
{
    public static class Utility
    {
        private const string Alphabet = "abcdefghijklmnopqrstuvwxyz0123456789";
        private static readonly int Base = Alphabet.Length;

        public static string GetValidEndUrl(string vanity)
        {
            if(string.IsNullOrEmpty(vanity))
            {
                string code = GetUrlSafeBase64Guid();
                return string.Join(string.Empty, code);
            }
            else
            {
                return string.Join(string.Empty, vanity);
            }
        }

        private static string GetUrlSafeBase64Guid()
        {
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray())
                .Replace("/", "-")
                .Replace("+", "_")
                .Replace("=", "");
        }

        public static string GetShortUrl(string host, string vanity){
               return host + "/" + vanity;
        }
    }
}