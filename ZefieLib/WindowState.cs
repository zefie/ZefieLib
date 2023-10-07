using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using static System.Windows.Forms.AxHost;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ZefieLib
{
    internal class WindowState
    {
        public static IntPtr FindWindow(string titleName)
        {
            System.Diagnostics.Process[] pros = System.Diagnostics.Process.GetProcesses(".");
            foreach (System.Diagnostics.Process p in pros) if (p.MainWindowTitle.ToUpper().Contains(titleName.ToUpper())) return p.MainWindowHandle;
            return new IntPtr();
        }
        private const int SW_HIDE = 0;
        private const int SW_NORMAL = 1;
        private const int SW_MAXIMIZE = 3;
        private const int SW_SHOW = 4;
        private const int SW_MINIMIZE = 5;


        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        public static bool HideWindow(IntPtr hWnd)
        {
            return ShowWindow(hWnd, SW_HIDE);
        }
        public static bool NormalWindow(IntPtr hWnd)
        {
            return ShowWindow(hWnd, SW_NORMAL);
        }
        public static bool MaximizeWindow(IntPtr hWnd)
        {
            return ShowWindow(hWnd, SW_MAXIMIZE);
        }
        public static bool ShowWindow(IntPtr hWnd)
        {
            return ShowWindow(hWnd, SW_SHOW);
        }
        public static bool MinimizeWindow(IntPtr hWnd)
        {
            return ShowWindow(hWnd, SW_MINIMIZE);
        }
    }
}
