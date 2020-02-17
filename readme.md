# a simple log center

## desc

支持服务器端的日志过滤

1 类库中（LogCenter.Proxy），声明了日志的别名[ProviderAlias("RemoteLogCenter")]
2 客户端（LogCenter.Client），通过appsettings.json中配置节RemoteLogCenter，来控制远端的日志输出
3 也可以通过RemoteHubReporter.Enabled来控制是否停用

## change list

- 20200217 add remote logger proxy, and client demo
- 20200116 init 