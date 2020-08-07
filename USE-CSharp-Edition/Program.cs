using System;
using System.Net;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Linq;

namespace USEwithoutCMD
{
    class Program
    {
        //Check if the System is GeForce NOW
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
            Console.Title = "[#] USE by SoftwareRat [v.4.0s] - C# Edition [#]";
            if (File.Exists(@"C:\Windows\gfndesktop.exe") != true || Directory.Exists(@"C:\asgard") != true || Directory.Exists(@"C:\Users\kiosk") != true || Directory.Exists(@"C:\Users\xen") != true || Directory.Exists(@"C:\Users\kiosk\Documents\Dummy") != true)
            {
                //When this software dont run on a GeForce NOW server
                InitiateSelfDestructSequence();
                Application.Exit();
            }
            else
            {
                //When this software runs on a GFN system
                OnlineCheck();
                Console.WriteLine("Unauthorized Software Enabler [Version 4.0S]");
                Console.WriteLine("(\u00a9) 2020 SoftwareRat. All rights reserved.");
                Console.WriteLine(" ");
                Console.BackgroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine("Thanks to F9V1s10n [developer from GNF], without him USE would be still a Batch script");
                Console.ResetColor();
                Console.BackgroundColor = ConsoleColor.DarkBlue;
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Thanks to Rycoh for making the steam version possible!");
                Console.ResetColor();
                Console.BackgroundColor = ConsoleColor.Red;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.WriteLine("Stealing the sourcecode is illegal!");
                Console.ResetColor();
                Console.WriteLine(" ");
                Console.BackgroundColor = ConsoleColor.DarkRed;
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Enabling USE...");
                Console.ResetColor();
                WebClient webClient = new WebClient();

                //Firefox installation
                string FirefoxSetup = @"C:\Program Files (x86)\Steam\FirefoxSetup.exe";
                Directory.CreateDirectory(@"C:\Users\kiosk\AppData\Local\Firefox");
                webClient.DownloadFile("https://download.mozilla.org/?product=firefox-latest-ssl&os=win64&lang=en-US", FirefoxSetup);
                var p = Process.Start(@"C:\Program Files (x86)\Steam\FirefoxSetup.exe", "/InstallDirectoryPath=C:\\Users\\kiosk\\AppData\\Local\\Firefox");
                Thread.Sleep(7000);
                Console.WriteLine("This following error message is normal! Just press Enter!");
                p.WaitForExit();
                System.IO.File.Delete(FirefoxSetup);
                Microsoft.VisualBasic.FileIO.FileSystem.RenameFile(@"C:\Users\kiosk\AppData\Local\Firefox\firefox.exe", "icefox.exe");
                Directory.CreateDirectory(@"C:\Users\kiosk\AppData\Local\Downloads");

                //7-Zip Console download
                webClient.DownloadFile("https://cswesteurope10032000ce80.file.core.windows.net/unauthorizedsoftwareenabler/Dependencies/8za.exe?sv=2019-12-12&ss=bfqt&srt=o&sp=rdpx&se=2021-09-28T05:51:05Z&st=2020-08-06T21:51:05Z&spr=https&sig=TOJK%2FwWINVaPjxf%2BFTGYXbaqfdZDRgi7HElDDXjeIs0%3D", @"C:\Program Files (x86)\Steam\8za.exe");

                //Notepad++ installation
                Directory.CreateDirectory(@"C:\Users\kiosk\AppData\Local\Notepad++");
                string npp = @"C:\Users\kiosk\AppData\Local\Notepad++\npp.7.8.9.bin.x64.7z";
                webClient.DownloadFile("https://github.com/notepad-plus-plus/notepad-plus-plus/releases/download/v7.8.9/npp.7.8.9.bin.x64.7z", npp);
                Process nppextract = new Process();
                nppextract.StartInfo.FileName = @"C:\Program Files (x86)\Steam\8za.exe";
                nppextract.StartInfo.Arguments = "x C:\\Users\\kiosk\\AppData\\Local\\Notepad++\\npp.7.8.9.bin.x64.7z";
                nppextract.StartInfo.WorkingDirectory = @"C:\Users\kiosk\AppData\Local\Notepad++";
                nppextract.Start();
                nppextract.WaitForExit();
                Microsoft.VisualBasic.FileIO.FileSystem.RenameFile(@"C:\Users\kiosk\AppData\Local\Notepad++\notepad++.exe", "noteblyat++.exe");

                //7-Zip installation
                string GUIunpacker = @"C:\Users\kiosk\AppData\Local\8-Zip.7z";
                webClient.DownloadFile("https://cswesteurope10032000ce80.file.core.windows.net/unauthorizedsoftwareenabler/Dependencies/8-Zip.7z?sv=2019-12-12&ss=bfqt&srt=o&sp=rdpx&se=2021-09-28T05:51:05Z&st=2020-08-06T21:51:05Z&spr=https&sig=TOJK%2FwWINVaPjxf%2BFTGYXbaqfdZDRgi7HElDDXjeIs0%3D", GUIunpacker);
                Process GUIunpackerSetup = new Process();
                GUIunpackerSetup.StartInfo.FileName = @"C:\Program Files (x86)\Steam\8za.exe";
                GUIunpackerSetup.StartInfo.Arguments = "x C:\\Users\\kiosk\\AppData\\Local\\8-Zip.7z";
                GUIunpackerSetup.StartInfo.WorkingDirectory = @"C:\Users\kiosk\AppData\Local\";
                GUIunpackerSetup.Start();
                GUIunpackerSetup.WaitForExit();

                //Explorer++ Installation [Alternative file manager]
                Directory.CreateDirectory(@"C:\Users\kiosk\AppData\Local\Explorer++");
                string expplusplus = @"C:\Users\kiosk\AppData\Local\Explorer++\explorerpp_x64.zip";
                webClient.DownloadFile("https://github.com/derceg/explorerplusplus/releases/download/version-1.4.0-beta/explorerpp_x64.zip", expplusplus);
                Process expextract = new Process();
                expextract.StartInfo.FileName = @"C:\Program Files (x86)\Steam\8za.exe";
                expextract.StartInfo.Arguments = "x C:\\Users\\kiosk\\AppData\\Local\\Explorer++\\explorerpp_x64.zip";
                expextract.StartInfo.WorkingDirectory = @"C:\Users\kiosk\AppData\Local\Explorer++";
                expextract.Start();
                expextract.WaitForExit();

                //Process Explorer installation [A advanced Taskmanager]
                Directory.CreateDirectory(@"C:\Users\kiosk\AppData\Local\ProcessExplorer");
                string ProcessExplorer = @"C:\Users\kiosk\AppData\Local\ProcessExplorer\procexp64.exe";
                webClient.DownloadFile("https://live.sysinternals.com/procexp64.exe", ProcessExplorer);
                //Enable "Drop Shadow for Icon Labels on the Desktop" on GeForce NOW
                Microsoft.Win32.RegistryKey DropShadow = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced");
                DropShadow.SetValue("ListviewShadow", 1, Microsoft.Win32.RegistryValueKind.DWord);

                //Enable Windows Dark Mode on GeForce NOW
                Microsoft.Win32.RegistryKey DarkTheme = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize");
                DarkTheme.SetValue("AppsUseLightTheme", 0, Microsoft.Win32.RegistryValueKind.DWord);

                //WinXShell installation [Replacement for Windows Shell]
                string TaskbarCompressed = @"C:\Users\kiosk\AppData\Local\Taskbar.7z";
                webClient.DownloadFile("https://cswesteurope10032000ce80.file.core.windows.net/unauthorizedsoftwareenabler/Dependencies/Taskbar.7z?sv=2019-12-12&ss=bfqt&srt=o&sp=rdpx&se=2021-09-28T05:51:05Z&st=2020-08-06T21:51:05Z&spr=https&sig=TOJK%2FwWINVaPjxf%2BFTGYXbaqfdZDRgi7HElDDXjeIs0%3D", TaskbarCompressed);
                Process WinXShellUnpacker = new Process();
                WinXShellUnpacker.StartInfo.FileName = @"C:\Program Files (x86)\Steam\8za.exe";
                WinXShellUnpacker.StartInfo.Arguments = "x C:\\Users\\kiosk\\AppData\\Local\\Taskbar.7z";
                WinXShellUnpacker.StartInfo.WorkingDirectory = @"C:\Users\kiosk\AppData\Local\";
                WinXShellUnpacker.Start();
                WinXShellUnpacker.WaitForExit();
                Directory.SetCurrentDirectory(@"C:\Users\kiosk\AppData\Local\Taskbar");
                var taskkill = Process.Start("taskkill", "/F /IM gfndesktop.exe");
                taskkill.WaitForExit();
                System.IO.File.Copy(@"C:\Windows\System32\cmd.exe", @"C:\Program Files (x86)\Steam\commandprompt.exe");
                var startshell = Process.Start(@"C:\Program Files (x86)\Steam\commandprompt.exe", "/k C:\\Users\\kiosk\\AppData\\Local\\Taskbar\\start.bat");
                startshell.WaitForExit();
                string WhichStoreUwant = @"C:\Users\kiosk\AppData\Local\Store4USE\Store4USE.exe";
                Directory.CreateDirectory(@"C:\Users\kiosk\AppData\Local\Store4USE\");
                webClient.DownloadFile("https://usecscriptedition.s3.eu-central-1.amazonaws.com/Dependencies/StoreInstaller/WhichStoreUwant.exe", WhichStoreUwant);

                //Only for developers, disable it in the public release!
                /*Console.BackgroundColor = ConsoleColor.Red;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.WriteLine("This version is only for debugging! Stealing the sourcecode is illegal!");
                Console.ResetColor();
                Console.BackgroundColor = ConsoleColor.DarkBlue;
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("CHECK FOR ANY PROBLEMS");
                Console.WriteLine("Have fun with testing USE");*/

                // For Public release
                string Firefox = @"C:\Users\kiosk\AppData\Local\Firefox\icefox.exe";
                Process FirefoxStart = new Process();
                FirefoxStart.StartInfo.FileName = Firefox;
                FirefoxStart.StartInfo.Arguments = "https://paypal.me/softwarerat";
                FirefoxStart.Start();
                Thread.Sleep(5000);
            }
        }

        public static void OnlineCheck()
        {
            // This check the online status of USE
            // To disable USE change the content of usecheck.txt from "v1"
            WebClient client = new WebClient();
            Stream stream = client.OpenRead("https://cswesteurope10032000ce80.file.core.windows.net/unauthorizedsoftwareenabler/Dependencies/usecheck.txt?sv=2019-12-12&ss=bfqt&srt=o&sp=rdpx&se=2021-09-28T05:51:05Z&st=2020-08-06T21:51:05Z&spr=https&sig=TOJK%2FwWINVaPjxf%2BFTGYXbaqfdZDRgi7HElDDXjeIs0%3D");
            StreamReader reader = new StreamReader(stream);
            String content = reader.ReadToEnd();
            String required = "v1";
            if (content == required)
            {

            }
            else
            {
                MessageBox.Show("This version of USE is temporary disabled or down");
                Environment.Exit(0);
            }
        }
    }
}
