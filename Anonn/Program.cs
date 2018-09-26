using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Collections.Specialized;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace Anonn
{
    public static class SHA
    {
        [DllImport("User32.dll")]
        static extern int SetForegroundWindow(IntPtr point);
        public static string GenerateSHA256String(string inputString)
        {
            SHA256 sha256 = SHA256Managed.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(inputString);
            byte[] hash = sha256.ComputeHash(bytes);
            return GetStringFromHash(hash);
        }
        private static string GetStringFromHash(byte[] hash)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                result.Append(hash[i].ToString("X2"));
            }
            return result.ToString();
        }

    }
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            string id;
            string chat = "";
            string username;
            int version = 0;
            int cversion = 4;
            bool ispublic = false;
            WebClient client3 = new WebClient();
            version = Int32.Parse(client3.DownloadString("https://anonn.cf/version.txt"));
            if (version > cversion)
            {
                Console.WriteLine("A new update has been released. You can download it at https://anonn.cf\n");
            }
            for (; ; )
            {
                Console.Write("Enter session ID\nType 'new' to create a new session\nOr type 'public' to view all public sessions: ");
                id = Console.ReadLine();
                Console.WriteLine("");
                string p;
                if (id == "new")
                {
                    for (; ; )
                    {
                        Console.Write("Would you like to create a public or private session? (enter 'p' or 'pr'): ");
                        p = Console.ReadLine();
                        if (p == "p")
                        {
                            break;
                        }
                        else if (p == "pr")
                        {
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Invalid choice.\n");
                        }
                    }
                    if (p == "pr")
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
                    else if (p == "p")
                    {
                        string name;
                        for (; ; )
                        {
                            Console.WriteLine("Please create a session ID: ");
                            name = Console.ReadLine();
                            if (name.Contains(" "))
                            {
                                Console.WriteLine("You cannot have a space in the session ID.");
                            }
                            else
                            {
                                break;
                            }
                        }
                        Console.WriteLine("Please create a password for this session\n" +
                            "(this is used so that only people with the password can WIPE or PIN, make this secure.): ");
                        string token = "";
                        do
                        {
                            ConsoleKeyInfo key = Console.ReadKey(true);
                            // Backspace Should Not Work
                            if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                            {
                                token += key.KeyChar;
                                Console.Write("*");
                            }
                            else
                            {
                                if (key.Key == ConsoleKey.Backspace && token.Length > 0)
                                {
                                    token = token.Substring(0, (token.Length - 1));
                                    Console.Write("\b \b");
                                }
                                else if (key.Key == ConsoleKey.Enter)
                                {
                                    break;
                                }
                            }
                        } while (true);
                        token = (SHA.GenerateSHA256String(token));
                        token = token.ToLower();
                        string urlAddress = "https://anonn.cf/sessions/public/list/create.php";
                        using (WebClient client = new WebClient())
                        {
                            var postData = new NameValueCollection()
                            {
                                ["name"] = name,
                                ["token"] = Settings.token,
                                ["wipepass"] = token
                            };
                            string pagesource = Encoding.UTF8.GetString(client.UploadValues(urlAddress, postData));
                        }
                        Console.WriteLine("\nSession Created.\n");
                    }
                }
                else if (id == "public")
                {
                    string publicDir = "https://anonn.cf/sessions/public/list.php";
                    WebClient client = new WebClient();
                    string content = client.DownloadString(publicDir);
                    Console.WriteLine("Current public sessions: \n" + content);
                    Console.WriteLine("Enter the session name you would like to enter: ");
                    id = Console.ReadLine();
                    id = "public/list/" + id;
                    Console.Write("Create username: ");
                    username = Console.ReadLine();
                    Console.WriteLine("\nYou can type PINS to view all pins," +
                        "\nPIN to pin a message (only as owner)" +
                        "\nAnd WIPE to delete the server (only as owner)\n" +
                        "Press enter to enter the session.");
                    Console.ReadLine();
                    ispublic = true;
                    break;
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
            string wipepass = "";
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
                    Console.Clear();
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
                    if (message.Contains("WIPE") && ispublic == true)
                    {
                        Console.WriteLine("Enter the admin password: ");
                        wipepass = "";
                        do
                        {
                            ConsoleKeyInfo key = Console.ReadKey(true);
                            // Backspace Should Not Work
                            if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                            {
                                wipepass += key.KeyChar;
                                Console.Write("*");
                            }
                            else
                            {
                                if (key.Key == ConsoleKey.Backspace && wipepass.Length > 0)
                                {
                                    wipepass = wipepass.Substring(0, (wipepass.Length - 1));
                                    Console.Write("\b \b");
                                }
                                else if (key.Key == ConsoleKey.Enter)
                                {
                                    break;
                                }
                            }
                        } while (true);
                    }
                    if (message.Contains("PIN") && message != username + ": PINS" && ispublic == true)
                    {
                        Console.WriteLine("Enter the admin password: ");
                        wipepass = "";
                        do
                        {
                            ConsoleKeyInfo key = Console.ReadKey(true);
                            // Backspace Should Not Work
                            if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                            {
                                wipepass += key.KeyChar;
                                Console.Write("*");
                            }
                            else
                            {
                                if (key.Key == ConsoleKey.Backspace && wipepass.Length > 0)
                                {
                                    wipepass = wipepass.Substring(0, (wipepass.Length - 1));
                                    Console.Write("\b \b");
                                }
                                else if (key.Key == ConsoleKey.Enter)
                                {
                                    break;
                                }
                            }
                        } while (true);
                        string urlAddress2 = "https://anonn.cf/sessions/" + id + "/pin.php";
                        using (WebClient client = new WebClient())
                        {
                            var postData = new NameValueCollection()
                            {
                                ["message"] = message,
                                ["wipepass"] = wipepass,
                                ["token"] = Settings.token
                            };
                            string pagesource = Encoding.UTF8.GetString(client.UploadValues(urlAddress2, postData));
                        }
                    }
                    if (message.Contains("PINS"))
                    {
                        string pins = client2.DownloadString("https://anonn.cf/sessions/" + id + "/pins.txt");
                        pins = pins.Replace("PIN", "");
                        for (; ; )
                        {
                            Console.WriteLine("All pins (press enter to return): \n" + pins);
                            Console.ReadLine();
                            break;
                        }
                    }
                    string urlAddress = "https://anonn.cf/sessions/" + id + "/edit.php";
                    using (WebClient client = new WebClient())
                    {
                        var postData = new NameValueCollection()
                        {
                            ["message"] = message,
                            ["wipepass"] = wipepass,
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
                        Console.Clear();
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
