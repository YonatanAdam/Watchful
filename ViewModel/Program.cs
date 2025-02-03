using Model;
using System;

namespace ViewModel
{
    partial class Program
    {
        static void Main(string[] args)
        {
            var userDb = new UserDB();

            User newUser = new User
            {
                Name = "segev2",
                Password = "dahan"
            };
            
            userDb.Insert(newUser);
            int recordsAffected = BaseDB.SaveChanges();
            
            var entities = userDb.Select("UserTbl");
            
            
            foreach (var e in entities)
            {
                Console.WriteLine($"(ID: {e.Id}) -> {e.ToString()}");
                // userDb.Delete(e);
            }
            
            Console.WriteLine($"{recordsAffected} record(s) inserted.");
            Console.WriteLine("End of UserTbl");

        }
    }
}