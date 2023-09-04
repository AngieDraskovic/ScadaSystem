using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SCADACore.Database.Model
{
    public class TagValue
    {
        [Key]
        public int Id { get; set; }
        public string TagName { get; set; }
        public DateTime TimeStamp { get; set; }
        public double Value { get; set; }
        public string Type { get; set; }

        public TagValue() { }

        public TagValue(string tagName, double value, string type)
        {
            TagName = tagName;
            Value = value;
            TimeStamp = DateTime.Now;
            Type = type;
        }
    }
}