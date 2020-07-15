# a simple log center

## desc

![system structure][system_structure]

支持服务器端的日志过滤

1 类库中，声明了日志的别名[ProviderAlias("RemoteLogCenter")]
2 客户端，通过appsettings.json中配置节RemoteLogCenter，来控制发往远端的日志输出
3 可以通过RemoteHubReporterConfig.Enabled来控制是否停用

## change list

- 20200219 add projects
- 20200219 init

## structure

- 工具库： LogCenter.Common
- 演示客户端：LogCenter.Client
- 日志服务：LogCenter.Web

- 封装组件：
	- C#: LogCenter.Common.dll (Server  + Client)
	- JS: zonekey.log.logHubHelper.js (Server + Client

## change list

- 20200715 refact codes
- 20200219 add docs
- 20200217 add remote logger proxy, and client demo
- 20200116 init 

[system_structure]: ../raw/master/doc/system_structure.png
