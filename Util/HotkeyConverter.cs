using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;

namespace xiaochao
{
    internal class HotkeyConverter
    {
        /// <summary>
        /// 将字符串的快捷键转化为系统的快捷键
        /// </summary>
        /// <param name="hotkey"></param>
        /// <returns></returns>
        public static Hotkey Convert_N(string hotkey)
        {
            Key keys = Key.None;
            ModifierKeys keys2 = ModifierKeys.None;
            hotkey = hotkey.Replace(" ", "");
            hotkey = hotkey.Replace(",", "");
            hotkey = hotkey.Replace("+", "");
            if (hotkey.IndexOf("ctrl", StringComparison.OrdinalIgnoreCase)>=0)
            {
                keys2 |= ModifierKeys.Control;
                hotkey = Regex.Replace(hotkey, "Ctrl", "", RegexOptions.IgnoreCase);
            }

            if (hotkey.IndexOf("shift", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                keys2 |= ModifierKeys.Shift;
                hotkey = Regex.Replace(hotkey, "Shift", "", RegexOptions.IgnoreCase);
            }

            if (hotkey.IndexOf("alt", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                keys2 |= ModifierKeys.Alt;
                hotkey = Regex.Replace(hotkey, "Alt", "", RegexOptions.IgnoreCase);
            }

            if (hotkey.IndexOf("win", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                keys2 |= ModifierKeys.Windows;
                hotkey = Regex.Replace(hotkey, "win", "", RegexOptions.IgnoreCase);
            }

            keys = (Key)Enum.Parse(typeof(Key), hotkey, ignoreCase: true);
            return new Hotkey(keys, keys2);
        }

        /// <summary>
        /// 做一层抽象，对双击的快捷键和正常的快捷键注册采用两种在注册模式
        /// </summary>
        /// <param name="hotkey"></param>
        /// <param name="_key"></param>
        /// <param name="_hotkey"></param>
        /// <returns></returns>
        public static bool Convert(string hotkey,out Key _key,out Hotkey _hotkey)
        {
            hotkey = hotkey.Replace(",", " ");
            hotkey = hotkey.Replace("+", " ");
            var a  = hotkey.Split(' ');
            if (a[0] == a[1])
            {
                //默认注册ctrl
                _key = Key.LeftCtrl;
                if (hotkey.IndexOf("ctrl", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    _key = Key.LeftCtrl;
                }

                if (hotkey.IndexOf("shift", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    _key = Key.LeftShift;
                }

                if (hotkey.IndexOf("alt", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    _key = Key.LeftAlt;
                }

                if (hotkey.IndexOf("win", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    _key = Key.LWin;
                }
                _hotkey = null;
                return true;
            }
            else
            {
                _key = Key.Clear;
                _hotkey = Convert_N(hotkey);
                return false;
            }
            
        }
        
    }


    //快捷键的组合类
    internal class Hotkey
    {
        //
        // Summary:
        //     Gets or sets the hotkey's keyboard code.
        public Key KeyCode {get;set;}

        //
        // Summary:
        //     Gets or sets the hotkey's modifier flags. The flags indicate which combination
        //     of CTRL, SHIFT, and ALT keys will be detected.
        public ModifierKeys Modifiers {get;set;}


        public Hotkey(Key keycode , ModifierKeys modifiers)
        {
            KeyCode = keycode;
            Modifiers = modifiers;
        }
    }



    //官方提供的注册快捷键的方法
    public static class NativeMethods
    {
        

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)] 
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint key, uint modifier); 

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)] 
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        
    }


    }

 
