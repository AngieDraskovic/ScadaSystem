using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WCF_Scada.models
{
    public class User
    {
        [Key]
        [StringLength(100)]
        public string Username { get; set; }
        public string PasswordHash{ get; set; }
    }
}