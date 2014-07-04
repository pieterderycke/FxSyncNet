using FxSyncNet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FxSyncNet.DemoClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Please enter your Mozilla sync credentials.");
            Console.Write("Email: ");
            string email = Console.ReadLine();

            Console.Write("Password: ");
            string password = Console.ReadLine();

            SyncClient syncClient = new SyncClient();
            syncClient.SignIn(email, password).Wait();
            IEnumerable<Bookmark> bookmarks = syncClient.GetBookmarks().Result;
            //IEnumerable<Tab> tabs = syncClient.GetTabs().Result;

            Console.WriteLine("Bookmarks:");

            foreach (Bookmark bookmark in bookmarks)
            {
                Console.WriteLine("Title: " + bookmark.Title);
                Console.WriteLine("Uri: " + bookmark.Uri);
                Console.WriteLine("------");
            }

            Console.WriteLine("Press enter to exit...");
            Console.ReadLine();
        }
    }
}
