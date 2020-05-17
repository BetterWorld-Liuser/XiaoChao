let axios = require('axios')

let app = new axios.create({
    //baseUrl:'http://shortCut.liuzhengdong.top:8361',
    baseURL:'http://127.0.0.1:3012',
    timeout:7000,
    headers: {
        "Content-Type":"application/json",
        "Access-Control-Allow-Origin":"*"
    }
})

//使用token
/* app.interceptors.request.use(config => {
    const token = localStorage.getItem('token');
     console.log(token) 
    config.headers.Authorization = `Bearer ${token}`;
    return config;
})
*/





//获取当前窗口的公有数据
let getCommonData =async function(appName){
    let res = await app.get('/getCommonData',{
        params:{
            appName,
        }
    })
    console.log(res)
    return res
}

//同步全部公有数据存入数据库
let syncCommonData = function(appName,appData){
    let res = app.post('/syncCommonData',{
        appName,
        appData
    })
    return res
}


//登陆
let login = function(email,password){
    let res = app.get('users/login',{
        params:{
            email,
        password
        }
    })
    return res
}

//获取当前窗口的私有数据
let getPersonalData =async function(appName,email,password){
    let res = app.get('users/req/getPersonalData',{
        params:{
            appName,
            email,
            password
        }
    })
    return res
}



//同步全部私有数据存入数据库
let syncPersonalData =async function(email,password,appName,appData){
    let res = app.post('users/req/syncPersonalData',{
        email,password,
        appName,
        appData
    })
    return res
}

let getUpdate = async function(){
    let res = await app.get('/update')
    return res.data
}

//获取个人信息





let request = {
    getUpdate,
    syncPersonalData,
    syncCommonData,
    getPersonalData,
    getCommonData,
    login
}

module.exports = request
