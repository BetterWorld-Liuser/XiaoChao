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
    //这个类采用单例模式
    public class ConfigManager
    {
        static private ConfigManager _instance;
        public bool Start_Up { get; set; } = true;
        public string Shortcut { get; set; } = "alt+s";
        public string Background_color { get; set; } = "#FFFFFF";
        
        public string Font_color { get; set; } = "#000000";
        public string Decoration_color { get; set; } = "#c9cdd4";
        public double Background_opacity { get; set; } = 0.9;

        public int Font_size { get; set; } = 15;
        public int Font_size_Title_in_assemble {
            get => Font_size + 4;
        }
        public int Font_size_Title_in_Toolbar
        {
            get => Font_size + 1;
        }

        public int Font_size_Title_of_version
        {
            get => Font_size - 7 ;
        }

        #region ConfigManager初始化函数
        private ConfigManager()
        {
            
            Dictionary<string, string> config_dictionary = new Dictionary<string, string>();
            config_dictionary = LoadConfig();
            InitSetting(config_dictionary);

        }
        #endregion ConfigManager初始化函数

        static public ConfigManager GetInstance()
        {
            return _instance ?? (_instance = new ConfigManager());
        }


        /// <summary>
        /// 导入配置文件
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> LoadConfig()
        {
            #region：找到配置文件并导入
            Dictionary<string, string> config_dictionary = new Dictionary<string, string>();
            string config_file_path = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "设置.md");
            if (File.Exists(config_file_path))
            {
                string color_pattern = "^#[a-fA-F0-9]{6}$";
                string opacity_pattern = "[0-9]{1,3}";
                //开始：循环遍历每一行元素，将其加入字典中
                foreach (string line in File.ReadLines(config_file_path))
                {

                    string lineTrimed = line.Trim();
                    string[] key_value = lineTrimed.Split(' ');
                    //将每个键值都加入字典中
                    if (key_value.Length >= 2) config_dictionary.Add(key_value[0], key_value[1]);


                }//结束：循环遍历每一行元素，将其加入字典中
            }
            #endregion 找到配置文件并导入
            return config_dictionary;
         }


        /// <summary>
        /// 初始化设置的各种值
        /// </summary>
        /// <param name="config_dictionary"></param>
        public void InitSetting(Dictionary<string, string> config_dictionary)
        {
            //设置开机启动
            string start_up;
            if (config_dictionary.TryGetValue("开机启动", out start_up))
            {
                if (start_up == "否") Start_Up = false;
            }

            //设置快捷键
            string shortcut;
            if (config_dictionary.TryGetValue("快捷键", out shortcut)) Shortcut = shortcut;


            //设置背景色
            string backgroundcolor;
            if (config_dictionary.TryGetValue("背景色", out backgroundcolor)) Background_color = backgroundcolor;

            //设置透明度
            string opacity;
            if (config_dictionary.TryGetValue("透明度", out opacity))
            {
                double temp = double.Parse(opacity);
                if (temp > 0 & temp < 100) Background_opacity = temp / 100;
            }

            //设置字体颜色
            string font_color;
            if (config_dictionary.TryGetValue("字体颜色", out font_color)) Font_color = font_color;

            //设置装饰色
            string decorationcolor;
            if (config_dictionary.TryGetValue("装饰色", out decorationcolor)) Decoration_color = decorationcolor;

            //设置基准大小
            string basesize;
            if (config_dictionary.TryGetValue("基准大小", out basesize))
            {
                Font_size = int.Parse(basesize);
            }

        }

    }
}
