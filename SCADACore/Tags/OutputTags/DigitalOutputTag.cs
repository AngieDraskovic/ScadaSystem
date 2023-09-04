using SCADACore.Tags.OutputTags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace SCADACore.Tags
{
    [DataContract]
    public class DigitalOutputTag : OutputTag
    {

        public DigitalOutputTag() : base() { }

        public DigitalOutputTag(string tagName, string description, string ioAddress, double initialValue) 
            : base(tagName, description, ioAddress, initialValue) { }
    }
}