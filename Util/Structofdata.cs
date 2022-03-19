using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xiaochao
{
    //单例模式
    //
    public class Structofdata : INotifyPropertyChanged
    {

        //------------------------------- 属性 -----------------------------

        private KeyValueAssembleList[] _keyValueAssemblesListArray { get; set; } = new KeyValueAssembleList[4];
        private string _bigtitle = "无标题";
        private string _bigtitle2 = "没找到";


        private static Structofdata _selfinstance;
        public event PropertyChangedEventHandler PropertyChanged;


        /// <summary>
        /// 关于快捷键的全部数据
        /// </summary>
        public KeyValueAssembleList[] KeyValueAssemblesListArray
        {
            get { return _keyValueAssemblesListArray; }
            set
            {
                _keyValueAssemblesListArray = value;
                OnPropertyChanged();

            }
        }
        /// <summary>
        /// 大标题
        /// </summary>
        public string Bigtitle
        {
            get { return _bigtitle; }
            set
            {
                _bigtitle = value;
                OnPropertyChanged();
            }
        }

        public string Bigtitle2
        {
            get { return _bigtitle2; }
            set
            {
                _bigtitle2 = value;
                OnPropertyChanged();
            }
        }




        //-------------------------- 构造函数 -----------------------
        /// <summary>
        /// 构造函数,页面中的全部数据
        /// </summary>
        /// <param name="keyValueAssemblesListArray"></param>
        /// <param name="bigtitle"></param>
        private Structofdata(KeyValueAssembleList[] keyValueAssemblesListArray, string bigtitle, string bigtitle2) {
            KeyValueAssemblesListArray = keyValueAssemblesListArray;
            Bigtitle = bigtitle;
            Bigtitle2 = bigtitle2;
        }


        private Structofdata() {

        }



        /// <summary>
        /// 单例模式获取对象实例的接口
        /// </summary>
        /// <param name="keyValueAssemblesArray"></param>
        /// <param name="bigtitle"></param>
        /// <returns></returns>
        static public Structofdata InitInstance(KeyValueAssembleList[] keyValueAssemblesArray, string bigtitle, string bigtitle2)
        {
            if(_selfinstance == null)
            {
                _selfinstance = new Structofdata(keyValueAssemblesArray,bigtitle,bigtitle2);
            }
            else
            {
                _selfinstance.KeyValueAssemblesListArray = keyValueAssemblesArray;
                _selfinstance.Bigtitle = bigtitle;
                _selfinstance.Bigtitle2 = bigtitle2;
            }
            return _selfinstance;
        }

        static public Structofdata InitInstance()
        {
            _selfinstance=new Structofdata();
            return _selfinstance;
        }




        //---------------------------------工具方法-----------------------------
        /// <summary>
        /// 属性改变触发方法
        /// </summary>
        /// <param name="name"></param>
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }



    }


    //--------------------- 类型定义 -----------------------

    public class KeyList
    {
        public List<string> Keys { get; set; } = new List<string>();
        public KeyList(string keyString)
        {
            Keys = keyString.Split('+').ToList();
            //Keys.Reverse();
        }
    }

    public class KeyValue
    {
        public KeyList Key { get; set; } = new KeyList("");
        public string Value { get; set; } = "";
        public string Url { get; set; } = "";
        public int Height { get; set; } = 0;
    }

    public class KeyValueAssemble
    {
        public List<KeyValue> KeyValues { get; set; } = new List<KeyValue>();
        public string SmallTitle { get; set; } = "";
        public int Height { get; set; } = 0;
        public FunctionType Ftype { get; set; } = FunctionType.Normal;
    }


    public class KeyValueAssembleList
    {
        public List<KeyValueAssemble> KeyValueAssemblesListInstance { get; set; } = new List<KeyValueAssemble>();
        public int height = 0;
        
    }


    public enum FunctionType
    {
        ContainURl,
        Normal
    }
}


    