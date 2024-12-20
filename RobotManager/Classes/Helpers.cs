using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RobotManager.Classes
{
    public static class Helpers
    {
        public static string Hash(string password, string? challenge = null)
        {
            using var sha256 = SHA256.Create();

            string clearText = challenge == null ? password : Hash(password) + challenge;
            var bytes = Encoding.UTF8.GetBytes(clearText);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        public static string? GenerateUserSecret(DateTime dateTime)
        {
            using var sha256 = SHA256.Create();
            string? token = Preferences.Get("token", null);
            if (token == null)
            {
                return null;
            }
            return Hash(token, dateTime.ToString());
        }
    }
}
