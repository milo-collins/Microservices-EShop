using MassTransit.Futures.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EShop.Infrastructure.Security
{
    public class Encrypter : IEncrypter
    {
        private readonly string Salt = "w-gE1#8TAuAWQ4iAw)o4gtp:jGJy9@2S?CxK=F*R=(SnW::WSB";
        public string GetHash(string value, string salt)
        {
            // Salt = data added to hashing process to increase security
            // Uses random number gen based on HMACSHA1
            // Takes a password, salt, and iteration count, thengenerates keys through calls to GetBytes
            var derivedBytes = new Rfc2898DeriveBytes(value, GetBytes(salt), 1000);
            return Convert.ToBase64String(derivedBytes.GetBytes(50));
        }

        public string GetSalt()
        {
            return Salt;
        }

        private static byte[] GetBytes(string value) {
            var bytes = new Byte[value.Length];
            Buffer.BlockCopy(value.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }
    }
}
