using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PInvoke.User32;
using static PInvoke.Kernel32;
using System.Runtime.InteropServices;
using System.Timers;

namespace xiaochao.Util
{
    public class WindowHooker
    {

        //存储用户设置的按键
        public VirtualKey _c_v_key { get; set; } = VirtualKey.VK_LCONTROL;

        //存储native的handle，用来取消hook
        private SafeHandle handle;

        //按键激发后的事件
        public delegate void KeyActivate();
        public KeyActivate Key_activate { get; set; }

        //交给native的委托
        public WindowsHookDelegate Keyboard_hook_delegate { get; set; }

        //计时器，用来触发快捷键的
        private static Timer aTimer;


        #region 构造函数
        public WindowHooker()
        {
            SetTimer();
            Keyboard_hook_delegate = this.KeyBoardHook;
            Hook();
        }

        #endregion

        ~WindowHooker()
        {
            UnHook();
        }

        public void Hook()
        {
           handle =  SetWindowsHookEx(WindowsHookType.WH_KEYBOARD_LL, Keyboard_hook_delegate, (IntPtr)0, 0);
        }

        public void UnHook()
        {
            UnhookWindowsHookEx(handle.DangerousGetHandle());
        }

        private int KeyBoardHook(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WindowMessage.WM_KEYDOWN)
            {
                KEYBDINPUT _input = Marshal.PtrToStructure<KEYBDINPUT>(lParam);
                if (_input.wVk == _c_v_key)
                {
                    if(aTimer.Enabled == true)
                    {
                        //说明定时器还没有结束，窗口出现
                        Key_activate();
                    }
                    else
                    {
                        //说明ctrl键才按了第一次，按到第二次的时候才会触发。
                        aTimer.Enabled = true;
                    }
                }

            }
            return CallNextHookEx((IntPtr)0, nCode, wParam, lParam); 
        }


        //定时器的方法
        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            aTimer.Enabled = false;
        }

        //定时器的初始化
        private static void SetTimer()
        {
            // Create a timer with a 500 milionsecond
            aTimer = new Timer(500);
            // Hook up the Elapsed event for the timer. 
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = false;
            aTimer.Enabled = false;
        }



        #region 引入win32的方法
        [DllImport("user32.dll")]
        private static extern bool UnhookWindowsHookEx(IntPtr hInstance);
        #endregion
    }


}