using System;
using System.IO;
using System.Diagnostics;
using System.Net;
using System.IO.Compression;
using System.Threading;
using System.Runtime.InteropServices;
using System.IO.Pipes;
using System.Text;
using System.Collections.Generic;
using Microsoft.Win32;

class JayydeeMultitool {
    static string currentVersion = "1.0.5"; 
    static string clientId = "1497756333095522335";
    static string sellauthUrl = "https://jayydee.mysellauth.com/";
    static string remoteDataUrl = "https://pastebin.com/raw/AsCLyiNw"; 

    static void Main() {
        ServicePointManager.Expect100Continue = true;
        ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;

        Console.Title = "Jayydee Multitool v" + currentVersion + " - Initializing...";
        
        // Dynamic Auto-Update Engine
        CheckForUpdates();

        Thread rpcThread = new Thread(RunRPC);
        rpcThread.IsBackground = true;
        rpcThread.Start();

        while (true) {
            DrawHeader();
            Console.WriteLine("\n  [1] Delete Roblox Ban Traces");
            Console.WriteLine("  [2] Download TMAC (Mac Spoofer)");
            Console.WriteLine("  [3] JayyDee's Windows Activation Tool");
            Console.WriteLine("  [4] Exit");
            Console.Write("\n  > Select Option: ");

            string choice = Console.ReadLine();
            if (choice == "1") RunPurge();
            else if (choice == "2") DownloadTMAC();
            else if (choice == "3") WindowsActivation();
            else if (choice == "4") Environment.Exit(0);
        }
    }

    static void CheckForUpdates() {
        try {
            using (WebClient wc = new WebClient()) {
                wc.Headers.Add("User-Agent", "Mozilla/5.0");
                string remoteData = wc.DownloadString(remoteDataUrl).Trim();
                
                string latestVersion = "";
                string downloadUrl = "";

                // Handle both | separator and New Lines
                if (remoteData.Contains("|")) {
                    string[] parts = remoteData.Split('|');
                    latestVersion = parts[0].Trim();
                    downloadUrl = parts[1].Trim();
                } else if (remoteData.Contains("\n")) {
                    string[] lines = remoteData.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                    latestVersion = lines[0].Trim();
                    downloadUrl = lines[1].Trim();
                }

                if (!string.IsNullOrEmpty(latestVersion) && latestVersion != currentVersion) {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("\n  [UPDATE] A new version (v" + latestVersion + ") is ready!");
                        Console.WriteLine("  [SYSTEM] Downloading from cloud, please wait...");
                        
                        string currentExe = Process.GetCurrentProcess().MainModule.FileName;
                        string newExe = currentExe + ".new";
                        
                        wc.DownloadFile(downloadUrl, newExe);
                        
                        string batchScript = "@echo off\n" +
                                             "timeout /t 2 /nobreak > nul\n" +
                                             "del \"" + currentExe + "\"\n" +
                                             "ren \"" + newExe + "\" \"" + Path.GetFileName(currentExe) + "\"\n" +
                                             "start \"\" \"" + currentExe + "\"\n" +
                                             "del \"%~f0\"";
                        
                        File.WriteAllText("updater.bat", batchScript);
                        Process.Start("updater.bat");
                        Environment.Exit(0);
                }
            }
        } catch { }
    }

    static void DrawHeader() {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(@"
  ####################################################################################################################
  #                                                                                                                  #
  #      JJJJJ   AAAAA  Y   Y  Y   Y  DDDD   EEEEE  EEEEE     AAAAA  IIIII                                           #
  #        J    A     A  Y Y    Y Y   D   D  E      E        A     A   I                                             #
  #        J    AAAAAAA   Y      Y    D   D  EEEE   EEEE     AAAAAAA   I                                             #
  #      J J    A     A   Y      Y    D   D  E      E        A     A   I                                             #
  #       JJ    A     A   Y      Y    DDDD   EEEEE  EEEEE    A     A  IIIII                                          #
  #                                                                                                                  #
  #                                     --- JAYYDEE MULTITOOL ---                                                    #
  #                                                                                                                  #
  ####################################################################################################################");
        Console.ResetColor();
    }

    static void RunPurge() {
        Console.WriteLine("\n  [SYSTEM] Initializing Purge Protocol...");
        KillProcess("RobloxPlayerBeta");
        string[] dirs = {
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Roblox"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Roblox"),
            Path.Combine(Path.GetTempPath(), "Roblox")
        };
        foreach (var d in dirs) DeleteDir(d);
        Console.WriteLine("  [STATUS] Scrubbing Registry (Native)...");
        try { Registry.CurrentUser.DeleteSubKeyTree(@"Software\Roblox", false); Console.WriteLine("  [+] Cleaned Registry: HKCU\\Software\\Roblox"); } catch { }
        try { Registry.CurrentUser.DeleteSubKeyTree(@"Software\Classes\roblox-player", false); Console.WriteLine("  [+] Cleaned Registry: Protocol Handlers"); } catch { }
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("\n  [FINISHED] ALL ROBLOX TRACES HAVE BEEN PURGED!");
        Console.ResetColor();
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine("\n  [#] Press any Key To download Roblox...");
        Console.ResetColor();
        Console.ReadKey();
        DownloadRoblox();
    }

