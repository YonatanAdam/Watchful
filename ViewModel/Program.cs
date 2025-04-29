using Model;
using System;

namespace ViewModel
{
    partial class Program
    {
        static void Main(string[] args)
        {
            GroupDB groupDB = new GroupDB();
            UserDB userDB = new UserDB();

            // Replace with a valid group ID from your database
            int testGroupId = 7;

            UserList users = userDB.SelectUsersByGroupId(testGroupId);

            Console.WriteLine($"Users in Group ID {testGroupId}:");
            foreach (User user in users)
            {
                Console.WriteLine($"- {user.Name} (ID: {user.Id})");
            }

        }
    }
}