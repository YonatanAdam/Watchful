using System;
using System.Linq;
using Model;
using ViewModel;

namespace WCFWeBService
{
    public class WatchfulService : IWatchfulService
    {
        private readonly UserDB _userDb = new UserDB();
        private readonly GroupDB _groupDb = new GroupDB();

        public UserList GetAllUsers()
        {
            try
            {
                return new UserList(_userDb.Select());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllUsers: {ex.Message}");
                return null;
            }
        }

        public User Login(string username, string password)
        {
            try
            {
                return _userDb.Login(username, password);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Login: {ex.Message}");
                return null;
            }
        }

        public bool CreateGroup(string groupName, string passcode, int adminId)
        {
            try
            {
                User admin = _userDb.GetUserById(adminId);
                if (admin == null) return false;

                Group group = _groupDb.CreateGroup(groupName, passcode, admin);
                return group != null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CreateGroup: {ex.Message}");
                return false;
            }
        }

        public bool UpdateGroup(int groupId, string groupName, string passcode, int adminId)
        {
            try
            {
                Group group = _groupDb.GetGroupById(groupId);
                if (group == null) return false;

                User admin = _userDb.GetUserById(adminId);
                if (admin == null) return false;

                group.GroupName = groupName;
                group.PassCode = passcode;
                group.Admin = admin;

                _groupDb.Update(group);
                return BaseDB.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdateGroup: {ex.Message}");
                return false;
            }
        }

        public bool DeleteGroup(int groupId)
        {
            try
            {
                Group group = _groupDb.GetGroupById(groupId);
                if (group == null) return false;

                _groupDb.Delete(group);
                return BaseDB.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DeleteGroup: {ex.Message}");
                return false;
            }
        }

        public Group GetGroupById(int groupId)
        {
            try
            {
                return _groupDb.GetGroupById(groupId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetGroupById: {ex.Message}");
                return null;
            }
        }

        public UserList GetUsersInGroup(int groupId)
        {
            try
            {
                return _groupDb.GetUsersByGroup(groupId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetUsersInGroup: {ex.Message}");
                return null;
            }
        }

        public int SaveChanges()
        {
            try
            {
                return BaseDB.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SaveChanges: {ex.Message}");
                return 0;
            }
        }
    }
}
