using System.Security.Cryptography;
using System.Text;

namespace FlightDocSystem.Helper

{
    public class Md5Helper
    {
        public static string Hash(string input)
        {
            using var md5 = MD5.Create();
            var bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
            var sb= new StringBuilder();
            foreach(var b in bytes)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }
    }
}
