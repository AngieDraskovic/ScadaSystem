using SCADACore.RealTimeDriverInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace SCADACore
{
    public class RealTimeDriverService : IRealTimeDriver
    {
        public bool InitRealTimeUnit(string rtuId, string path, string rtuAddress)
        {
            return TagProcessing.InitRealTimeUnit(rtuId, path, rtuAddress);
        }

        public void WriteValue(string rtuId, string value, string rtuAddress, byte[] signature)
        {
            TagProcessing.WriteValue(rtuId, value, rtuAddress, signature);
        }
    }
}
