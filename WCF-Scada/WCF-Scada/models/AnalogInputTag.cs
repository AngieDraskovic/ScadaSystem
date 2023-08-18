using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WCF_Scada.models
{
    public class AnalogInputTag : Tag
    {
        private List<Alarm> alarms;
        public Driver Driver { get; set; }
        public int ScanTime { get; set; }
        public bool OnScan { get; set; }
        [DataMember] public double LowLimit { get; set; }
        [DataMember] public double HighLimit { get; set; }
        [DataMember] public string Units { get; set; }
    }
}