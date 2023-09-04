using SCADACore.Database.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace SCADACore.Interfaces.AlarmDisplayInterface
{
    [ServiceContract(CallbackContract = typeof(IAlarmDisplayCallback))]
    public interface IAlarmDisplay
    {
        [OperationContract(IsOneWay = true)]
        void AlarmDisplayInit();
    }

    public interface IAlarmDisplayCallback
    {
        [OperationContract(IsOneWay = true)]
        void AlarmHappened(AlarmTime alarmTime);
    }
}
