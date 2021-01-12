# a simple log center

## desc

![system structure][system_structure]

支持服务器端的日志过滤

- 1 类库中，声明了日志的别名[ProviderAlias("RemoteLogCenter")]
- 2 客户端，通过appsettings.json中配置节RemoteLogCenter，来控制发往远端的日志输出
- 3 可以通过RemoteHubReporterConfig.Enabled来控制是否停用

## structure

- 工具库： LogCenter.Common
- 演示客户端：LogCenter.Client
- 日志服务：LogCenter.Web

- 封装组件：
	- C#: LogCenter.Common.dll (Server  + Client)
	- JS: common.log.logHubHelper.js (Server + Client)

- Logger类型
	- Client端发送日志，使用的Logger类型是："LogCenter.Client.RemoteLogger", 原始的日志类型，存储在其Category属性中
	- Server端接受Hub日志，使用的Logger的类型是："LogCenter.Server.NetCoreLogHelper", 原始的日志类型，体现在message的描述中 => logHelper.Log(args.Category + " " + args.Message, args.Level)
 	
## Client端的LogLevel设置：appsettings.json 

通过调整Client端的appsettings.json中的RemoteLogCenter设置，可以控制发往远端的日志级别
例如"Microsoft": "Information" => "Microsoft": "Error"，可大大降低发送到远端的日志数量

``` json
{
    "RemoteLogCenter": {
      "LogLevel": {
        "Default": "Information",
        "LogCenter": "Warning",
        "System": "Error",
        "Microsoft": "Error"
      }
    }
}
```

## Server端的LogLevel设置：nlog.json 

``` xml
  <rules>
    <!--All logs, including from Microsoft-->
    <logger name="*" minlevel="Trace" writeTo="all" />
    <!--Skip non-critical Microsoft logs and so log only own logs-->
    <logger name="Microsoft.*" maxlevel="Info" final="true" />
    <!-- BlackHole without writeTo -->
    <logger name="LogCenter.*" minlevel="Debug" writeTo="application" />
  </rules>
```

## multi client log support

Client端设置：appsettings.json 

``` json
  "RemoteHubReporter": {
    "ClientId" : "[DemoClient]", 
    "Enabled": true,
    "HubUri": "ws://192.168.1.182:8040/hubs/logHub",
    "MaxTryCount": 3
  },
```
- RemoteLoggerProvider.ClientId => RemoteHubReporter.ClientId
- RemoteLogger.Log()
- LogHub.CallServerLog()


## change list

- 20200916 add multi clients log support
- 20200916 change logs directory, add logs browser support; add build scripts
- 20200715 refact codes
- 20200219 add docs
- 20200217 add remote logger proxy, and client demo
- 20200116 init 

[system_structure]: doc/system_structure.png

## 工具日志和远端日志

工具日志和远端日志两部分设计，完全不相关，注意避免混淆

- LogHelper    
    - ILogHelper <- LogHelper
    - ILogHelper <- LogHelperAdapter(Logger<LogHelper> logger)
- RemoteLoggerProvider
    - ILoggerProvider <- RemoteLoggerProvider

## todo

- log list view
- download log in log list view
- view log in log list view