using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WCF_Scada.models
{
    [DataContract]
    [KnownType(typeof(AnalogOutputTag))]
    [KnownType(typeof(DigitalOutputTag))]
    public class OutputTag : Tag
    {
        [DataMember]  public double InitialValue { get; set; }
    }
    [DataContract]
    public class DigitalOutputTag : OutputTag
    {
    }
}