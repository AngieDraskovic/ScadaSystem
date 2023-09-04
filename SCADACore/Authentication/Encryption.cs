using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace SCADACore.Authentication
{
    public static class Encryption
    {
        public static string EncryptData(string valueToEncrypt)
        {
            return EncryptValue(valueToEncrypt);
        }

        private static string GenerateSalt()
        {
            RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider();
            byte[] salt = new byte[32];
            crypto.GetBytes(salt);
            return Convert.ToBase64String(salt);
        }
        private static string EncryptValue(string strValue)
        {
            string saltValue = GenerateSalt();
            byte[] saltedPassword = Encoding.UTF8.GetBytes(saltValue + strValue);
            using (SHA256Managed sha = new SHA256Managed())
            {
                byte[] hash = sha.ComputeHash(saltedPassword);
                return $"{Convert.ToBase64String(hash)}:{saltValue}";
            }
        }
    }
}