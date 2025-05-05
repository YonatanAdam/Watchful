using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Model;

namespace WCFWeBService
{
    [ServiceContract]
    public interface IWatchfulService
    {
        [OperationContract]
        UserList GetAllUsers();

        [OperationContract]
        User Login(string username, string password);

        [OperationContract]
        bool CreateGroup(string groupName, string passcode, int adminId);

        [OperationContract]
        bool UpdateGroup(int groupId, string groupName, string passcode, int adminId);

        [OperationContract]
        bool DeleteGroup(int groupId);

        [OperationContract]
        Group GetGroupById(int groupId);

        [OperationContract]
        UserList GetUsersInGroup(int groupId);

        [OperationContract]
        int SaveChanges();
    }
}
