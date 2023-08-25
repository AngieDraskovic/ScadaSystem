using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using WCF_Scada.models.Drivers;
namespace WCF_Scada.models
{


    public enum DriverType
    {
        Simulation,
        RealTime
    }


    [DataContract]
    [KnownType(typeof(AnalogInputTag))]
    [KnownType(typeof(DigitalInputTag))]
    public class InputTag : Tag
    {
        [DataMember]  public DriverType DriverType { get; set; }
        [DataMember] public int ScanTime { get; set; }
        [DataMember] public bool OnScan { get; set; }

    }

    [DataContract]
    public class AnalogInputTag : InputTag
    {
        private List<Alarm> alarms;
        [DataMember] public double LowLimit { get; set; }
        [DataMember] public double HighLimit { get; set; }
        [DataMember] public string Units { get; set; }

        [DataMember]
        public List<Alarm> Alarms
        {
            get
            {
                if (alarms == null) alarms = new List<Alarm>();
                return alarms;
            }
            set
            {
                alarms = value;
            }
        }

        public AnalogInputTag()
        {
            alarms = new List<Alarm>();
        }

        public void AddAlarm(Alarm alarm)
        {
            if (alarms == null)
                alarms = new List<Alarm>();
            alarms.Add(alarm);
        }
    }
}
