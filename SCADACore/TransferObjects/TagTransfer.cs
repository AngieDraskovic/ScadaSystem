using SCADACore.Tags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace SCADACore.TransferObjects
{
    [DataContract]
    public class TagTransfer
    {
        [DataMember]
        public Tag Tag { get; set; }
        [DataMember]
        public double Value { get; set; }

        public TagTransfer() { }

        public TagTransfer(Tag tag, double value)
        {
            Tag = tag;
            Value = value;
        }
    }
}