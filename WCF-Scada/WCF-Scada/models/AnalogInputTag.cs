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
        public double LowLimit { get; set; }
        public double HighLimit { get; set; }
        public string Units { get; set; }
    }
}