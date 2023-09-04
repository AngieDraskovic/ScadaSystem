using SCADACore.Tags.InputTags;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SCADACore.Database.Model
{
    public class AlarmTime
    {
        [Key]
        public int Id { get; set; }
        public AlarmType AlarmType { get; set; }
        public double Value { get; set; }
        public double Limit { get; set; }
        public int Priority { get; set; }
        public string TagName { get; set; }
        public DateTime TimeStamp { get; set; }

        public AlarmTime() { }

        public AlarmTime(Alarm alarm, double value, double limit)
        {
            AlarmType = alarm.AlarmType;
            Priority = alarm.Priority;
            TagName = alarm.TagName;
            TimeStamp = DateTime.Now;
            Value = value;
            Limit = limit;
        }
    }
}