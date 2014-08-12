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
            string password = ReadPassword();

            SyncClient syncClient = new SyncClient();
            syncClient.SignIn(email, password).Wait();
            //IEnumerable<Bookmark> bookmarks = syncClient.GetBookmarks().Result;
            //IEnumerable<Client> clients = syncClient.GetTabs().Result;
            IEnumerable<HistoryRecord> history = syncClient.GetHistory().Result;

            //Console.WriteLine("Bookmarks:");

            //foreach (Bookmark bookmark in bookmarks)
            //{
            //    Console.WriteLine("Title: " + bookmark.Title);
            //    Console.WriteLine("Uri: " + bookmark.Uri);
            //    Console.WriteLine("------");
            //}

            //Console.WriteLine("Tabs:");

            //foreach (Client client in clients)
            //{
            //    Console.WriteLine("Id: " + client.Id);
            //    Console.WriteLine("Name: " + client.ClientName);
            //    Console.WriteLine("Tabs:");

            //    foreach(Tab tab in client.Tabs)
            //    {
            //        Console.WriteLine("\tTitle: " + tab.Title);
            //        Console.WriteLine("\tLast Used: " + tab.LastUsed);
            //    }

            //    Console.WriteLine("------");
            //}

            Console.WriteLine("Press enter to exit...");
            Console.ReadLine();
        }

        private static string ReadPassword()
        {
            StringBuilder password = new StringBuilder();

            ConsoleKeyInfo info = Console.ReadKey(true);
            while (info.Key != ConsoleKey.Enter)
            {
                if (info.Key != ConsoleKey.Backspace)
                {
                    Console.Write("*");
                    password.Append(info.KeyChar);
                }
                else if (info.Key == ConsoleKey.Backspace)
                {
                    if (password.Length != 0)
                    {
                        // remove one character from the list of password characters
                        password = password.Remove(password.Length - 1, 1);

                        // get the location of the cursor
                        int pos = Console.CursorLeft;

                        // move the cursor to the left by one character
                        Console.SetCursorPosition(pos - 1, Console.CursorTop);

                        // replace it with space
                        Console.Write(" ");

                        // move the cursor to the left by one character again
                        Console.SetCursorPosition(pos - 1, Console.CursorTop);
                    }
                }

                info = Console.ReadKey(true);
            }

            // add a new line because user pressed enter at the end of their password
            Console.WriteLine();
            return password.ToString();
        }

    }
}
