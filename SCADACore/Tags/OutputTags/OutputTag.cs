using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace SCADACore.Tags.OutputTags
{
    [DataContract]
    public abstract class OutputTag : Tag
    {
        [DataMember]
        public double InitialValue { get; set; }

        public OutputTag() : base() { }

        public OutputTag(string tagName, string description, string ioAddress, double initialValue) : base(tagName, description, ioAddress)
        {
            InitialValue = initialValue;
        }
    }
}