using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace SCADACore.DatabaseManagerInterfaces
{
    [ServiceContract]
    public interface IAuthentication
    {
        [OperationContract]
        bool RegisterUser(string username, string password, string token);

        [OperationContract]
        string Login(string username, string password);

        [OperationContract]
        void Logout(string token);

        [OperationContract]
        bool UsersEmpty();

        [OperationContract]
        bool IsUserAdmin(string token);
    }
}
