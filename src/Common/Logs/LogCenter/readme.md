# log center support

## �ͻ���Զ����־�ĵ��ù�ϵ

���ù�ϵͼ�� ��־ϵͳ -> LogHubReportor(Client) -> LogHub(Server) -> ��־ϵͳ -> ...

Ϊ�˷�ֹѭ�����ã��������²��ԣ�

- LogReportor: ������־�Ƿ�����Զ�����������𣬾����Ƿ񱨸��LogHub
- LogHub: ���յ���־������Զ�����ʶ��Ȼ���ٽ�����־ϵͳ


## dependencies for SignalR in aspnetcore3.x project

- Server-Side dependencies��auto included by "Microsoft.AspNetCore.App" package
- Client-Side dependencies: 
	- JavaScript Client: "@microsoft/signalr" via LibMan
	- DotNETClient: "Microsoft.AspNetCore.SignalR.Client" via nuget

## level

��������ʹ��appsettings.json�е����ã���Ϊ��

- ��λ�ڹ������ܵ���ǰ�ˣ����������ܴ�Դͷ�������У����������е�_logger.IsEnabled(logLevel)���߼��ж�
- nlog��λ����֮���Ҳ���Ӱ��_logger.IsEnabled(logLevel)���߼��ж�
- ���������������ң�NLogĬ�ϴ󲿷ֶ�ʹ��Trace������������MicrosoftLogging�ļ�����������

���磺appsettings.json������ => "Common.Logs.LogCenter": "Information" 

ע�⣺ 

- Hangfire��ʾ���� config.UseNLogLogProvider()��������Nlog�����ù���
- LogHub�Ͽ�����־������ǣ�ILogger<LogHub>����Ψһ������:"Common.Logs.LogCenter.Server.LogHub"
- ����IIS����־��������ȷ��Ӧ�óص�Ȩ��ʹ�õ���LocalSystem��

## refs

[ASP.NET Core SignalR .NET Client](https://docs.microsoft.com/en-us/aspnet/core/signalr/dotnet-client)

## ʹ��˵��

��־�鿴�������ڣ�logMonitor.html

 ����ض��� ClientId: "light-platform"
 ������У� ClientId: "*"

�����ʼ��ʱ�������ȳ��Դӷ�������ýӿ�ȥ��ȡ�����û�ж�ȡ�ɹ�����ʹ��Ĭ�ϡ���ϸ�߼��μ�logMonitor.js��
