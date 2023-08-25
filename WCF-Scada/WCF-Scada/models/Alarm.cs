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
    public class Alarm
    {
        [DataMember] public int Id { get; set; }  

        [DataMember] public AlarmType Type { get; set; }  

        [DataMember] public int Priority { get; set; }  // Prioritet alarma: 1, 2, ili 3

        [DataMember] public double ThresholdValue { get; set; }  // Granična vrijednost

        [DataMember] public string VariableName { get; set; }  

       
        [ForeignKey("AnalogInputTag")]
        public string AnalogInputTagId { get; set; }
        public virtual AnalogInputTag AnalogInputTag { get; set; }
    }

    public enum AlarmType
    {
        Low,
        High
    }

}