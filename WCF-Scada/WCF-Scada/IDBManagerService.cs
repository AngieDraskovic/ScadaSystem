using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using WCF_Scada.models;

namespace WCF_Scada
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IDBManagerService" in both code and config file together.
    [ServiceContract]
    public interface IDBManagerService
    {
        [OperationContract]
        bool RegisterUser(string username, string password);
        [OperationContract]
        string LogIn(string username, string password);

        [OperationContract]
        bool AddTag(Tag tag, string token);
    }
}
