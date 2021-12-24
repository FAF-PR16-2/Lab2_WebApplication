using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace WebApplication
{
    public class PasswordHasher
    {
        public string HashString(string password)
        {
            var salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            var hashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256/8));

            return $"{Convert.ToBase64String(salt)}.{hashedPassword}";
        }

        public bool VerifyPassword(string password, string storedHash)
        {
            var salt = Convert.FromBase64String(storedHash.Split(".")[0]);
            var hash = storedHash.Split(".")[1];

            var hashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256/8));

            return hashedPassword == hash;
        }
    }
}