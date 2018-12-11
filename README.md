# Teams Logger

Rich logging for Microsoft Teams using HTTP POST to Teams Incoming Webhook Connector

# Usage
## Basic logger creation

```csharp
var logger = new TeamsLogger.TeamsLogger(
	new TeamsWebhookClient("Your teams channel Uri"),
        new LoggerConfiguration { AutomaticallySetColor = true },
        "Module Name");
logger.LogMessage(LogSeverity.Info, "This is a simple log");
```

## IoC Container logger registration

```csharp
// This example uses Autofac
builder.Register(ctx =>
{
    var loggingUri = ConfigurationManager.AppSettings["TeamsLoggingUri"]; // Your teams channel uri
    var webhookClient = new TeamsWebhookClient(loggingUri);
    return new TeamsLogger.TeamsLogger(
	webhookClient,
        new LoggerConfiguration { AutomaticallySetColor = true },
        "Module Name");
}).AsSelf().SingleInstance();

logger.LogMessage(LogSeverity.Info, "This is a simple log");
```

## Configuration

There is only one setting `new LoggerConfiguration { AutomaticallySetColor = true }` which dictates if message cards are automcatically colored based on severity of the message logged. User can always override this setting by supplying their own hex code.

# Logging Concepts

## Simple Logs

These create simple log messages and post to Teams right away. Each log message represents an individual message card.

```csharp
// Color will set the color of the message card
public void LogMessage(LogSeverity severity, string message, string color = null);
public Task LogMessageAsync(LogSeverity severity, string message, string color = null);
```

![Simple Log](https://github.com/utsavized/TeamsLogger/blob/7d0af9e674e01bd155b9cff8e8e48b76c1f04263/docs/simplelog.PNG)

## Running Logs

Running logs help create rich log cards by aggregating multiple logs serially within your app, and eventually logging them all at once as a single Teams message card. Note: This only works if your app isn't distributing its logging. For distributed case, use simple logging.

For example:

```csharp
var logger = new TeamsLogger.TeamsLogger(
	new TeamsWebhookClient("Your teams channel Uri"),
        new LoggerConfiguration { AutomaticallySetColor = true },
        "SomeModule");
	
// App begins running log aggregation
logger.BeginRunningLog("Begin logging");

// Logs app events
logger.AddLogToCurrentMessageCard(LogSeverity.Info, "Some event happened");
logger.AddLogToCurrentMessageCard(LogSeverity.Warn, "Not so good event happened");
logger.AddLogToCurrentMessageCard(LogSeverity.Error, "Bad event happened");

// some exception occurred, and was caught here
// Full log was uploaded to some url
logger.CreateNewExceptionMessageCard(e, "log url here", "Log");

// App continues
logger.AddLogToCurrentMessageCard(LogSeverity.Info, "Some event happened");
logger.AddLogToCurrentMessageCard(LogSeverity.Warn, "Not so good event happened");

_logger.PostRunningLog(); // Log is posted here, can use async
```

![Simple Log](https://github.com/utsavized/TeamsLogger/blob/7d0af9e674e01bd155b9cff8e8e48b76c1f04263/docs/runninglog.PNG)



