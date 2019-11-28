using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Foutloos
{
    public static class SecurePasswordHasher
    {
        //Size of the salt.
        private const int SaltSize = 8;

        //Size of the hash.
        private const int HashSize = 5;

        //This function takes a password and the amount of iterations and returns a hashed password.
        public static string Hash(string password, int iterations)
        {
            // Create salt
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[SaltSize]);

            // Create hash
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations);
            var hash = pbkdf2.GetBytes(HashSize);

            // Combine salt and hash
            var hashBytes = new byte[SaltSize + HashSize];
            Array.Copy(salt, 0, hashBytes, 0, SaltSize);
            Array.Copy(hash, 0, hashBytes, SaltSize, HashSize);

            // Convert to base64
            var base64Hash = Convert.ToBase64String(hashBytes);

            // Format hash with extra information
            return string.Format(base64Hash);
        }

        //This fucntion takes the password without the amount of iterations, runs Hash with 10.000 iterations.
        public static string Hash(string password)
        {
            return Hash(password, 10000);
        }

        //Checks if the hash is supported.
        public static bool IsHashSupported(string hashString)
        {
            return hashString.Contains("$MYHASH$V1$");
        }

        //Checks a password against a hash.
        public static bool Verify(string password, string hashedPassword)
        {
            // Extract iteration and Base64 string
            var base64Hash = hashedPassword;

            // Get hash bytes
            var hashBytes = Convert.FromBase64String(base64Hash.ToString());

            // Get salt
            var salt = new byte[SaltSize];
            Array.Copy(hashBytes, 0, salt, 0, SaltSize);

            // Create hash with given salt
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(HashSize);

            // Get result
            for (var i = 0; i < HashSize; i++)
            {
                if (hashBytes[i + SaltSize] != hash[i])
                {
                    return false;
                }
            }
            return true;
        }
    } 
}
