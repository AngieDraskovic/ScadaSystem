using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using WCF_Scada.Context;
using WCF_Scada.models;

namespace WCF_Scada.Processing
{
    public class UserProcessing
    {

        private static Dictionary<string, User> authenticatedUsers = new Dictionary<string, User>();

        public static string LogIn(string username, string password)
        {
            using (var db = new UserContext())
            {
                var user = db.Users.FirstOrDefault(u => u.Username == username);
                if (user != null)
                {
                    if (ValidateEncryptedData(password, user.PasswordHash))
                    {
                        string token = GenerateToken(username);
                        authenticatedUsers.Add(token, user);
                        return token; 
                    }
                }
            }
            return null; 
        }

        public static void LogOut(string token)
        {
            authenticatedUsers.Remove(token);
        }

        public static bool IsAuthenticatedUser(string token)
        {
            return authenticatedUsers.ContainsKey(token);
        }

        public static bool RegisterUser(string username, string password)
        {
            string encryptedPassword = EncryptValue(password);
            User user = new User()
            {
                Username = username,
                PasswordHash = encryptedPassword
            };

            using (var db = new UserContext())
            {
                try
                {
                    db.Users.Add(user);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Exception: " + ex.ToString());
                    return false;
                }
            }
            return true;
        }
        private static string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            { 
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        private static bool VerifyPasswordHash(string inputPassword, string storedHash)
        {
            string inputHash = ComputeSha256Hash(inputPassword);
            return inputHash == storedHash;
        }

        private static string GenerateSalt()
        {
            RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider();
            byte[] salt = new byte[32];
            crypto.GetBytes(salt);
            return Convert.ToBase64String(salt);
        }

        private static bool ValidateEncryptedData(string valueToValidate, string valueFromDatabase)
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


        private static string GenerateToken(string username)
        {
            RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider();
            byte[] randVal = new byte[32];
            crypto.GetBytes(randVal);
            string randStr = Convert.ToBase64String(randVal);
            return username + randStr;
        }

        private static string EncryptValue(string value)
        {
            string saltValue = GenerateSalt();
            byte[] saltedPassword = Encoding.UTF8.GetBytes(saltValue + value);
            using (SHA256Managed sha = new SHA256Managed())
            {
                byte[] hash = sha.ComputeHash(saltedPassword);
                return $"{Convert.ToBase64String(hash)}:{saltValue}";
            }
        }
    }
}