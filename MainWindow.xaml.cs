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
using Path = System.IO.Path;
using System.Reflection;
using System.Windows.Forms;
using System.Drawing;
using System.Windows.Interop;
using xiaochao.Util;

namespace xiaochao
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region 固定变量定义
        const int HOTKEY_id = 6666;


        #endregion 固定变量定义



        #region 属性定义
        public Structofdata Data { get; set; } = Structofdata.InitInstance();

        private static string BaseDir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        private static string AppDir = Path.Combine(BaseDir, "XiaoChao.exe");
        //data文件夹地址
        private readonly string _sub_directory = Path.Combine(BaseDir, "data");
        //local文件夹地址
        private readonly string _local_directory = Path.Combine(BaseDir, "local");

        //用于全局按键的hooker，完成类似双击等操作的捕捉
        private static WindowHooker _hooker;

        public ConfigManager ConfigManagerInstance { get; set; } = ConfigManager.GetInstance();
        //一条键与值的高度
        public int Normal_data_height { get; set; } = 27;
        //大标题的高度
        public int Bigtitle_data_height { get; set; } = 30;
        //列的数量
        public int Column_count { get; set; } = 3;

        //窗口的长度
        public int Window_Height { get; set; } = 750;
        //窗口的高度
        public int Window_Width { get; set; } = 1200;
        //列宽
        public int Column_Width { get; set; }

        //每个元素的宽度
        public int Colum_Item_Width { get; set; }
        public string Version { get; set; } = Assembly.GetExecutingAssembly().GetName().Version.ToString();

        #endregion -------------------------- 属性定义 ------------------------




        #region --------------------------构造函数-----------------------------
        /// <summary>
        /// 构造函数
        /// </summary>
        public MainWindow()
        {
            //每一列的宽度
            Column_Width = (Window_Width - 40) / Column_count;

            //每个元素的宽度
            Colum_Item_Width = Column_Width - 20;

            //父类的方法
            InitializeComponent();

            InitHwnd();

            Hide();

            Check_Directory_Exist();
            
            //设置开机启动
            //record:2022年2月24日
            StartUp(ConfigManagerInstance.Start_Up);

            RetrieveData();
            //如果开机启动则不自动显示
            string[] args = Environment.GetCommandLineArgs();
            if (!args.Contains("--autostart"))
            {
                Show();
            }

            

        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            //InitHwnd();
            //绑定快捷键，因为只有在这个时候快捷键绑定才是有效的
            Register();
            //HwndSource source = PresentationSource.FromVisual(this) as HwndSource;
            //source.AddHook(WndProc);
        }



        #endregion --------------------------构造函数-----------------------------


        protected override void OnClosed(EventArgs e)
        {
            //取消注册快捷键
            NativeMethods.UnregisterHotKey(WindowsManager.GethWnd(), HOTKEY_id);
            base.OnClosed(e);
        }

        /// <summary>
        /// 获取关于目前前台应用的数据
        /// </summary>
        private void RetrieveData()
        {
            //初始化大标题
            string bigtitle = WindowsManager.GetFrontWindowText();

            //初始化exe标题
            string bigtitle_exe = WindowsManager.GetWindowProcessName();

            //初始化数据array，自定义Column的数量
            KeyValueAssembleList[] keyValueAssemblesArray = new KeyValueAssembleList[Column_count];

            //判断是否有data文件夹
            bool data_exist = Directory.Exists(Path.Combine(BaseDir, "data"));
            bool local_exist = Directory.Exists(Path.Combine(BaseDir, "local"));
            if ((!data_exist) && (!local_exist)) return;


            //搜索文件List
            string[] data_fileArray = Directory.GetFiles(Path.Combine(BaseDir, "data"), "*.*", SearchOption.TopDirectoryOnly);
            string[] local_fileArray = Directory.GetFiles(Path.Combine(BaseDir, "local"), "*.*", SearchOption.TopDirectoryOnly);

            //连接两个文件List
            string[] fileArray = new string[data_fileArray.Length + local_fileArray.Length];
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
                                if (key_value.Length < 2) break;
                                KeyValue keyValue = new KeyValue();
                                keyValue.Height = Normal_data_height;
                                keyValue.Key = new KeyList(key_value[0]);
                                keyValue.Value = key_value[1].Replace('+', ' ');
                                if (key_value.Length >= 3)
                                {
                                    keyValue.Url = key_value[2];
                                    tempkeyValueAssembles.Last().Ftype = FunctionType.ContainURl;
                                }
                                tempkeyValueAssembles.Last().KeyValues.Add(keyValue);
                                tempkeyValueAssembles.Last().Height += Normal_data_height;


                            }
                        }
                    }
                }
            }




            //init some Lists
            for (int i = 0; i < Column_count; i++)
            {
                keyValueAssemblesArray[i] = new KeyValueAssembleList();
            }

            //循环插入数据,判别哪个List最短插入哪个,并把最后一行空出来放带有URI的数据
            foreach (KeyValueAssemble keyValueAssemble in tempkeyValueAssembles)
            {
                switch (keyValueAssemble.Ftype)
                {
                    case FunctionType.Normal:
                        int min_length_index = 0;
                        int temp_min_length_index = 0;
                        for (int i = 0; i < Column_count - 1; i++)
                        {
                            if (keyValueAssemblesArray[i].height < keyValueAssemblesArray[temp_min_length_index].height)
                            {
                                min_length_index = i;
                                temp_min_length_index = i;
                            }
                        }
                        keyValueAssemblesArray[min_length_index].KeyValueAssemblesListInstance.Add(keyValueAssemble);
                        keyValueAssemblesArray[min_length_index].height += keyValueAssemble.Height;
                        break;
                    case FunctionType.ContainURl:
                        keyValueAssemblesArray[keyValueAssemblesArray.Length - 1].KeyValueAssemblesListInstance.Add(keyValueAssemble);
                        keyValueAssemblesArray[keyValueAssemblesArray.Length - 1].height += keyValueAssemble.Height;
                        break;
                }
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
                System.Windows.MessageBox.Show("没有检测到data文件夹(1)");
            }
            else if (code == 2)
            {
                System.Windows.MessageBox.Show("没有检测到设置文件, 采用默认设置(2)");
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
                Screen screen = Screen.FromPoint(System.Windows.Forms.Cursor.Position);
                var scaleRatio = Math.Max(VisualTreeHelper.GetDpi(this).DpiScaleX, VisualTreeHelper.GetDpi(this).DpiScaleY);

                this.Left = (screen.WorkingArea.Left + (screen.WorkingArea.Width - this.Width * scaleRatio) / 2) / scaleRatio;
                this.Top = (screen.WorkingArea.Top + (screen.WorkingArea.Height - this.Height * scaleRatio) / 2) / scaleRatio;
                RetrieveData();
                Show();
                Activate();
                Focus();
            }
        }


        /// <summary>
        /// 注册快捷键, 需要window的handle
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Register()
        {
            IntPtr hWnd = new WindowInteropHelper(this).Handle;
            //HotkeyManager.Current.AddOrReplace("SwitchShow", hotkey.KeyCode, hotkey.Modifiers, HotkeyPressed);
            //如果是double注册...
            //如果不是double注册
            if(HotkeyConverter.Convert(ConfigManagerInstance.Shortcut, out Key _key, out Hotkey _hotkey))
            {
                //注册双击click的方法
                _hooker = new WindowHooker();
                _hooker._c_v_key = (PInvoke.User32.VirtualKey)KeyInterop.VirtualKeyFromKey(_key);
                _hooker.Key_activate = Switchwindow;
            }
            else
            {
                int key = KeyInterop.VirtualKeyFromKey(_hotkey.KeyCode);
                bool success = NativeMethods.RegisterHotKey(hWnd, HOTKEY_id, (uint)_hotkey.Modifiers, (uint)key);
                if (!success)
                {
                    System.Windows.MessageBox.Show("快捷键注册失效，请重新设置");
                    Close();
                }
            }


            HwndSource _source = HwndSource.FromHwnd(hWnd);
            _source.AddHook(HotKeyHook);




        }


        /// <summary>
        /// 快捷键触发的处理方法
        /// </summary>
        private IntPtr HotKeyHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;
            if (wParam == (IntPtr)HOTKEY_id && msg == WM_HOTKEY)
            {
                Switchwindow();
                handled = true;
            }
            return IntPtr.Zero;
        }



        //------------------- 工具函数 --------------------

        //打开设置文件夹，必须待在MainWindow中
        private void Setting_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "设置.md")))
            {
                System.Diagnostics.Process.Start("Explorer.exe", Path.Combine(BaseDir, "设置.md"));
            }
            else
            {
                System.Windows.MessageBox.Show("请自行创建 {设置.md} 文件");
                System.Diagnostics.Process.Start("Explorer.exe", Directory.GetCurrentDirectory());
            }
            Hide();

        }

        //检查是否有数据文件夹，没有则创建，必须待在MainWindow中
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

        //打开数据文件夹，必须待在MainWindow中
        private void Data_Click(object sender, RoutedEventArgs e)
        {

            System.Diagnostics.Process.Start("Explorer.exe", Path.Combine(BaseDir, "local"));
            Hide();
        }

        /// <summary>
        /// 确保窗口有Hwnd
        /// </summary>
        public void InitHwnd()
        {
            var helper = new WindowInteropHelper(this);
            helper.EnsureHandle();
        }



        /// <summary>
        /// 此函数负责开机启动。
        /// </summary>
        /// <param name="start">true代表开机启动，false代表不</param>
        private void StartUp(bool start)
        {
            if (start)
            {
                Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                Assembly curAssembly = Assembly.GetExecutingAssembly();
                key.SetValue(curAssembly.GetName().Name, AppDir + " --autostart");

            }
            else
            {
                Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                Assembly curAssembly = Assembly.GetExecutingAssembly();
                key.DeleteValue(curAssembly.GetName().Name);
            }
        }

        //退出程序
        private void Quit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        //url被点击
        private void Url_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Windows.Controls.Button bt = (System.Windows.Controls.Button)sender;
                var dataContext = (KeyValue)bt.DataContext;
                string url = dataContext.Url;
                if (url != "")
                {
                    System.Diagnostics.Process.Start(url);
                    Hide();
                }
            }catch (Exception ex)
            {
                System.Windows.MessageBox.Show("链接有问题，请重新编辑链接");
            }

        }


        //当窗口失去焦点时自动隐藏
        private void Main_Deactivated(object sender, EventArgs e)
        {
            Hide();
        }

        //更新的按钮
        private void Update_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/BetterWorld-Liuser/XiaoChao");
            Hide();
        }

        //处理消息队列
        //private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        //{
        //    const int WM_INPUT = 0x00FF;

        //    switch (msg)
        //    {
        //        case WM_INPUT:

        //    }

        //    return IntPtr.Zero;
        //}
    }
}
