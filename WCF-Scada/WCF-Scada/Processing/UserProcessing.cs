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

        public static string LogIn(string username, string password)
        {
            using (var db = new UserContext())
            {
                var user = db.Users.FirstOrDefault(u => u.Username == username);
                if (user != null)
                {
                    if (VerifyPasswordHash(password, user.PasswordHash))
                    { 
                        return "Login uspjesan!";
                    }
                }
            }
            return "Login neuspjesan";
        }


        public static bool RegisterUser(string username, string password)
        {
            User user = new User()
            {
                Username = username,
                PasswordHash = ComputeSha256Hash(password)
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



    }
}