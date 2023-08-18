using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WCF_Scada.models
{
    public class Tag
    {
        [Key]
        public string Id { get; set; }  // name
        public string Description { get; set; }
        public string IOAddress { get; set; }
    }
}