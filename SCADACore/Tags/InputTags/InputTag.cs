using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace SCADACore.Tags.InputTags
{
    public enum DriverType
    {
        SIMULATION_DRIVER,
        REAL_TIME_DRIVER
    }

    [DataContract]
    public abstract class InputTag : Tag
    {
        [DataMember]
        public DriverType Driver { get; set; }
        [DataMember]
        public int ScanTime { get; set; }
        [DataMember]
        public bool ScanOn { get; set; }

        public InputTag() : base() { }

        public InputTag(string tagName, string description, DriverType driver, string ioAddress,
            int scanTime, bool scanOn) : base(tagName, description, ioAddress)
        {
            Driver = driver;
            ScanTime = scanTime;
            ScanOn = scanOn;
        }
    }
}