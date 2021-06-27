# 前言
考虑到这已经是这个软件的第三版了，前两版都不再兼容和维护。这次我发誓会继续维护此版本。
此版本的优势：
- 可以完全本地运行或者网络运行。
- 软件小：1mb。
- 软件占用运行内存小：50mb左右


# 简介
快捷键助手是一款帮助您记忆并提示“正在使用的软件”的快捷键的应用。随着我们使用的电脑软件越来越多，CAD、Vistual Studio、OneNote、Word、WPS、QQ、Mathpix、utools、VSCode、Blender、Solidworks、PS、AI等等软件的快捷键混杂在一起很难记忆。
小白在学习新软件的时候也总要花费很长时间来记忆快捷键，加上在我最初入坑MacOS的时候就被上面类似的软件”CheatSheet”深深吸引了。
![图1](https://tuchuang-liuzhengdong.oss-cn-shanghai.aliyuncs.com/023329axpwzobopzp77xxx.png)

![图2](https://tuchuang-liuzhengdong.oss-cn-shanghai.aliyuncs.com/023642xzxttnsr1isx9e2u.png)


所以就本软件就诞生啦。
# 已添加快捷键软件列表
- Windows常用快捷键
- OneNote
- Word
- Edge
- 知乎网页版
- Adobe PhotoShop
- CAD
- Adobe illustrator
- 3dsMax
- SketchUp

# 安装说明
需要预先安装windowsdesktop-runtime-5.0.7-win-x64.exe
在发行版中即可找到。
再使用本软件

# 使用方式
将此文件夹下的所有软件放在你喜欢的位置，比如Program Files/ShortCutHelper等文件夹下，然后双击启动即可。
软件默认设置开机启动，可以用Windows自带的开机管理器管理，之后使用Alt+S快捷键来关闭和呼出，软件支持夜间模式。


# 如何深度使用
用户通过自己的JSON文件数据或者订阅网络上的JSON文件数据，相似的操作有“阅读APP”和“Clash订阅”，与它们类似，你可以订阅网络的JSON文件。当然，你也可以将订阅地址删除后直接使用本地文件。本软件没有识别不同软件快捷的黑科技，所有快捷键数据都需要人工填写，不过，默认的订阅地址已存在了市面上比较常用软件的快捷键。

所有设置文件都会存放在：C:\Users\{user}\AppData\Local\ShortCutHelper 目录中，目录内有subscription.json以及config.json两个主要文件。
Subscription用来存放快捷键的数据，Config.json用来存放软件本身的数据。
你如果想自己定制自己的快捷键也可以将订阅地址删除，然后自己在subscription.json文件中添加数据。
里面的几个参数自己推测一下哈哈，后续会写在GitHub上，软件的支持列表同样也会在GitHub上更新，同时欢迎各位来添加快捷键。

默认订阅地址为了保证国内的快速访问（gitee）：

https://gitee.com/liuzhengdong666/ShortCutHelper/raw/master/subscription.json

开源地址（优先更新）：

https://github.com/liuzhengdong2/ShortCutHelper

# 如何删除本软件
本软件不包含任何注册表操作，直接删除文件即可。

# 已知缺陷
- 启用本软件的快捷键（Alt+S）暂时还不可更改。
- 要求.Net 5 Core运行环境，Windows应该会自动下载。
- 内存占用为50MB以下，刚启动在70MB左右，目测有优化空间。
- 网络模式会自动覆盖自定义文件，所以如果自定义了请及时备份。
- 声明一下，我想把“快捷键助手”做成一个系统插件形式的软件，所以在系统托盘里目前不计划有图标显示（除非你们都在骂）。

# 安装说明：
本软件要求.NET 5的运行环境，刚打开软件的时候系统会自动提示安装，同时在发布地址也有运行环境的安装包。

下载地址：https://gitee.com/liuzhengdong666/ShortCutHelper/releases