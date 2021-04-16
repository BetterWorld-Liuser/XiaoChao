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

new Vue({
  el: "#app",
  data: {
    platform: "1",
    appName: "快捷键助手",
    personalData: [],
    commonData: [
      {
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
  },
  computed: {
  }
});
