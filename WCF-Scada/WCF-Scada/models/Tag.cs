using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WCF_Scada.models
{
    [DataContract]
    [KnownType(typeof(InputTag))]
    [KnownType(typeof(OutputTag))]
    public class Tag
    {
        [Key]
        [DataMember]  public string Id { get; set; }  // name
        [DataMember]  public string Description { get; set; }
        [Index(IsUnique = true)]
        [MaxLength(255)]
        [DataMember]
        public string IOAddress { get; set; }
    }
}