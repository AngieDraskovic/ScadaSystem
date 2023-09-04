using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace SCADACore.Tags.InputTags
{
    public enum AlarmType
    {
        LOW,
        HIGH
    }

    [DataContract]
    public class Alarm
    {
        [DataMember]
        public AlarmType AlarmType { get; set; }
        [DataMember]
        public int Priority { get; set; }
        [DataMember]
        public string TagName { get; set; }
        [DataMember]
        public int AlarmId { get; set; }

        public Alarm() { }

        public Alarm(AlarmType alarmType, int priority, string tagName)
        {
            AlarmType = alarmType;
            Priority = priority;
            TagName = tagName;
        }
    }
}