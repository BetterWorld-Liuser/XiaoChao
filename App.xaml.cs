using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Forms;
using System.Drawing;

namespace xiaochao
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        System.Threading.Mutex mutex;
        public App()
        {
            this.Startup += new StartupEventHandler(App_Startup);
        }

        void App_Startup(object sender, StartupEventArgs e)
        {
            bool ret;
            mutex = new System.Threading.Mutex(true, "XiaoChao", out ret);

            if (!ret)
            {
                System.Windows.MessageBox.Show("已有一个程序实例运行");
                Environment.Exit(0);
            }

        }
    }
}
