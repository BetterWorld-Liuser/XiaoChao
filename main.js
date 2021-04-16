const {
  BrowserWindow,
  Tray,
  app,
  Menu,
  globalShortcut,
  ipcMain,
} = require("electron");
const activeWin = require("active-win");
const path = require("path");
let db = require("./DB/main.js");


//声明变量
let mainWindow = null; //声明要打开的主窗口
let configWindow = null;
let donateWindow = null;
let iconPath = path.join(__dirname, "dist/icons/document128good.png");
let showState = true;
let keyCodes = require("./dist/keyMapMain");


//声明函数
let openAndCreateMainWindow = function(){
  mainWindow = new BrowserWindow({
    //创建主窗口
    width: 1200,
    height: 700,
    webPreferences: {
      nodeIntegration:true,
      contextIsolation:false
    },
    frame: false,
    zoomFactor: 1.0,
    //backgroundColor: '#d9d9d9',
    transparent: true,
    thickFrame: false,
    hasShadow: false,
    resizable: false,
    center: true,
    movable: false,
    alwaysOnTop: false,
    skipTaskbar: true,
    show: true,
  });
  mainWindow.loadFile("./pages/index.html");
  mainWindow.on("closed", () => {
    mainWindow = null;
  });
}
let openAndCreateConfigWindow = function(){
  configWindow = new BrowserWindow({
    //设置窗口
    width: 500,
    height: 700,
    webPreferences: {
      nodeIntegration: true,
      nodeIntegrationInWorker: true,
    },
    frame: false,
    zoomFactor: 1.1,
    //parent:mainWindow,
    //backgroundColor: '#d9d9d9',
    transparent: true,
    //thickFrame:false,
    //hasShadow:false,
    resizable: false,
    center: true,
    movable: false,
    alwaysOnTop: false,
    skipTaskbar: true,
    show: false,
  });
  configWindow.loadFile("./pages/config.html");
  configWindow.on("closed", () => {
    configWindow = null;
  });
}
let openAndCreateDonateWindow = function(){
  donateWindow = new BrowserWindow({
    //设置窗口
    width: 700,
    height: 600,
    webPreferences: {
      nodeIntegration: true,
      nodeIntegrationInWorker: true,
    },
    //parent: mainWindow,
    frame: false,
    zoomFactor: 1.1,
    //backgroundColor: '#d9d9d9',
    //transparent: true,
    //thickFrame:false,
    //hasShadow:false,
    resizable: false,
    center: true,
    movable: false,
    alwaysOnTop: false,
    skipTaskbar: true,
    show: false,
  })
  donateWindow.loadFile("./pages/donate.html")
  donateWindow.on("closed", () => {
    donateWindow = null;
  });
}
let registerShortCut = function(){
  let shortCut = "";
  for (let i = 0; i < config.keys.length; i++) {
    shortCut += keyCodes[config.keys[i]];
    if (i != config.keys.length - 1) {
      shortCut += "+";
    }
  }
  globalShortcut.register(shortCut, () => {
    if (showState == true) {
      //同步快捷键
      mainWindow.webContents.send('syncData','')
      mainWindow.hide();
      showState = false;
    } else {
      let realWinName = activeWin
        .sync()
        .owner.name.replace(/\.exe/, "")
      //打开窗口
      mainWindow.webContents.send("giveYouWin", realWinName);
      
      mainWindow.show();
      showState = true;
    }
  });
}
let createTray = function(){
  let control = db.get('config.showConfig').value()
  if(control==true){
    tray = new Tray(iconPath);
      let contextMenu = Menu.buildFromTemplate([
    { label: "设置", type: "normal", click: () => {
      openAndCreateConfigWindow()
      configWindow.show()
    } },
    { label: "捐赠", type: "normal", click: () => {
      openAndCreateDonateWindow()
      donateWindow.show()
    } },
    { label: "退出", type: "normal", click: () => {
      app.quit()
    } }
  ]);
  tray.setToolTip("快捷键助手");
  tray.setContextMenu(contextMenu);
  }
}
//读取设置
let config = db.get("config").value();

//APP设置
//app.allowRendererProcessReuse = true;

//设置主进程和渲染进程之间的通信
ipcMain.on("getWindow", (event) => {
  let realWinName = activeWin.sync().owner.name.replace(/\.exe/, "")
  //let realWinName = winName.slice(0, 1).toUpperCase() + winName.slice(1); //获得窗口的名字
  event.returnValue = realWinName;
});
ipcMain.on("closeConfigWindow", (event) => {
  configWindow.close();
});
ipcMain.on("closeDonateWindow", (event) => {
  donateWindow.close();
});
ipcMain.on("openDonateWindow", (event) => {
  openAndCreateDonateWindow()
  donateWindow.once('ready-to-show',()=>{
    donateWindow.show()
  })
});
ipcMain.on("openConfigWindow", (event) => {
  openAndCreateConfigWindow()
  configWindow.once('ready-to-show',()=>{
    configWindow.show()
  })
});



app.on("ready", () => {
  //注册全局快捷键
  registerShortCut()

  //注册系统任务栏
  createTray()

  //制造窗口
  openAndCreateMainWindow()
  openAndCreateConfigWindow()
  openAndCreateDonateWindow()


  mainWindow.once("ready-to-show", () => {
    //mainWindow.show()
    //configWindow.show();
    //donateWindow.show()
    //mainWindow.webFrame.setZoomFactor(1);
    //configWindow.webContents.setZoomFactor(1);
  });

  //打开调试器
  mainWindow.webContents.openDevTools(); 
  //donateWindow.webContents.openDevTools();
  //configWindow.webContents.openDevTools();
});

app.on("will-quit", () => {
  // 注销所有快捷键
  globalShortcut.unregisterAll();
});
