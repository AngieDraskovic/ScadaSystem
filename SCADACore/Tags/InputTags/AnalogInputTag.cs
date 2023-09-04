using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace SCADACore.Tags.InputTags
{
    [DataContract]
    public class AnalogInputTag : InputTag
    {
        [DataMember]
        public double LowLimit { get; set; }
        [DataMember]
        public double HighLimit { get; set; }
        [DataMember]
        public string Unit { get; set; }
        [DataMember]
        public List<Alarm> Alarms { get; set; }

        public AnalogInputTag() : base() { }

        public AnalogInputTag(string tagName, string description, DriverType driver, string ioAddress,
            int scanTime, List<Alarm> alarms, bool scanOn, double lowLimit, double highLimit, string unit)
            : base(tagName, description, driver, ioAddress, scanTime, scanOn)
        {
            LowLimit = lowLimit;
            HighLimit = highLimit;
            Alarms = alarms;
            Unit = unit;    
        }
    }
}