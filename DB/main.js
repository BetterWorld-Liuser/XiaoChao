const low = require('lowdb')
const FileSync = require('lowdb/adapters/FileSync')
const { remote, app } = require('electron');
const path = require('path');
const fs = require('fs-extra'); 
//const os = require('os');
//const Memory = require('lowdb/adapters/Memory');
//引入模块


//定义变量
const APP = process.type === 'renderer' ? remote.app : app; 
const adapter = new FileSync(path.join(__dirname, 'datastore.json')) // 初始化lowdb读写的json文件名以及存储路径
const db = low(adapter)
 



// Set some defaults
db.defaults({ 
    userEmail:"",
    passWord:"",
    loginState:false,
    config:{
      keys: [18,83],
      powerConfig:true,
      showConfig:true,
    },
    version:"1.0.0"
 }).write()
  
console.log('初始化成功')


module.exports = db

  

/* // Add a post
db.get('posts')
  .push({ id: 1, title: 'lowdb is awesome'})
  .write()
 
// Set a user using Lodash shorthand syntax
db.set('user.name', 'typicode')
  .write() */