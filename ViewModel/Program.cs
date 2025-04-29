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

            foreach (var e in entities)
            {
                Console.WriteLine($"(ID: {e.Id}) -> {e.ToString()}");
                db.Delete(e);
            }

            int changes = BaseDB.SaveChanges();

            Console.WriteLine("Changes: " + changes);

        }
    }
}