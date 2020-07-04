using System;
using System.Net;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace USE_CSharp_Edition
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "USE by SoftwareRat [v.3.0] - C# Edition";
            if (File.Exists(@"C:\Windows\gfndesktop.exe") != true || Directory.Exists(@"C:\asgard") != true || Directory.Exists(@"C:\Users\kiosk") != true || Directory.Exists(@"C:\Users\xen") != true || Directory.Exists(@"C:\Users\kiosk\Documents\Dummy") != true)
            {
                MessageBox.Show("You don`t run this software on NVIDIA GeForce NOW!" + Environment.NewLine +
                    "For more information, contact SoftwareRat on Discord!", "Non GeForce NOW system detected! [Errorcode: 0x001]",
                     MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                Console.BackgroundColor = ConsoleColor.Green;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.WriteLine("NVIDIA GeForce NOW System detected!");
                Console.ReadKey();
                Console.ResetColor();
                Directory.SetCurrentDirectory(@"C:\Program Files (x86)\Steam");
                WebClient webClient = new WebClient();
                webClient.DownloadFile("https://download.mozilla.org/?product=firefox-latest-ssl&os=win64&lang=en-US", @"C:\Program Files (x86)\Steam\FirefoxSetup.exe");
                System.Diagnostics.Process.Start(@"C:\Program Files (x86)\Steam\FirefoxSetup.exe", "/DesktopShortcut=false" + "/InstallDirectoryPath=C:\\Users\\kiosk\\AppData\\Local\\Firefox");
                Directory.SetCurrentDirectory(@"C:\Users\kiosk\AppData\Local\Firefox");
                System.IO.File.Move("firefox.exe", "icefox.exe");
                WebClient Notepad = new WebClient();
                webClient.DownloadFile("https://github.com/notepad-plus-plus/notepad-plus-plus/releases/download/v7.8.8/npp.7.8.8.bin.x64.7z", @"C:\Users\kiosk\AppData\Local\Notepad++\npp.7.8.8.bin.x64.7z");
                System.Diagnostics.Process.Start(@"C:\Program Files (x86)\Steam\steamapps\common\Assassins Creed Origins\8za.exe", "x" + "C:\\Users\\kiosk\\AppData\\Local\\Notepad++\\npp.7.8.8.bin.x64.7z");
                Console.BackgroundColor = ConsoleColor.Yellow;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.WriteLine("Have fun with testing USE :)");
                Console.ReadKey();
            }
        }
    }
}
