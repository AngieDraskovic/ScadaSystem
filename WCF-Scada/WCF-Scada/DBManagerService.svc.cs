using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using WCF_Scada.models;
using WCF_Scada.Processing;

namespace WCF_Scada
{
    
    public class DBManagerService : IDBManagerService
    {
        public bool RegisterUser(string username, string password)
        {
           
          return UserProcessing.RegisterUser(username, password);
         
        }

        public string LogIn(string username, string password)
        {

            return UserProcessing.LogIn(username, password);

        }

        public bool AddTag(Tag tag, string token) {
            return false;
        }
    }
}
