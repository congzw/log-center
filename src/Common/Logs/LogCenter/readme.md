# log center support

## 客户端远程日志的调用关系

调用关系图： 日志系统 -> LogHubReportor(Client) -> LogHub(Server) -> 日志系统 -> ...

为了防止循环调用，采用如下策略：

- LogReportor: 根据日志是否属于远程类别或忽略类别，决定是否报告给LogHub
- LogHub: 接收到日志后，增加远程类标识，然后再交给日志系统


## dependencies for SignalR in aspnetcore3.x project

- Server-Side dependencies：auto included by "Microsoft.AspNetCore.App" package
- Client-Side dependencies: 
	- JavaScript Client: "@microsoft/signalr" via LibMan
	- DotNETClient: "Microsoft.AspNetCore.SignalR.Client" via nuget

## level

建议优先使用appsettings.json中的设置，因为：

- 它位于过滤器管道的前端，它的设置能从源头控制所有，包括代码中的_logger.IsEnabled(logLevel)的逻辑判断
- nlog的位于它之后，且不能影响_logger.IsEnabled(logLevel)的逻辑判断
- 这样不容易理解混乱：NLog默认大部分都使用Trace，这样基本是MicrosoftLogging的级别在起作用

例如：appsettings.json中设置 => "Common.Logs.LogCenter": "Information" 

注意： 

- Hangfire显示配置 config.UseNLogLogProvider()，所以由Nlog的配置管理
- LogHub上控制日志级别的是：ILogger<LogHub>，其唯一名称是:"Common.Logs.LogCenter.Server.LogHub"
- 部署到IIS，日志不出来，确认应用池的权限使用的是LocalSystem！

## refs

[ASP.NET Core SignalR .NET Client](https://docs.microsoft.com/en-us/aspnet/core/signalr/dotnet-client)

## 使用说明

日志查看界面的入口：logMonitor.html

 侦测特定： ClientId: "light-platform"
 侦测所有： ClientId: "*"

界面初始化时，会首先尝试从服务端配置接口去读取，如果没有读取成功，则使用默认。详细逻辑参见logMonitor.js。
