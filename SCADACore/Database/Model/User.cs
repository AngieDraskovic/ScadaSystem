using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SCADACore.DBModel
{
    public enum Role
    {
        USER,
        ADMIN
    }

    public class User
    {
        [Key]
        public int Id { get; set; }

        [StringLength(450)]
        [Index(IsUnique = true)]
        public string Username { get; set; }

        public string EncryptedPassword { get; set; }

        public Role Role { get; set; }

        public User() { }

        public User(string username, string encryptedPassword, Role role)
        {
            Username = username;
            EncryptedPassword = encryptedPassword;
            Role = role;
        }
    }
}