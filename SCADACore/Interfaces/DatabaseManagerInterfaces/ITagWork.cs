using SCADACore.Tags;
using SCADACore.Tags.InputTags;
using SCADACore.TransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace SCADACore.DatabaseManagerInterfaces
{
    [ServiceContract]
    public interface ITagWork
    {
        [OperationContract]
        string AddTag(Tag tag, string token);

        [OperationContract]
        bool DeleteTag (string tagName, string type, string token);

        [OperationContract]
        List<Tag> GetTags(string token, string type);

        [OperationContract]
        bool ChangeTagScan(string token, string tagName, bool scanOn);

        [OperationContract]
        List<TagTransfer> GetOutputTagValues(string token);

        [OperationContract]
        bool ChangeOutputTagValue(string tagName, string type, string token, double newValue);

        [OperationContract]
        bool AddAlarm(string tagName, Alarm alarm, string token);

        [OperationContract]
        bool DeleteAlarm(string tagName, int alarmId, string token);
    }
}
