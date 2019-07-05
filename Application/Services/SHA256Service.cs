using Application.Interfaces.Services;
using System.Text;
using System.Security.Cryptography;

namespace Application.Services
{
    /// <summary>
    /// SHA256-based implementation for a hash interface
    /// </summary>
    public class SHA256Service : IHashService
    {
        public string Hash(string source)
        {
            var sourceBytes = Encoding.UTF8.GetBytes(source);

            var hashBytes = new SHA256Managed().ComputeHash(sourceBytes);

            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < hashBytes.Length; i++)
            {
                builder.Append(hashBytes[i].ToString("x2"));
            }

            return builder.ToString();
        }
    }
}
