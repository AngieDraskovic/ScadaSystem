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
    [ServiceContract(SessionMode = SessionMode.Required)]
    public interface IDBManagerService
    {
        [OperationContract(IsInitiating = false)]
        bool RegisterUser(string username, string password);
        [OperationContract(IsInitiating = true)]
        string LogIn(string username, string password);

        [OperationContract(IsInitiating = false)]
        bool AddTag(Tag tag, string token);
        [OperationContract(IsInitiating = false)]
        bool RemoveTag(string tagName, string token);


        [OperationContract(IsInitiating = false, IsTerminating = true)]
        void LogOut(string token);

    }
}
