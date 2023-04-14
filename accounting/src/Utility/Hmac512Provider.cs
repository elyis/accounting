using System.Security.Cryptography;
using System.Text;

namespace accounting.src.Utility
{
    public class Hmac512Provider
    {
        private static readonly string _key = "test";


        public static string Compute(string value)
        {
            using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(_key));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(value));
            return Convert.ToBase64String(hash);
        }
    }
}
