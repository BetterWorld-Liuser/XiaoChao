using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;

namespace xiaochao
{
    internal class HotkeyConverter
    {

        

        // Summary:
        //     [Special] Converts a hotkey string to its variant WK.Libraries.HotkeyListenerNS.Hotkey
        //     object.
        public static Hotkey Convert(string hotkey)
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

        
    }

    internal class Hotkey
    {
        //
        // Summary:
        //     Gets or sets the hotkey's keyboard code.
        public Key KeyCode
        {
            get;
            set;
        }

        //
        // Summary:
        //     Gets or sets the hotkey's modifier flags. The flags indicate which combination
        //     of CTRL, SHIFT, and ALT keys will be detected.
        public ModifierKeys Modifiers
        {
            get;
            set;
        }


        public Hotkey(Key keycode , ModifierKeys modifiers)
        {
            KeyCode = keycode;
            Modifiers = modifiers;
        }
    }
}
