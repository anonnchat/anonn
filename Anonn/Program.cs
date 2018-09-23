using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Collections.Specialized;
using System.Windows.Forms;

namespace Anonn
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            string id;
            string chat = "";
            string username;
            int version = 0;
            int cversion = 1;
            WebClient client3 = new WebClient();
            version = Int32.Parse(client3.DownloadString("https://anonn.cf/version.txt"));
            if (version > cversion)
            {
                Console.WriteLine("A new update has been released. You can download it at https://anonn.cf\n");
            }
            for (; ; )
            {
                Console.Write("Enter session ID, or type 'new' to create a new session: ");
                id = Console.ReadLine();
                Console.WriteLine("");
                if (id == "new")
                {
                    int length = 15;
                    Random random = new Random();
                    string characters = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
                    StringBuilder result = new StringBuilder(length);
                    for (int i = 0; i < length; i++)
                    {
                        result.Append(characters[random.Next(characters.Length)]);
                    }
                    string urlAddress = "https://anonn.cf/sessions/create.php";
                    using (WebClient client = new WebClient())
                    {
                        var postData = new NameValueCollection()
                        {
                            ["name"] = result.ToString(),
                            ["token"] = Settings.token
                        };
                        string pagesource = Encoding.UTF8.GetString(client.UploadValues(urlAddress, postData));
                    }
                    Console.WriteLine("Your new session has been created. Your new session ID is: " + result + "\n" +
                        "This has been copied to your clipboard.\n");
                    Clipboard.SetText(result.ToString());
                }
                else
                {
                    Console.Write("Create username: ");
                    username = Console.ReadLine();
                    Console.WriteLine("\n\nPlease note, typing 'WIPE' in your message will delete the entire chat.\n" +
                        "Press enter to connect to this session.");
                    Console.ReadLine();
                    try
                    {
                        WebClient client = new WebClient();
                        chat = client.DownloadString("https://anonn.cf/sessions/" + id);
                        break;
                    }
                    catch
                    {
                        Console.WriteLine("Session does not exist. Please try again.\n");
                    }
                }
            }
            bool paused = false;
            string message = "";
            string chatNew = "";
            Console.Clear();
            Console.WriteLine(chat);
            Console.Write("Message: ");
            for (; ; )
            {
                WebClient client2 = new WebClient();
                try
                {
                    chatNew = client2.DownloadString("https://anonn.cf/sessions/" + id);
                }
                catch
                {
                    Console.WriteLine("Session has been wiped, or an unknown error has occured.\n" +
                        "Press enter to close.");
                    Console.ReadLine();
                    Environment.Exit(1);
                }
                if (Console.KeyAvailable)
                {
                    paused = true;
                }
                if (paused == true)
                {
                    message = Console.ReadLine();
                    message = username + ": " + message;
                    string urlAddress = "https://anonn.cf/sessions/" + id + "/edit.php";
                    using (WebClient client = new WebClient())
                    {
                        var postData = new NameValueCollection()
                        {
                            ["message"] = message,
                            ["token"] = Settings.token
                        };
                        string pagesource = Encoding.UTF8.GetString(client.UploadValues(urlAddress, postData));
                    }
                    paused = false;
                    try
                    {
                        chat = client2.DownloadString("https://anonn.cf/sessions/" + id);
                    }
                    catch
                    {
                        Console.WriteLine("Session has been wiped, or an unknown error has occured.\n" +
                            "Press enter to close.");
                        Console.ReadLine();
                        Environment.Exit(1);
                    }
                }
                if (chat != chatNew)
                {
                    Console.Clear();
                    Console.WriteLine(chatNew);
                    Console.Write("Message: ");
                    chat = chatNew;
                }
                Thread.Sleep(10);
            }
        }
    }
}
