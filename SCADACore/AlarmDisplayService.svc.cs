using SCADACore.Interfaces.AlarmDisplayInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace SCADACore
{

    public class AlarmDisplayService : IAlarmDisplay
    {
        public void AlarmDisplayInit()
        {
            TagProcessing.AlarmDisplayInit();
        }
    }
}
