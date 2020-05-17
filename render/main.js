let keyCodes = require("../dist/keyMap");
let db = require("../DB/main.js");
let Sortable = require("sortablejs");
let config = db.read().get("config").value();
let { ipcRenderer, remote } = require("electron");
let hotkeys = require("hotkeys-js");
let { v1: uuidv1 } = require("uuid");
let request = require("../request/main");
//require('electron').webFrame.setZoomFactor(1);
let draggable = require("vuedraggable");
let md5 = require('md5')

//定义变量
/* let sortModuleConfig = {
  group: "modules", // set both lists to same group
  animation: 150,
  handle: "#addNewItem",
  swapThreshold: 0.75,
  easing: "cubic-bezier(1, 0, 0, 1)",
  ghostClass: "sortable-ghost", // drop placeholder的css类名
  chosenClass: "sortable-chosen", // 被选中项的css 类名
  dragClass: "sortable-drag",
};

let sortKeyConfig = {
  group: "keys",
  animation: 150,
  swapThreshold: 0.5,
  easing: "cubic-bezier(1, 0, 0, 1)",
}; */

let app = new Vue({
  el: "#app",
  data: {
    platform: "1",
    appName: "快捷键助手",
    personalData: [],
    commonData: [
      {
        id: "1",
        moduleName: "暂存",
        items: [{ use: "", keys: [16, 17] }],
      },
    ],
    keyCodes,
    config,
    //addKeyVisible: "disappear",
    keyForm: {
      function: "",
      keys: [],
    },
    dataState: "公共数据",
    dataStateWith: 0,
    inputMethod: 0,
    inputState: "组合键模式",
    trash: [],
  },
  components: {
    draggable,
  },
  methods: {
    changeInputState() {
      if (this.inputMethod == 0) {
        this.inputMethod = 1;
        this.inputState = "累加键模式";
        this.keyForm.keys = [];
      } else {
        this.inputMethod = 0;
        this.inputState = "组合键模式";
        this.keyForm.keys = [];
      }
    },
    async getData(arg) {

      //得到本地数据
      let localData = JSON.parse(localStorage.getItem(arg));
      //console.log(localStorage.getItem(arg));
      //检查是否登陆
      let loginState = db.read().get("loginState").value();

      //先把本地数据拿出来
      if (localData != null) {
        if(loginState==true)this.personalData = localData.personalData;
        this.commonData = localData.commonData;
      }

      let personalRes={}
      let commonRes = await request.getCommonData(arg);
      this.commonData = commonRes.data.commonData;
      
    
      if (loginState) {
        //判断登录状态
        //console.log(this.commonData)
        let email = db.get('userEmail').value();
        let password = db.get('passWord').value();
        personalRes = await request.getPersonalData(arg, email, md5(password));
        if(personalRes.data.ok==2){
          this.personalData=[]
        }else{
          this.personalData = personalRes.data.personalData==null?[]:personalRes.data.personalData
        }
      }

    },
    async syncData() {

/*       let loginState = db.get('loginState').value()

      if(loginState==true){

      }else{

      } */

      //本地存储
      let storage = {
        personalData: this.personalData,
        commonData: this.commonData,
      };
      localStorage.setItem(this.appName, JSON.stringify(storage));

      //存网络
      let loginState = db.read().get("loginState").value();
      if (loginState) {
        let email = db.get('userEmail').value();
        let password = db.get('passWord').value();
        await request.syncPersonalData(email,md5(password),this.appName, this.personalData);
        await request.syncCommonData(this.appName, this.commonData);
      } else {
        await request.syncCommonData(this.appName, this.commonData);
        //await request.syncPersonalData(this.appName, this.personalData);
      }
    },
    changeDataState() {
      if (this.dataStateWith == 0) {
        let loginState = db.get('loginState').value()
        if(loginState==true){
          this.dataState = "私人数据";
          this.dataStateWith = 1;
        }else{
          remote.dialog.showMessageBox({
            title:"提示",
            message:"私人数据需要登录后才可使用"
          })
        }

        
      } else {
        this.dataState = "公共数据";
        this.dataStateWith = 0;
      }
    },
    addModule(name) {
      let id = uuidv1();
      if (this.dataStateWith == 0) {
        this.commonData.push({
          id,
          moduleName: name,
          items: [],
        });
      } else {
        this.personalData.push({
          id,
          moduleName: name,
          items: [],
        });
      }
      //添加模块

      let _this = this;
      Vue.nextTick(() => {
        _this.addNewSort(id);
      });
    },
    addNewKey() {
      event.preventDefault();
      //添加快捷键
      if (this.keyForm.function == "" || this.keyForm.keys == []) {
        alert("请输入功能或按键");
        return;
      }

      if (this.dataStateWith == 0) {
        this.commonData[0].items.push({
          id: uuidv1(),
          use: this.keyForm.function,
          keys: this.keyForm.keys,
          info: {},
        });
      } else {
        this.personalData[0].items.push({
          id: uuidv1(),
          use: this.keyForm.function,
          keys: this.keyForm.keys,
          info: {},
        });
      }
      hotkeys.unbind("*");
      this.reverseDisplay();
      this.keyForm.function = "";
      this.keyForm.keys = [];
    },
    openConfig() {
      ipcRenderer.send("openConfigWindow");
    },
    addSort() {
      //初始化添加sort
      console.log("正在初始化挂载排序...");
      let content = document.getElementById("content");
      let itemList = document.getElementsByClassName("itemList");
      let trash = document.getElementById("trash");
      //Sortable.create(content, sortModuleConfig); //给模块容器挂在上可移动

      //console.log(itemList.length)
      /* for (let i = 0; i < itemList.length; i++) {
        //给所有模块挂载上可移动
        Sortable.create(itemList[i], sortKeyConfig);
      } */

      //Sortable.create(trash, sortKeyConfig);
    },
    addNewSort(id) {
      //给新添加的模块添加可移动功能
      let newModule = document.getElementById(id);

      //Sortable.create(newModule, sortKeyConfig);
    },
    getWindowName() {
      //获取外部窗口名称
      let windowName = ipcRenderer.send("getWindow");
      this.appName = windowName;
    },
    blurConsole(id) {
      //修改模块的名字
      let changeName = event.target.innerText;

      if (this.dataStateWith == 0) {
        for (let i = 0; i < this.commonData.length; i++) {
          if (this.commonData[i].id == id) {
            this.commonData[i].moduleName = changeName;
            break;
          }
        }
      } else {
        for (let i = 0; i < this.personalData.length; i++) {
          if (this.personalData[i].id == id) {
            this.personalData[i].moduleName = changeName;
            break;
          }
        }
      }
    },
    WatchKey() {
      let _this = this;
      if (this.inputMethod == 0) {
        hotkeys("*", function () {
          let keys = hotkeys.getPressedKeyCodes();
          //console.log(keys); //=> [17, 65] or [70]
          _this.keyForm.keys = keys;
        });
      } else {
        hotkeys("*", function () {
          let keys = hotkeys.getPressedKeyCodes();
          //console.log(keys); //=> [17, 65] or [70]
          _this.keyForm.keys.push(keys);
        });
      }
    },
    cancelWatchKey() {
      hotkeys.unbind("*");
    },
    reverseDisplay() {
      event.preventDefault();
      let popWindow = document.getElementById("popWindow");
      if (popWindow.classList.value == "disappear") {
        popWindow.className = "appear";
      } else {
        popWindow.className = "disappear";
      }
    },
  },
  computed: {
    appData() {
      if (this.dataStateWith == 1) {
        return this.personalData;
      } else {
        return this.commonData;
      }
    },
  },
  watch: {
    appData() {
      this.$nextTick((_) => {
        //this.addSort()
      });
    },
  },
  mounted() {
    let _this = this;

    this.$nextTick(() => {
      _this.addSort();
    });
  },
  created() {
    //this.getWindowName();
    let _this = this;
    ipcRenderer.on("giveYouWin", async (event, arg) => {
      _this.appName = arg;
      _this.getData(arg);
    });
    ipcRenderer.on("syncData", (event, arg) => {
      let storage = {
        personalData: _this.personalData,
        commonData: _this.commonData,
      };
      localStorage.setItem(_this.appName, JSON.stringify(storage));
      document.getElementById("trash").innerHTML = "";
      _this.syncData();
    });
  },
});
