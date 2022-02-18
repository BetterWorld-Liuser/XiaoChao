using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using NHotkey.Wpf;
using NHotkey;
using Path = System.IO.Path;
using WK.Libraries.BootMeUpNS;
using System.Reflection;


namespace xiaochao
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        //-------------------------- 属性定义↓ --------------------------
        public Structofdata Data { get; set; } = Structofdata.InitInstance();
        //data文件夹地址
        private readonly string _sub_directory = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "data");
        //local文件夹地址
        private readonly string _local_directory = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "local");


        public ConfigManager ConfigManagerInstance { get; set; } = ConfigManager.GetInstance();
        public int Normal_data_height { get; set; } = 20;
        public int Bigtitle_data_height { get; set; } = 30;
        public int Column_count { get; set; } = 4;
        public string Version { get; set; } = Assembly.GetExecutingAssembly().GetName().Version.ToString();

        //-------------------------- 属性定义↑ ------------------------




        //-------------------------- 构造函数👇-----------------------
        /// <summary>
        /// 构造函数
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            Hide();

            Check_Directory_Exist();

            //绑定快捷键
            Hotkey hotkey = HotkeyConverter.Convert(ConfigManagerInstance.Shortcut);
            HotkeyManager.Current.AddOrReplace("SwitchShow", hotkey.KeyCode, hotkey.Modifiers, HotkeyPressed);


            //设置相开机启动
            if (ConfigManagerInstance.Start_Up)
            {
                var bootMeUp = new BootMeUp
                {
                    UseAlternativeOnFail = true,
                    BootArea = BootMeUp.BootAreas.StartupFolder,
                    TargetUser = BootMeUp.TargetUsers.CurrentUser,

                    // Enable auto-booting.
                    Enabled = true
                };
            }
            else
            {
                var bootMeUp = new BootMeUp
                {
                    UseAlternativeOnFail = true,
                    BootArea = BootMeUp.BootAreas.StartupFolder,
                    TargetUser = BootMeUp.TargetUsers.CurrentUser,

                    // Enable auto-booting.
                    Enabled = false
                };
            }



            //UpdateApp();


            //初始化数据
            RetrieveData();
            Show();

        }





        //-------------------------- 构造函数👆 --------------------------

        /// <summary>
        /// 获取关于目前前台应用的数据
        /// </summary>
        private void RetrieveData()
        {
            //初始化大标题
            string bigtitle = WindowsManager.GetFrontWindowText();

            //初始化exe标题
            string bigtitle_exe = WindowsManager.GetWindowProcessName();


            //初始化数据array
            KeyValueAssembleList[] keyValueAssemblesArray = new KeyValueAssembleList[Column_count];

            //判断是否有data文件夹
            bool data_exist = Directory.Exists(Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "data"));
            bool local_exist = Directory.Exists(Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "local"));
            if ((!data_exist)&&(!local_exist)) return;


            //搜索文件List
            string[] data_fileArray = Directory.GetFiles("data", "*.*", SearchOption.TopDirectoryOnly);
            string[] local_fileArray = Directory.GetFiles("local", "*.*", SearchOption.TopDirectoryOnly);

            //连接两个文件List
            string[] fileArray = new string[data_fileArray.Length+local_fileArray.Length];
            Array.Copy(data_fileArray, fileArray, data_fileArray.Length);
            Array.Copy(local_fileArray, 0, fileArray, data_fileArray.Length, local_fileArray.Length);

            List<string> fileList = new List<string>();
            for (int i = 0; i < fileArray.Length; i++)
            {
                string file_name = Path.GetFileNameWithoutExtension(fileArray[i]);
                if (bigtitle.Contains(file_name) | bigtitle_exe.Contains(file_name) | file_name == "全局")
                {
                    fileList.Add(fileArray[i]);
                }
            }

            //初始化临时assemble列表
            List<KeyValueAssemble> tempkeyValueAssembles = new List<KeyValueAssemble>();

            //循环遍历每个文件,将assemblelist填充完毕
            if (fileList.Count != 0)
            {
                foreach (string file in fileList)
                {
                    //循环遍历每个文件的每一行
                    foreach (string line in File.ReadLines(file))
                    {
                        //两个处理逻辑
                        //一个是遇到#打头的
                        //一个是正常的数据
                        string line_clean = line.Trim();
                        if (line_clean.Length > 0)
                        {
                            if (line_clean[0] == '#')
                            {

                                tempkeyValueAssembles.Add(new KeyValueAssemble());
                                //添加标题
                                tempkeyValueAssembles.Last().SmallTitle = line_clean.Substring(1).Trim();
                                tempkeyValueAssembles.Last().Height += Bigtitle_data_height;

                            }
                            else
                            {
                                //正常数据
                                string[] key_value = line_clean.Split(' ');
                                KeyValue keyValue = new KeyValue
                                {
                                    Height = Normal_data_height,
                                    Key = key_value[0],
                                    Value = key_value[1]
                                };
                                keyValue.Height = Normal_data_height;
                                if (key_value.Length >= 3)
                                {
                                    keyValue.Url = key_value[2];
                                }
                                tempkeyValueAssembles.Last().KeyValues.Add(keyValue);
                                tempkeyValueAssembles.Last().Height += Normal_data_height;


                            }
                        }
                    }
                }
            }




            //先初始化四个List
            for (int i = 0; i < Column_count; i++)
            {
                keyValueAssemblesArray[i] = new KeyValueAssembleList();
            }

            //循环插入数据,判别哪个List最短插入哪个
            foreach (KeyValueAssemble keyValueAssemble in tempkeyValueAssembles)
            {
                int min_length_index = 0;
                int temp_min_length_index = 0;
                for (int i = 0; i < Column_count; i++)
                {
                    if (keyValueAssemblesArray[i].height < keyValueAssemblesArray[temp_min_length_index].height)
                    {
                        min_length_index = i;
                        temp_min_length_index = i;
                    }
                }
                keyValueAssemblesArray[min_length_index].KeyValueAssemblesListInstance.Add(keyValueAssemble);
                keyValueAssemblesArray[min_length_index].height += keyValueAssemble.Height;
            }
            Data = Structofdata.InitInstance(keyValueAssemblesArray, bigtitle, bigtitle_exe);

        }

        /// <summary>
        /// 给予用户提示
        /// </summary>
        /// <param name="code"></param>
        private void MessageShow(int code)
        {

            if (code == 1)
            {
                MessageBox.Show("没有检测到data文件夹(1)");
            }
            else if (code == 2)
            {
                MessageBox.Show("没有检测到设置文件, 采用默认设置(2)");
            }
        }

        /// <summary>
        /// 切换窗口显示状态
        /// </summary>
        private void Switchwindow()
        {
            if (IsVisible)
            {
                Hide();
            }
            else
            {
                RetrieveData();
                Show();
                Activate();
            }
        }

        /// <summary>
        /// 快捷键按下的处理函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HotkeyPressed(object sender, HotkeyEventArgs e)
        {
            if (e.Name == "SwitchShow")
            {

                Switchwindow();
            }



        }




        //------------------- 工具函数 --------------------

        //打开设置文件夹
        private void Setting_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "设置.md")))
            {
                System.Diagnostics.Process.Start("Explorer.exe", Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "设置.md"));
            }
            else
            {
                MessageBox.Show("请自行创建 {设置.md} 文件");
                System.Diagnostics.Process.Start("Explorer.exe", Directory.GetCurrentDirectory());
            }

        }

        //检查是否有数据文件夹，没有则创建
        private void Check_Directory_Exist()
        {
            if (!Directory.Exists(_sub_directory))
            {
                Directory.CreateDirectory(_sub_directory);
            }
            if (!Directory.Exists(_local_directory))
            {
                Directory.CreateDirectory(_local_directory);
            }
        }

        //打开数据文件夹
        private void Data_Click(object sender, RoutedEventArgs e)
        {

            System.Diagnostics.Process.Start("Explorer.exe", Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "local"));

        }

        //退出程序
        private void Quit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }




    }
}
