using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace SCADACore.RealTimeDriverInterface
{
    [ServiceContract]
    public interface IRealTimeDriver
    {
        [OperationContract]
        bool InitRealTimeUnit(string rtuId, string path, string rtuAddress);

        [OperationContract]
        void WriteValue(string rtuId, string value, string rtuAddress, byte[] signature);
    }
}
