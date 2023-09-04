using SCADACore.Database.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace SCADACore.Interfaces.ReportManagerInterface
{
    [ServiceContract]
    public interface IReportManager
    {
        [OperationContract]
        List<AlarmTime> GetAllAlarmsForTime(DateTime start, DateTime end, string sortType, bool asc);

        [OperationContract]
        List<AlarmTime> GetAllAlarmsForPriority(int priority, bool asc);

        [OperationContract]
        List<TagValue> GetAllTagValuesForTime(DateTime start, DateTime end, bool asc);

        [OperationContract]
        List<TagValue> GetAllTagValuesForType(string type, bool asc);

        [OperationContract]
        List<TagValue> GetAllTagValuesForName(string tagName, bool asc);
    }
}
