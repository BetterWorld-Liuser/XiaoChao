using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using System.Reflection;

namespace xiaochao
{
    public class ConfigManager
    {
        static private ConfigManager _instance;
        public bool Start_Up { get; set; } = true;
        public string Shortcut { get; set; } = "alt+s";
        public string Background_color { get; set; } = "#FFFFFF";
        
        public string Font_color { get; set; } = "#000000";
        public string Decoration_color { get; set; } = "#747d8c";
        public double Background_opacity { get; set; } = 0.9;



        private ConfigManager()
        {
            string config_file_path = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "设置.md");
            //找到配置文件的位置
            if (File.Exists(config_file_path))
            {
                //找到
                foreach (string line in File.ReadLines(config_file_path))
                {
                    string color_pattern = "^#[a-fA-F0-9]{6}$";
                    string opacity_pattern = "[0-9]{1,3}";
                    string line_a = line.Trim();
                    string[] key_value = line_a.Split(' ');
                    if (key_value.Length >= 2)
                    {
                             if (line.Contains("开机启动"))
                        {

                            if (key_value[1] == "是")
                            {
                                Start_Up = true;
                            }
                            else
                            {
                                Start_Up = false;
                            }
                        }
                        else if (line.Contains("快捷键"))
                        {
                            //设置快捷键
                            Shortcut = key_value[1];

                        }
                        else if (line.Contains("背景色"))
                        {
                            if (Regex.IsMatch(key_value[1], color_pattern))
                            {
                                Background_color = key_value[1];
                            }

                        }
                        else if (line.Contains("透明度"))
                        {
                            if (Regex.IsMatch(key_value[1], opacity_pattern))
                            {
                                double temp = double.Parse(key_value[1]);
                                if (temp > 0 & temp < 100)
                                {
                                    Background_opacity = temp/100;
                                }
                            }
                        }
                        else if (line.Contains("字体颜色"))
                        {
                            if (Regex.IsMatch(key_value[1], color_pattern))
                            {
                                Font_color = key_value[1];
                            }
                        }
                        else if (line.Contains("装饰色"))
                        {
                            if (Regex.IsMatch(key_value[1], color_pattern))
                            {
                                Decoration_color = key_value[1];
                            }
                        }
                    }
                    
                }

            }
        }

        static public ConfigManager GetInstance()
        {
            return _instance ?? (_instance = new ConfigManager());
        }
    }
}
