using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace SCADACore.Tags.InputTags
{
    [DataContract]
    public class DigitalInputTag : InputTag
    {

        public DigitalInputTag() : base() { }

        public DigitalInputTag(string tagName, string description, DriverType driver, string ioAddress,
            int scanTime, List<Alarm> alarms, bool scanOn)
            : base(tagName, description, driver, ioAddress,scanTime, scanOn) { }
    }
}