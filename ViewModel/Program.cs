using Model;
using System;

namespace ViewModel
{
    partial class Program
    {
        static void Main(string[] args)
        {
            var db = new GroupDB();
            var udb =new UserDB();

            var entities = db.Select("GroupTbl");
            User admin = udb.GetUserById(4);

            Group group = new Group
            {
                GroupName = "Shteiman",
                Admin = admin
            };

            db.Insert(group);

            foreach (var e in entities)
            {
                Console.WriteLine($"(ID: {e.Id}) -> {e.ToString()}");
            }

            int changes = BaseDB.SaveChanges();

            Console.WriteLine("End of LocationTbl");
            Console.WriteLine("Changes: " + changes);

        }
    }
}