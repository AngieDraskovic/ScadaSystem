using SCADACore.Tags.InputTags;
using SCADACore.Tags.OutputTags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Xml.Serialization;

namespace SCADACore.Tags
{
    [DataContract]
    [KnownType(typeof(OutputTag))]
    [KnownType(typeof(DigitalOutputTag))]
    [KnownType(typeof(AnalogOutputTag))]
    [KnownType(typeof(InputTag))]
    [KnownType(typeof(DigitalInputTag))]
    [KnownType(typeof(AnalogInputTag))]
    [XmlInclude(typeof(DigitalInputTag))]
    [XmlInclude(typeof(AnalogInputTag))]
    public abstract class Tag
    {
        [DataMember]
        public string TagName { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public string IoAddress { get; set; }

        public Tag() { }

        public Tag(string tagName, string description, string ioAddress)
        {
            TagName = tagName;
            Description = description;
            IoAddress = ioAddress;
        }
    }
}