using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace SCADACore.Authentication
{
    public static class Validation
    {
        private static readonly string IMPORT_FOLDER = @"C:\public_key\";
        private static CspParameters csp;
        private static RSACryptoServiceProvider rsa;

        public static bool ValidateEncryptedData(string valueToValidate, string valueFromDatabase)
        {
            string[] arrValues = valueFromDatabase.Split(':');
            string encryptedDbValue = arrValues[0];
            string salt = arrValues[1];
            byte[] saltedValue = Encoding.UTF8.GetBytes(salt + valueToValidate);
            using (var sha = new SHA256Managed())
            {
                byte[] hash = sha.ComputeHash(saltedValue);
                string enteredValueToValidate = Convert.ToBase64String(hash);
                return encryptedDbValue.Equals(enteredValueToValidate);
            }
        }
        
        public static string GenerateToken(string username)
        {
            RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider();
            byte[] randVal = new byte[32];
            crypto.GetBytes(randVal);
            string randStr = Convert.ToBase64String(randVal);
            return username + randStr;
        }

        public static bool VerifySignedMessage(string message, byte[] signature)
        {
            using (SHA256 sha = SHA256Managed.Create())
            {
                var hashValue = sha.ComputeHash(Encoding.UTF8.GetBytes(message));
                var deformatter = new RSAPKCS1SignatureDeformatter(rsa);
                deformatter.SetHashAlgorithm("SHA256");
                return deformatter.VerifySignature(hashValue, signature);
            }
        }

        public static void ImportPublicKey(string keyPath)
        {
            string path = Path.Combine(IMPORT_FOLDER, keyPath);

            FileInfo fi = new FileInfo(path);
            if (fi.Exists)
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    csp = new CspParameters();
                    rsa = new RSACryptoServiceProvider(csp);
                    string publicKeyText = reader.ReadToEnd();
                    rsa.FromXmlString(publicKeyText);
                }
            }

        }
    }
}