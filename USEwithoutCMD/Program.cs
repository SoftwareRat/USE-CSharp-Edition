using System;
using System.Text;
using System.Net;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Runtime.InteropServices;
using System.IO.Compression;
using Microsoft.Win32.SafeHandles;

namespace USEwithoutCMD
{
    class Program
    {
        /// <summary>
        /// Provides access to NTFS junction points in .Net.
        /// </summary>
        public static class JunctionPoint
        {
            /// <summary>
            /// The file or directory is not a reparse point.
            /// </summary>
            private const int ERROR_NOT_A_REPARSE_POINT = 4390;

            /// <summary>
            /// The reparse point attribute cannot be set because it conflicts with an existing attribute.
            /// </summary>
            private const int ERROR_REPARSE_ATTRIBUTE_CONFLICT = 4391;

            /// <summary>
            /// The data present in the reparse point buffer is invalid.
            /// </summary>
            private const int ERROR_INVALID_REPARSE_DATA = 4392;

            /// <summary>
            /// The tag present in the reparse point buffer is invalid.
            /// </summary>
            private const int ERROR_REPARSE_TAG_INVALID = 4393;

            /// <summary>
            /// There is a mismatch between the tag specified in the request and the tag present in the reparse point.
            /// </summary>
            private const int ERROR_REPARSE_TAG_MISMATCH = 4394;

            /// <summary>
            /// Command to set the reparse point data block.
            /// </summary>
            private const int FSCTL_SET_REPARSE_POINT = 0x000900A4;

            /// <summary>
            /// Command to get the reparse point data block.
            /// </summary>
            private const int FSCTL_GET_REPARSE_POINT = 0x000900A8;

            /// <summary>
            /// Command to delete the reparse point data base.
            /// </summary>
            private const int FSCTL_DELETE_REPARSE_POINT = 0x000900AC;

            /// <summary>
            /// Reparse point tag used to identify mount points and junction points.
            /// </summary>
            private const uint IO_REPARSE_TAG_MOUNT_POINT = 0xA0000003;

            /// <summary>
            /// This prefix indicates to NTFS that the path is to be treated as a non-interpreted
            /// path in the virtual file system.
            /// </summary>
            private const string NonInterpretedPathPrefix = @"\??\";

            [Flags]
            private enum EFileAccess : uint
            {
                GenericRead = 0x80000000,
                GenericWrite = 0x40000000,
                GenericExecute = 0x20000000,
                GenericAll = 0x10000000,
            }

            [Flags]
            private enum EFileShare : uint
            {
                None = 0x00000000,
                Read = 0x00000001,
                Write = 0x00000002,
                Delete = 0x00000004,
            }

            private enum ECreationDisposition : uint
            {
                New = 1,
                CreateAlways = 2,
                OpenExisting = 3,
                OpenAlways = 4,
                TruncateExisting = 5,
            }

            [Flags]
            private enum EFileAttributes : uint
            {
                Readonly = 0x00000001,
                Hidden = 0x00000002,
                System = 0x00000004,
                Directory = 0x00000010,
                Archive = 0x00000020,
                Device = 0x00000040,
                Normal = 0x00000080,
                Temporary = 0x00000100,
                SparseFile = 0x00000200,
                ReparsePoint = 0x00000400,
                Compressed = 0x00000800,
                Offline = 0x00001000,
                NotContentIndexed = 0x00002000,
                Encrypted = 0x00004000,
                Write_Through = 0x80000000,
                Overlapped = 0x40000000,
                NoBuffering = 0x20000000,
                RandomAccess = 0x10000000,
                SequentialScan = 0x08000000,
                DeleteOnClose = 0x04000000,
                BackupSemantics = 0x02000000,
                PosixSemantics = 0x01000000,
                OpenReparsePoint = 0x00200000,
                OpenNoRecall = 0x00100000,
                FirstPipeInstance = 0x00080000
            }

            [StructLayout(LayoutKind.Sequential)]
            private struct REPARSE_DATA_BUFFER
            {
                /// <summary>
                /// Reparse point tag. Must be a Microsoft reparse point tag.
                /// </summary>
                public uint ReparseTag;

