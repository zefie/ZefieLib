using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace ZefieLib
{
    public class Process
    {
        public static IntPtr GetProcessByTitle(String title)
        {
            IntPtr hWnd = IntPtr.Zero;
            foreach (System.Diagnostics.Process pList in System.Diagnostics.Process.GetProcesses())
            {
                if (pList.MainWindowTitle.Contains(title))
                {
                    hWnd = pList.MainWindowHandle;
                    break;
                }
            }
            return hWnd;
        }
        public static IntPtr GetProcessByName(String name)
        {
            IntPtr hWnd = IntPtr.Zero;
            foreach (System.Diagnostics.Process pList in System.Diagnostics.Process.GetProcesses())
            {
                if (pList.ProcessName.Contains(name))
                {
                    hWnd = pList.MainWindowHandle;
                    break;
                }
            }
            return hWnd;
        }
        public static System.Diagnostics.Process? LaunchApp(string app, string? args = null)
        {
            return System.Diagnostics.Process.Start(new ProcessStartInfo(app, args ?? "")
            {
                WorkingDirectory = System.IO.Path.GetDirectoryName(app)
            });
        }
        public static System.Diagnostics.Process? LaunchApp(System.Uri uri, string? workdir = null)
        {
            return System.Diagnostics.Process.Start(new ProcessStartInfo("explorer", uri.ToString())
            {
                WorkingDirectory = System.IO.Path.GetDirectoryName(workdir ?? Application.ExecutablePath)
            });
        }
        public static System.Diagnostics.Process? LaunchShell(string args, string? workdir = null)
        {
            return System.Diagnostics.Process.Start(new ProcessStartInfo("cmd.exe")
            {
                WorkingDirectory = System.IO.Path.GetDirectoryName(workdir ?? Application.ExecutablePath),
                Arguments = "/C " + args
            });
        }
    }
}
