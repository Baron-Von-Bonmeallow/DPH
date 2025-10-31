using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
namespace Password.Hash
{
    internal class Hash
    {
        static void Secondary(string[] args)
        {
            Console.WriteLine("Insert Password:");
            string password = Console.ReadLine();

            byte[] salt;
            Console.Write("Salt Cipher (Y/N)? ");
            string? SLuse = Console.ReadLine();
            if (SLuse?.ToLower() != "y") 
            {
                salt = [];
            }
            else
            {
                salt = new byte[128 / 8];
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(salt);
                }
            }
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));
            Console.WriteLine($"Hashed Password: {hashed}");
        }
    }
}
