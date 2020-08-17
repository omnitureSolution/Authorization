using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace AuthServer.Persistence
{
    public class AppUser
    {
        public int id = 0;
        public string SubjectId = string.Empty;
        public string Username = string.Empty;

        public List<Claim> Claims = new List<Claim>();
        public static string Create(string value, string salt)
        {
            value = value.ToLower();
            salt = salt.ToLower();
            var valueBytes = KeyDerivation.Pbkdf2(
                password: value,
                salt: Encoding.UTF8.GetBytes(salt.ToLower()),
                prf: KeyDerivationPrf.HMACSHA512,
                iterationCount: 10000,
                numBytesRequested: 256 / 8);

            return Convert.ToBase64String(valueBytes);
        }
        public static bool Validate(string value, string key, string hash)
            => Create(value, key) == hash;

    }
}
