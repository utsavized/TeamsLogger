using System;
using TeamsLogger.Models;

namespace TeamsLogger.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var logger = new TeamsLogger(
                new TeamsWebhookClient("https://outlook.office.com/webhook/71a6c967-860c-4188-828c-e6baba9bc51c@72f988bf-86f1-41af-91ab-2d7cd011db47/IncomingWebhook/55e584c493fa4c1f828c3056521df94d/ec7ccc0b-51c9-4fbc-b2a1-936cee91b855"),
                new LoggerConfiguration { AutomaticallySetColor = true },
                "SomeModule");

            //logger.LogMessage(LogSeverity.Info, "Message to log");
            //logger.LogMessage(LogSeverity.Warn, "Log message", "CCDDC4");
            logger.BeginRunningLog("Begin logging");
            logger.CreateNewMessageCard();
            logger.AddLogToCurrentMessageCard(LogSeverity.Info, "Some event happened");
            logger.AddLogToCurrentMessageCard(LogSeverity.Warn, "Not so good event happened");
            logger.AddLogToCurrentMessageCard(LogSeverity.Error, "Bad event happened");
            string str = null;
            try
            {
                var place = str[3];
            }
            catch (Exception ex)
            {
                logger.CreateNewExceptionMessageCard(ex, "http://microsoft.com");
            }
            
            logger.AddLogToCurrentMessageCard(LogSeverity.Info, "Some event happened");
            logger.AddLogToCurrentMessageCard(LogSeverity.Warn, "Not so good event happened");
            logger.PostRunningLog();
            
        }
    }
}
