using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WCF_Scada.models.Drivers
{

    [DataContract]
    [KnownType(typeof(SimulationDriver))]
    [KnownType(typeof(RealTimeDriver))]
    public abstract class Driver
    {
        public abstract double ReturnValue(string address);
    }

}