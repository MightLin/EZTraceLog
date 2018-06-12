Sample

```c#
// set the log file path and file Name
TraceLog.SetLogFile("D:\AppData\Log"), "TestLogConsole");

TraceLog.InfoWrite("main", "Start Process");

TraceLog.DebugWrite("methodName", "This is debug log");

TraceLog.WarnWrite("className", "This is warn log");

TraceLog.ErrorWrite("canTypeAnything", "This is error log");

TraceLog.InfoWrite("main", "End Process");
```

Result

D:\AppData\Log\TestLogConsole.2018-06-11.log

```txt
17:22:45.670 [main      ][INFO ] Start Process
17:22:45.671 [methodName][DEBUG] This is debug log
17:22:45.671 [className ][WARN ] This is warn log
17:22:45.671 [canTypeAnything][ERROR] This is error log
17:22:45.671 [main      ][INFO ] End Process
```


# You can send error message  when *ErrorWrite*

```c#
public interface IMsgSender
{
	bool Send(string title, string message);
}
```

```C#
public void main()
{
	TraceLog.ErrMsgSender = mySmtpSender;
}

```
