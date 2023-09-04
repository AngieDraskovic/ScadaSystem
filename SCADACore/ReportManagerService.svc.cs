using SCADACore.Database.Model;
using SCADACore.Interfaces.ReportManagerInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace SCADACore
{
    public class ReportManagerService : IReportManager
    {
        public List<AlarmTime> GetAllAlarmsForPriority(int priority, bool asc)
        {
            return TagProcessing.GetAllAlarmsForPriority(priority, asc);
        }

        public List<AlarmTime> GetAllAlarmsForTime(DateTime start, DateTime end, string sortType, bool asc)
        {
            return TagProcessing.GetAllAlarmsForTime(start, end, sortType, asc);
        }

        public List<TagValue> GetAllTagValuesForName(string tagName, bool asc)
        {
            return TagProcessing.GetAllTagValuesForName(tagName, asc);
        }

        public List<TagValue> GetAllTagValuesForTime(DateTime start, DateTime end, bool asc)
        {
            return TagProcessing.GetAllTagValuesForTime(start, end, asc);
        }

        public List<TagValue> GetAllTagValuesForType(string type, bool asc)
        {
            return TagProcessing.GetAllTagValuesForType(type, asc);
        }
    }
}
