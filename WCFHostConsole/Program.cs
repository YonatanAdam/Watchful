using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using Model;
using ViewModel;

namespace WCFHostConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ServiceHost service = new ServiceHost(typeof(WCFWeBService.WatchfulService));

            Console.WriteLine("Starting Service...");

            foreach (var item in service.Description.Endpoints)
            {
                Console.WriteLine($"Endpoint: {item.Address}");
            }

            service.Open();
            Console.ReadLine();
        }
    }
}
