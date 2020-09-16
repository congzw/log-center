# default log file folder

## nlog.config

``` xml
<nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd"
      xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
      autoReload="true"
      internalLogLevel="Info"
      internalLogFile="wwwroot/logs/internal-nlog.txt">
  <variable name="logDirectory" value="wwwroot/logs/"/>
  ...
```

## gitignore

``` gitignore

# wwwroot/logs
**/wwwroot/logs/*.txt

```