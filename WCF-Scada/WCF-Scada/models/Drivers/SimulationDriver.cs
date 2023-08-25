using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WCF_Scada.models.Drivers
{
    [DataContract]
    public class SimulationDriver : Driver
    {

        public override double ReturnValue(string address)
        {
            return 0;
        }
    }
}