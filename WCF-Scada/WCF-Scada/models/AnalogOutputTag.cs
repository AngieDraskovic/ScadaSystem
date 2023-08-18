using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WCF_Scada.models
{
    public class AnalogOutputTag : Tag
    {

        public double InitialValue { get; set; }
        public double LowLimit { get; set; }
        public double HighLimit { get; set; }
        public string Units { get; set; }
    }
}