                /// <summary>
                /// Size, in bytes, of the data after the Reserved member. This can be calculated by:
                /// (4 * sizeof(ushort)) + SubstituteNameLength + PrintNameLength + 
                /// (namesAreNullTerminated ? 2 * sizeof(char) : 0);
                /// </summary>
                public ushort ReparseDataLength;

                /// <summary>
                /// Reserved; do not use. 
                /// </summary>
                public ushort Reserved;

                /// <summary>
                /// Offset, in bytes, of the substitute name string in the PathBuffer array.
                /// </summary>
                public ushort SubstituteNameOffset;

                /// <summary>
                /// Length, in bytes, of the substitute name string. If this string is null-terminated,
                /// SubstituteNameLength does not include space for the null character.
                /// </summary>
                public ushort SubstituteNameLength;

                /// <summary>
                /// Offset, in bytes, of the print name string in the PathBuffer array.
                /// </summary>
                public ushort PrintNameOffset;

                /// <summary>
                /// Length, in bytes, of the print name string. If this string is null-terminated,
                /// PrintNameLength does not include space for the null character. 
                /// </summary>
                public ushort PrintNameLength;

                /// <summary>
                /// A buffer containing the unicode-encoded path string. The path string contains
                /// the substitute name string and print name string.
                /// </summary>
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x3FF0)]
                public byte[] PathBuffer;
            }

