using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WCF_Scada.models
{
    [DataContract]
    public class AnalogOutputTag : OutputTag
    {
        [DataMember]  public double LowLimit { get; set; }
        [DataMember]  public double HighLimit { get; set; }
        [DataMember]  public string Units { get; set; }
    }
}