# Teams Logger

Rich logging for Microsoft Teams

# Usage
## Basic logger creation

```csharp
var logger = new TeamsLogger.TeamsLogger(
	new TeamsWebhookClient("Your teams channel Uri"),
        new LoggerConfiguration { AutomaticallySetColor = true },
        "Module Name");
logger.LogMessage(LogSeverity.Info, "This is a simple log");
```

## IoC Container

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
