using SCADACore.Authentication;
using SCADACore.Database.Context;
using SCADACore.DBModel;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADACore
{
    public static class UserProcessing
    {
        private static ConcurrentDictionary<string, User> _userDict = new ConcurrentDictionary<string, User>();
        private static ConcurrentDictionary<string, User> _authenticatedUsers = new ConcurrentDictionary<string, User>();

        public static string Login(string username, string password)
        {
            foreach (var user in _userDict.Values)
            {
                if (username == user.Username && Validation.ValidateEncryptedData(password, user.EncryptedPassword))
                {
                    string token = Validation.GenerateToken(user.Username);
                    _authenticatedUsers.TryAdd(token, user);
                    return token;
                }
            }
            return "";
        }

        public static void Logout(string token)
        {
            _authenticatedUsers.TryRemove(token, out User value);
        }

        public static bool RegisterUser(string username, string password, string token = "")
        {
            string encryptedPassword = Encryption.EncryptData(password);

            try
            {
                Role role = CanRegister(token);
                User user = new User(username, encryptedPassword, role);

                SaveUser(user);
                _userDict.TryAdd(username, user);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static Role CanRegister(string token)
        {
            if (UsersEmpty())
            {
                return Role.ADMIN;
            }

            if (IsUserAuthenticated(token, Role.ADMIN))
            {
                return Role.USER;
            }

            throw new Exception();
        }

        public static bool IsUserAuthenticated(string token, Role role)
        {
            if (_authenticatedUsers.ContainsKey(token))
            {
                _authenticatedUsers.TryGetValue(token, out User user);
                if (user.Role >= role)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsUserAdmin(string token)
        {
            if (_authenticatedUsers.ContainsKey(token))
            {
                _authenticatedUsers.TryGetValue(token, out User user);
                if (user.Role == Role.ADMIN)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool UsersEmpty()
        {
            if (_userDict.IsEmpty)
            {
                return true;
            }

            return false;
        }

        public static void LoadUsers()
        {
            using (var db = new SCADAContext())
            {
                foreach (var user in db.Users)
                {
                    _userDict.TryAdd(user.Username, user);
                }
            }
        }

        public static void SaveUser(User user)
        {
            using (var db = new SCADAContext())
            {
                db.Users.Add(user);
                db.SaveChanges();
            }
        }
    }
}