    static void DownloadRoblox() {
        Console.WriteLine("  [SYSTEM] Roblox IS NOW DOWNLOADING...");
        try {
            using (WebClient wc = new WebClient()) {
                string path = Path.Combine(Path.GetTempPath(), "RobloxLauncher.exe");
                wc.DownloadFile("https://www.roblox.com/download/client?os=win", path);
                Process.Start(path);
                Console.WriteLine("  [+] Installation started.");
            }
        } catch { }
        Thread.Sleep(3000);
    }

    static void DownloadTMAC() {
        string tmacPath = @"C:\Program Files (x86)\Technitium\TMACv6.0\TMAC.exe";
        if (File.Exists(tmacPath)) {
            Console.WriteLine("\n  [INFO] TMAC Already Downloaded! Launching...");
            Process.Start(new ProcessStartInfo(tmacPath) { UseShellExecute = true, Verb = "runas" });
            Console.WriteLine("\n  [#] Press any key to return to menu...");
            Console.ReadKey();
            return;
        }

        Console.WriteLine("\n  [SYSTEM] Initiating TMAC Download...");
        KillProcess("TMAC"); // Close any running instances first
        string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        string folder = Path.Combine(desktop, "TMAC_Installer");
        if (Directory.Exists(folder)) try { Directory.Delete(folder, true); } catch { }
        Directory.CreateDirectory(folder);
        try {
            using (WebClient wc = new WebClient()) {
                string zip = Path.Combine(folder, "tmac.zip");
                wc.DownloadFile("https://download.technitium.com/tmac/TMACv6.0.7_Setup.zip", zip);
                ZipFile.ExtractToDirectory(zip, folder);
                File.Delete(zip);
                string[] files = Directory.GetFiles(folder, "*.exe");
                if (files.Length > 0) {
                    ProcessStartInfo psi = new ProcessStartInfo(files[0]);
                    psi.UseShellExecute = true;
                    psi.Verb = "runas"; // Force Admin
                    psi.WindowStyle = ProcessWindowStyle.Normal;
                    Process.Start(psi);
                    Console.WriteLine("  [+] TMAC Auto-Launched Successfully!");
                }
                Console.WriteLine("  [+] TMAC Files are in: " + folder);
            }
        } catch (Exception ex) {
            Console.WriteLine("  [ERROR] TMAC Failed: " + ex.Message);
        }
        Thread.Sleep(3000);
    }

    static void WindowsActivation() {
        Console.WriteLine("\n  [SYSTEM] Launching Activation Engine...");
        Process.Start(new ProcessStartInfo("powershell", "-Command \"irm https://get.activated.win | iex\"") { CreateNoWindow = false, UseShellExecute = true });
    }

    static void RunRPC() {
        while (true) {
            for (int i = 0; i < 5; i++) {
                try {
                    using (var pipe = new NamedPipeClientStream(".", "discord-ipc-" + i, PipeDirection.InOut)) {
                        pipe.Connect(500);
                        Send(pipe, 0, "{\"v\":1,\"client_id\":\"" + clientId + "\"}");
                        string json = "{\"cmd\":\"SET_ACTIVITY\",\"args\":{\"pid\":" + Process.GetCurrentProcess().Id + ",\"activity\":{\"details\":\"USING JayyDee's MULTITOOL\",\"assets\":{\"large_image\":\"logo\"},\"buttons\":[{\"label\":\"Get Tool\",\"url\":\"" + sellauthUrl + "\"}]}},\"nonce\":\"" + Guid.NewGuid() + "\"}";
                        Send(pipe, 1, json);
                        while (pipe.IsConnected) Thread.Sleep(15000);
                    }
                } catch { continue; }
            }
            Thread.Sleep(10000);
        }
    }

    static void Send(Stream s, int op, string json) {
        byte[] b = Encoding.UTF8.GetBytes(json);
        s.Write(BitConverter.GetBytes(op), 0, 4);
        s.Write(BitConverter.GetBytes(b.Length), 0, 4);
        s.Write(b, 0, b.Length);
        s.Flush();
    }

    static void KillProcess(string name) {
        foreach (var p in Process.GetProcessesByName(name)) try { p.Kill(); } catch { }
    }

    static void DeleteDir(string path) {
        if (Directory.Exists(path)) try { Directory.Delete(path, true); Console.WriteLine("  [+] Purged: " + path); } catch { }
    }
}
