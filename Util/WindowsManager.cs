using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static PInvoke.User32;

namespace xiaochao
{
    internal class WindowsManager
    {



        //-------------------------- 外部定义 ---------------------------------
        
        /// <summary>
        /// 获取window的标题
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="text"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern uint GetWindowModuleFileName(IntPtr hwnd, StringBuilder lpszFileName, uint cchFileNameMax);


        //------------------------- 静态方法 ----------------------

        /// <summary>
        /// 获取窗口的标题
        /// </summary>
        /// <returns></returns>
        public static string GetFrontWindowText()
        {
            IntPtr hWnd = GetForegroundWindow();
            return GetTitle(hWnd);
        }

        /// <summary>
        /// 获取窗口归属的exe
        /// </summary>
        /// <param name="hwnd"></param>
        /// <returns></returns>
        public static string GetWindowProcessName()
        {
            IntPtr hwnd = GetForegroundWindow();
            GetWindowThreadProcessId(hwnd, out int pid);
            Process p = Process.GetProcessById((int)pid);
            return p.ProcessName;
        }


        /// <summary>
        /// 将对应名称的窗口放置在前台
        /// </summary>
        /// <param name="windowName"></param>
        /// <returns></returns>
        public static bool Set_window_frontByName(string windowName)
        {
            return SetForegroundWindow(FindWindow(null, windowName));
        }


        public static string GetTitle(IntPtr handle)
        {
            string windowText = "";
            int nChars = GetWindowTextLength(handle);
            char[] Buff = new char[nChars];
            if (GetWindowText(handle, Buff, nChars+1) > 0)
            {
                windowText = new string(Buff);
            }
            return windowText;
        }
    }
}
