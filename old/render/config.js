let keyCodes = require("../dist/keyMap");
let db = require("../DB/main.js");
let request = require('../request/main')
let { ipcRenderer,remote} = require("electron");
let hotkeys = require("hotkeys-js");
let md5 = require("md5")
//定义变量
let config = db.get("config").value();
let loginImgPath = {
    1:"../dist/icons/allogin.png",
    2:"../dist/icons/nologin.png"
}


let configApp = new Vue({
    el: "#app",
    data:{
        email:"",
        password:"",
        loginState:"未登录",
        imgSrc:"../dist/icons/nologin.png",
        keyCodes,
        keys:[18,83],
        powerConfig:true,
        showConfig:true,
        update:0,
        updatePlace:""
    },
    methods:{
        async login(){
            db.set('userEmail',this.email).write()
            db.set('passWord',this.password).write()
            if(this.email==""||this.password==""){
                this.loginState="未登录"
                this.imgSrc = loginImgPath[2]
                
            }else{
                let res =await  request.login(this.email,md5(this.password))
                console.log(res)
                let ok = res.data.ok
                if(ok!=0){
                    remote.dialog.showMessageBoxSync({
                        title:'信息',
                        message:res.data.message
                    })
                    this.imgSrc = loginImgPath[1]
                    this.loginState="已登录"
                    db.set('loginState',true).write()
                }else{
                    remote.dialog.showMessageBoxSync({
                        title:'登录失败',
                        message:res.data.message
                    })
                    this.loginState="未登录"
                    this.imgSrc = loginImgPath[2]
                    db.set('loginState',false).write()
                }
            }
        },
        watchKey() {
            //开启按键监控
            let _this = this;
            hotkeys("*", function () {
              let keys = hotkeys.getPressedKeyCodes();
              //console.log(keys); //=> [17, 65] or [70]
              _this.keys = keys;
            });
        },
        cancelWatchKey(){
            //取消按键绑定
            hotkeys.unbind('*')
        },
        save(){
            //save并退出
            let saveConfig = {
                keys:this.keys,
                powerConfig:this.powerConfig,
                showConfig:this.showConfig
            }
            db.set('config',saveConfig).write()
            ipcRenderer.sendSync('closeConfigWindow')
            
        },
        async knowUpDate(){
            let res = request.getUpdate()
            let localVersion = db.get('version').value()
            let onlineVersion = res.version
            if(localVersion!=onlineVersion){
                this.update=1,
                this.updatePlace=res.updatePlace
            }
        }
    },
    created(){
        this.keys = config.keys
        this.powerConfig = config.powerConfig
        this.showConfig = config.showConfig
        this.email = db.get('userEmail').value()
        this.password = db.get('passWord').value()
        let loginState = db.get('loginState').value()
        if(loginState==true){
            this.imgSrc = loginImgPath[1]
            this.loginState = "已登录"
        }
        knowUpDate()
    }
})