using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace SCADACore.Tags.OutputTags
{
    [DataContract]
    public class AnalogOutputTag : OutputTag
    {
        [DataMember]
        public double LowLimit { get; set; }
        [DataMember]
        public double HighLimit { get; set; }
        [DataMember]
        public string Unit { get; set; }

        public AnalogOutputTag() : base() { }

        public AnalogOutputTag(string tagName, string description, string ioAddress, double initialValue, double lowLimit, double hightLimit, string unit)
            : base(tagName, description, ioAddress, initialValue) 
        {
            LowLimit = lowLimit;
            HighLimit = hightLimit;
            Unit = unit;
        }
    }
}