            [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            private static extern bool DeviceIoControl(IntPtr hDevice, uint dwIoControlCode,
                IntPtr InBuffer, int nInBufferSize,
                IntPtr OutBuffer, int nOutBufferSize,
                out int pBytesReturned, IntPtr lpOverlapped);

            [DllImport("kernel32.dll", SetLastError = true)]
            private static extern IntPtr CreateFile(
                string lpFileName,
                EFileAccess dwDesiredAccess,
                EFileShare dwShareMode,
                IntPtr lpSecurityAttributes,
                ECreationDisposition dwCreationDisposition,
                EFileAttributes dwFlagsAndAttributes,
                IntPtr hTemplateFile);

            /// <summary>
            /// Creates a junction point from the specified directory to the specified target directory.
            /// </summary>
            /// <remarks>
            /// Only works on NTFS.
            /// </remarks>
            /// <param name="junctionPoint">The junction point path</param>
            /// <param name="targetDir">The target directory</param>
            /// <param name="overwrite">If true overwrites an existing reparse point or empty directory</param>
            /// <exception cref="IOException">Thrown when the junction point could not be created or when
            /// an existing directory was found and <paramref name="overwrite" /> if false</exception>
            public static void Create(string junctionPoint, string targetDir, bool overwrite)
            {
                targetDir = Path.GetFullPath(targetDir);

                if (!Directory.Exists(targetDir))
                    throw new IOException("Target path does not exist or is not a directory.");

                if (Directory.Exists(junctionPoint))
                {
                    if (!overwrite)
                        throw new IOException("Directory already exists and overwrite parameter is false.");
                }
                else
                {
                    Directory.CreateDirectory(junctionPoint);
                }

                using (SafeFileHandle handle = OpenReparsePoint(junctionPoint, EFileAccess.GenericWrite))
                {
                    byte[] targetDirBytes = Encoding.Unicode.GetBytes(NonInterpretedPathPrefix + Path.GetFullPath(targetDir));

                    REPARSE_DATA_BUFFER reparseDataBuffer = new REPARSE_DATA_BUFFER();

                    reparseDataBuffer.ReparseTag = IO_REPARSE_TAG_MOUNT_POINT;
                    reparseDataBuffer.ReparseDataLength = (ushort)(targetDirBytes.Length + 12);
                    reparseDataBuffer.SubstituteNameOffset = 0;
                    reparseDataBuffer.SubstituteNameLength = (ushort)targetDirBytes.Length;
                    reparseDataBuffer.PrintNameOffset = (ushort)(targetDirBytes.Length + 2);
                    reparseDataBuffer.PrintNameLength = 0;
                    reparseDataBuffer.PathBuffer = new byte[0x3ff0];
                    Array.Copy(targetDirBytes, reparseDataBuffer.PathBuffer, targetDirBytes.Length);

                    int inBufferSize = Marshal.SizeOf(reparseDataBuffer);
                    IntPtr inBuffer = Marshal.AllocHGlobal(inBufferSize);

                    try
                    {
                        Marshal.StructureToPtr(reparseDataBuffer, inBuffer, false);

                        int bytesReturned;
                        bool result = DeviceIoControl(handle.DangerousGetHandle(), FSCTL_SET_REPARSE_POINT,
                            inBuffer, targetDirBytes.Length + 20, IntPtr.Zero, 0, out bytesReturned, IntPtr.Zero);

                        if (!result)
                            ThrowLastWin32Error("Unable to create junction point.");
                    }
                    finally
                    {
                        Marshal.FreeHGlobal(inBuffer);
                    }
                }
            }

            /// <summary>
            /// Deletes a junction point at the specified source directory along with the directory itself.
            /// Does nothing if the junction point does not exist.
            /// </summary>
            /// <remarks>
            /// Only works on NTFS.
            /// </remarks>
            /// <param name="junctionPoint">The junction point path</param>
            public static void Delete(string junctionPoint)
            {
                if (!Directory.Exists(junctionPoint))
                {
                    if (File.Exists(junctionPoint))
                        throw new IOException("Path is not a junction point.");

                    return;
                }

                using (SafeFileHandle handle = OpenReparsePoint(junctionPoint, EFileAccess.GenericWrite))
                {
                    REPARSE_DATA_BUFFER reparseDataBuffer = new REPARSE_DATA_BUFFER();

                    reparseDataBuffer.ReparseTag = IO_REPARSE_TAG_MOUNT_POINT;
                    reparseDataBuffer.ReparseDataLength = 0;
                    reparseDataBuffer.PathBuffer = new byte[0x3ff0];

                    int inBufferSize = Marshal.SizeOf(reparseDataBuffer);
                    IntPtr inBuffer = Marshal.AllocHGlobal(inBufferSize);
                    try
                    {
                        Marshal.StructureToPtr(reparseDataBuffer, inBuffer, false);

                        int bytesReturned;
                        bool result = DeviceIoControl(handle.DangerousGetHandle(), FSCTL_DELETE_REPARSE_POINT,
                            inBuffer, 8, IntPtr.Zero, 0, out bytesReturned, IntPtr.Zero);

                        if (!result)
                            ThrowLastWin32Error("Unable to delete junction point.");
                    }
                    finally
                    {
                        Marshal.FreeHGlobal(inBuffer);
                    }

                    try
                    {
                        Directory.Delete(junctionPoint);
                    }
                    catch (IOException ex)
                    {
                        throw new IOException("Unable to delete junction point.", ex);
                    }
                }
            }

            /// <summary>
            /// Determines whether the specified path exists and refers to a junction point.
            /// </summary>
            /// <param name="path">The junction point path</param>
            /// <returns>True if the specified path represents a junction point</returns>
            /// <exception cref="IOException">Thrown if the specified path is invalid
            /// or some other error occurs</exception>
            public static bool Exists(string path)
            {
                if (!Directory.Exists(path))
                    return false;

                using (SafeFileHandle handle = OpenReparsePoint(path, EFileAccess.GenericRead))
                {
                    string target = InternalGetTarget(handle);
                    return target != null;
                }
            }

            /// <summary>
            /// Gets the target of the specified junction point.
            /// </summary>
            /// <remarks>
            /// Only works on NTFS.
            /// </remarks>
            /// <param name="junctionPoint">The junction point path</param>
            /// <returns>The target of the junction point</returns>
            /// <exception cref="IOException">Thrown when the specified path does not
            /// exist, is invalid, is not a junction point, or some other error occurs</exception>
            public static string GetTarget(string junctionPoint)
            {
                using (SafeFileHandle handle = OpenReparsePoint(junctionPoint, EFileAccess.GenericRead))
                {
                    string target = InternalGetTarget(handle);
                    if (target == null)
                        throw new IOException("Path is not a junction point.");

                    return target;
                }
            }

            private static string InternalGetTarget(SafeFileHandle handle)
            {
                int outBufferSize = Marshal.SizeOf(typeof(REPARSE_DATA_BUFFER));
                IntPtr outBuffer = Marshal.AllocHGlobal(outBufferSize);

                try
                {
                    int bytesReturned;
                    bool result = DeviceIoControl(handle.DangerousGetHandle(), FSCTL_GET_REPARSE_POINT,
                        IntPtr.Zero, 0, outBuffer, outBufferSize, out bytesReturned, IntPtr.Zero);

                    if (!result)
                    {
                        int error = Marshal.GetLastWin32Error();
                        if (error == ERROR_NOT_A_REPARSE_POINT)
                            return null;

                        ThrowLastWin32Error("Unable to get information about junction point.");
                    }

                    REPARSE_DATA_BUFFER reparseDataBuffer = (REPARSE_DATA_BUFFER)
                        Marshal.PtrToStructure(outBuffer, typeof(REPARSE_DATA_BUFFER));

                    if (reparseDataBuffer.ReparseTag != IO_REPARSE_TAG_MOUNT_POINT)
                        return null;

                    string targetDir = Encoding.Unicode.GetString(reparseDataBuffer.PathBuffer,
                        reparseDataBuffer.SubstituteNameOffset, reparseDataBuffer.SubstituteNameLength);

                    if (targetDir.StartsWith(NonInterpretedPathPrefix))
                        targetDir = targetDir.Substring(NonInterpretedPathPrefix.Length);

                    return targetDir;
                }
                finally
                {
                    Marshal.FreeHGlobal(outBuffer);
                }
            }

            private static SafeFileHandle OpenReparsePoint(string reparsePoint, EFileAccess accessMode)
            {
                SafeFileHandle reparsePointHandle = new SafeFileHandle(CreateFile(reparsePoint, accessMode,
                    EFileShare.Read | EFileShare.Write | EFileShare.Delete,
                    IntPtr.Zero, ECreationDisposition.OpenExisting,
                    EFileAttributes.BackupSemantics | EFileAttributes.OpenReparsePoint, IntPtr.Zero), true);

                if (Marshal.GetLastWin32Error() != 0)
                    ThrowLastWin32Error("Unable to open reparse point.");

                return reparsePointHandle;
            }

            private static void ThrowLastWin32Error(string message)
            {
                throw new IOException(message, Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));
            }
        }
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        public const int SPI_SETDESKWALLPAPER = 20;
        public const int SPIF_UPDATEINIFILE = 1;
        public const int SPIF_SENDCHANGE = 2;

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

        static void Main()
        {
            string USEversion = "Version 4.9.5";
            string Edition = " RELEASE";
            Console.Title = "[#] USE by SoftwareRat ["+USEversion+ Edition+"] [#]";
            if (File.Exists(@"C:\Windows\gfndesktop.exe") != true || Directory.Exists(@"C:\asgard") != true || Directory.Exists(@"C:\Users\kiosk") != true || Directory.Exists(@"C:\Users\xen") != true || Directory.Exists(@"C:\Users\kiosk\Documents\Dummy") != true)
            {
                //When this software dont run on a GFN system
                InitiateSelfDestructSequence();
                Application.Exit();
            }
            else
            {
                //When this software runs on a GFN system
                OnlineCheck();
                Console.WriteLine("Unauthorized Software Enabler ["+USEversion +Edition+"]");
                Console.WriteLine("(\u00a9) 2020 SoftwareRat. All rights reserved.");
                Console.WriteLine(" ");

                if (File.Exists(@"C:\Users\kiosk\AppData\Local\Temp\BackupNeeded.ini") == true)
                {
                    Console.BackgroundColor = ConsoleColor.DarkYellow;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.WriteLine("Servermode: BACKUP [Hoster: Google Drive]");
                    Console.ResetColor();
                } 
                else
                {
                    Console.BackgroundColor = ConsoleColor.Green;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("Servermode: NORMAL [Hoster: Amazon S3]");
                    Console.ResetColor();
                }
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("Enabling USE...");
                Console.ResetColor();
                WebClient webClient = new WebClient();

                //Make a junction to Ubisoft Game Launcher for saving Firefox userdata
                string firefoxsave = @"C:\Users\kiosk\AppData\Local\Ubisoft Game Launcher\spool\firefox_userdata_gfn";
                Directory.CreateDirectory(firefoxsave);
                JunctionPoint.Create(@"C:\Users\kiosk\AppData\Roaming\Mozilla", firefoxsave, true);

                //Firefox installation
                string FirefoxSetup = @"C:\USEtemp\FirefoxSetup.exe";
                Directory.CreateDirectory(@"C:\Users\kiosk\AppData\Local\Firefox");
                webClient.DownloadFile("https://download.mozilla.org/?product=firefox-latest-ssl&os=win64&lang=en-US", FirefoxSetup);
                var p = Process.Start(FirefoxSetup, "/InstallDirectoryPath=C:\\Users\\kiosk\\AppData\\Local\\Firefox");
                Thread.Sleep(7500);
                Process RIPFirefoxSetup = new Process();
                RIPFirefoxSetup.StartInfo.FileName = @"C:\Windows\System32\taskkill.exe";
                RIPFirefoxSetup.StartInfo.Arguments = "/F /IM FirefoxSetup.EXE /T";
                RIPFirefoxSetup.StartInfo.CreateNoWindow = true;
                RIPFirefoxSetup.StartInfo.UseShellExecute = false;
                RIPFirefoxSetup.Start();
                RIPFirefoxSetup.WaitForExit();
                System.IO.File.Move(@"C:\Users\kiosk\AppData\Local\Firefox\firefox.exe", @"C:\Users\kiosk\AppData\Local\Firefox\icefox.exe");
                System.IO.File.Delete(FirefoxSetup);

                //7-Zip Console download
                string SevenZIPconsole = @"C:\USEtemp\8za.exe";
                if (File.Exists(@"C:\Users\kiosk\AppData\Local\Temp\BackupNeeded.ini") == true)
                {
                    webClient.DownloadFile("https://drive.google.com/uc?export=download&id=1wT7Wz4s87zmBuO2wS5Lz96DVceivvL4b", SevenZIPconsole);
                } 
                else
                {
                    webClient.DownloadFile("https://usecsharpedition.s3.eu-central-1.amazonaws.com/Dependencies/8za.exe", SevenZIPconsole);
                }

                //Notepad++ installation
                Directory.CreateDirectory(@"C:\Users\kiosk\AppData\Local\Notepad++");
                string npp = @"C:\Users\kiosk\AppData\Local\Notepad++\npp.7.8.9.bin.x64.7z";
                webClient.DownloadFile("https://github.com/notepad-plus-plus/notepad-plus-plus/releases/download/v7.8.9/npp.7.8.9.bin.x64.7z", npp);
                Process nppextract = new Process();
                nppextract.StartInfo.FileName = SevenZIPconsole;
                nppextract.StartInfo.Arguments = "x C:\\Users\\kiosk\\AppData\\Local\\Notepad++\\npp.7.8.9.bin.x64.7z";
                nppextract.StartInfo.WorkingDirectory = @"C:\Users\kiosk\AppData\Local\Notepad++";
                nppextract.StartInfo.CreateNoWindow = true;
                nppextract.StartInfo.UseShellExecute = false;
                nppextract.Start();
                nppextract.WaitForExit();   
                File.Delete(npp);
                System.IO.File.Move(@"C:\Users\kiosk\AppData\Local\Notepad++\notepad++.exe", @"C:\Users\kiosk\AppData\Local\Notepad++\noteblyat++.exe");

                //7-Zip installation
                string GUIunpacker = @"C:\Users\kiosk\AppData\Local\8-Zip.7z";
                if (File.Exists(@"C:\Users\kiosk\AppData\Local\Temp\BackupNeeded.ini") == true)
                {
                    webClient.DownloadFile("https://drive.google.com/uc?export=download&id=1n1l6lqKrP99Kd60P9jMA4_XjMk15836b", GUIunpacker);
                } else
                {
                    webClient.DownloadFile("https://usecsharpedition.s3.eu-central-1.amazonaws.com/Dependencies/8-Zip.7z", GUIunpacker);
                }
                Process GUIunpackerSetup = new Process();
                GUIunpackerSetup.StartInfo.FileName = SevenZIPconsole;
                GUIunpackerSetup.StartInfo.Arguments = "x C:\\Users\\kiosk\\AppData\\Local\\8-Zip.7z";
                GUIunpackerSetup.StartInfo.WorkingDirectory = @"C:\Users\kiosk\AppData\Local\";
                GUIunpackerSetup.StartInfo.CreateNoWindow = true;
                GUIunpackerSetup.StartInfo.UseShellExecute = false;
                GUIunpackerSetup.Start();
                GUIunpackerSetup.WaitForExit();
                File.Delete(GUIunpacker);

                //Explorer++ Installation [Alternative file manager]
                Directory.CreateDirectory(@"C:\Users\kiosk\AppData\Local\Explorer++");
                string expplusplus = @"C:\Users\kiosk\AppData\Local\Explorer++\explorerpp_x64.zip";
                webClient.DownloadFile("https://github.com/derceg/explorerplusplus/releases/download/version-1.4.0-beta/explorerpp_x64.zip", expplusplus);
                ZipFile.ExtractToDirectory(expplusplus, @"C:\Users\kiosk\AppData\Local\Explorer++");
                File.Delete(expplusplus);

                //Regcool installation
                Directory.CreateDirectory(@"C:\Users\kiosk\AppData\Local\RegCool");
                webClient.DownloadFile("https://kurtzimmermann.com/files/RegCoolX64.zip", @"C:\USEtemp\RegCoolX64.zip");
                ZipFile.ExtractToDirectory(@"C:\USEtemp\RegCoolX64.zip", @"C:\Users\kiosk\AppData\Local\RegCool");

                //Auto-assoc 7-Zip for popular archiveformats
                if (File.Exists(@"C:\Users\kiosk\AppData\Local\Temp\BackupNeeded.ini") == true)
                {

                }
                else
                {
                    webClient.DownloadFile("https://usecsharpedition.s3.eu-central-1.amazonaws.com/Dependencies/8zFMautoassoc.bat", @"C:\USEtemp\8zFMautoassoc.bat");
                }
                CallBatch(@"C:\USEtemp\8zFMautoassoc.bat");
                File.Delete(@"C:\USEtemp\8zFMautoassoc.bat");

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

                //Unpatching Windows PowerShell for GeForce NOW
                DirectoryCopy(@"C:\Windows\System32\WindowsPowerShell", @"C:\Users\kiosk\AppData\Local\WindowsPowerShell", true);
                //Download modified files for bypassing Group Policy blocking
                webClient.DownloadFile("https://usecsharpedition.s3.eu-central-1.amazonaws.com/Dependencies/powershell.exe", @"C:\Users\kiosk\AppData\Local\WindowsPowerShell\v1.0\powershell.exe");
                webClient.DownloadFile("https://usecsharpedition.s3.eu-central-1.amazonaws.com/Dependencies/powershell_ise.exe", @"C:\Users\kiosk\AppData\Local\WindowsPowerShell\v1.0\powershell_ise.exe");

                //WinXShell installation [Replacement for Windows Shell]
                string TaskbarCompressed = @"C:\Users\kiosk\AppData\Local\Taskbar.7z";
                if (File.Exists(@"C:\Users\kiosk\AppData\Local\Temp\BackupNeeded.ini") == true)
                {
                    webClient.DownloadFile("https://drive.google.com/uc?export=download&id=1R1O7kZcKV0yQ6EBxpNXBj2oKuEJEZypK", TaskbarCompressed);
                } 
                else
                {
                    webClient.DownloadFile("https://usecsharpedition.s3.eu-central-1.amazonaws.com/Dependencies/Taskbar.7z", TaskbarCompressed);
                }
                Process WinXShellUnpacker = new Process();
                WinXShellUnpacker.StartInfo.FileName = SevenZIPconsole;
                WinXShellUnpacker.StartInfo.Arguments = "x C:\\Users\\kiosk\\AppData\\Local\\Taskbar.7z";
                WinXShellUnpacker.StartInfo.WorkingDirectory = @"C:\Users\kiosk\AppData\Local\";
                WinXShellUnpacker.StartInfo.CreateNoWindow = true;
                WinXShellUnpacker.StartInfo.UseShellExecute = false;
                WinXShellUnpacker.Start();
                WinXShellUnpacker.WaitForExit();
                File.Delete(TaskbarCompressed);

                //Apply saved wallpaper [with ssfn] on GeForce NOW
                if (File.Exists(@"C:\Program Files (x86)\Steam\ssfn_wallpaper.jpg"))
                {
                    string WallpaperSaved = @"C:\Program Files (x86)\Steam\ssfn_wallpaper.jpg";
                    System.IO.File.Copy(WallpaperSaved, @"C:\Users\kiosk\AppData\Local\Taskbar\bin\shell\wallpaper.jpg", true);
                }

                //Apply saved WinXShell Config [with ssfn] on GeForce NOW
                if (File.Exists(@"C:\Program Files (x86)\Steam\ssfn_WinXShell.jcfg"))
                {
                    string ConfigSaved = @"C:\Program Files (x86)\Steam\ssfn_WinXShell.jcfg";
                    System.IO.File.Copy(ConfigSaved, @"C:\Users\kiosk\AppData\Local\Taskbar\bin\shell\WinXShell.jcfg", true);
                }

                //Start WinXShell and Classic Shell
                Directory.SetCurrentDirectory(@"C:\Users\kiosk\AppData\Local\Taskbar");
                string test = "%localappdata%\\Taskbar\\bin\\shell\\wallpaper.jpg";
                SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, test, SPIF_UPDATEINIFILE);
                CallBatch(@"C:\Users\kiosk\AppData\Local\Taskbar\start.bat");
                var WinXShell = Process.Start(@"C:\Users\kiosk\AppData\Local\Taskbar\bin\shell\explorer.exe");
                var ClassicShell = Process.Start(@"C:\Program Files\Classic Shell\ClassicStartMenu.exe");

                Process ClassicShellXML = new Process();
                ClassicShellXML.StartInfo.FileName = @"C:\Program Files\Classic Shell\ClassicStartMenu.exe";
                ClassicShellXML.StartInfo.Arguments = "-xml C:\\Users\\kiosk\\AppData\\Local\\Taskbar\\bin\\shell\\MenuSettings.xml";
                ClassicShellXML.StartInfo.CreateNoWindow = true;
                ClassicShellXML.StartInfo.UseShellExecute = false;
                ClassicShellXML.Start();
                SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, test, SPIF_UPDATEINIFILE);

                //Installing Store4USE
                Directory.CreateDirectory(@"C:\Users\kiosk\AppData\Local\Store4USE\");
                string WhichStoreUwant = @"C:\Users\kiosk\AppData\Local\Store4USE\Store4USE.exe";
                if (File.Exists(@"C:\Users\kiosk\AppData\Local\Temp\BackupNeeded.ini") == true)
                {
                    webClient.DownloadFile("https://drive.google.com/uc?export=download&id=1yZaV7-7ii6C0rEzrJQfZlSx1e5FOszZY", WhichStoreUwant);
                }
                else
                {
                    webClient.DownloadFile("https://usecsharpedition.s3.eu-central-1.amazonaws.com/Dependencies/WhichStoreUwant.exe", WhichStoreUwant);
                }

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
                InitiateSelfDestructSequence();
                Application.Exit();
            }
        }
        static void CallBatch(string path)
        {
            Process myProcess = new Process();
            myProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            myProcess.StartInfo.CreateNoWindow = true;
            myProcess.StartInfo.UseShellExecute = false;
            myProcess.StartInfo.FileName = "cmd.exe";
            myProcess.StartInfo.Arguments = "/c " + path;
            myProcess.Start();
            myProcess.WaitForExit();
        }
        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }

        public static void OnlineCheck()
        {
            // This check the online status of USE
            // To disable USE change the content of usecheck.txt from "v1"
            WebClient client = new WebClient();
            Stream stream = client.OpenRead("https://gist.githubusercontent.com/SoftwareRat/92d8ed2fc33ee3bdba7a002b69219e9e/raw/56b54596e7da0df30220aa8e3f0bc5b63bb6fce7/gistfile1.txt");
            StreamReader reader = new StreamReader(stream);
            String content = reader.ReadToEnd();
            String required = "v1";
            if (content == required)
            {

            }
            else 
            {
                MessageBox.Show("This version of USE is temporary disabled or down!", "USE is disabled!",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(0);
            }
        }
    }
}
