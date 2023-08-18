using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WCF_Scada.models
{
    public class DigitalInputTag : Tag
    {
        public Driver Driver { get; set; }
        public int ScanTime { get; set; }
        public bool OnScan { get; set; }
    }
}