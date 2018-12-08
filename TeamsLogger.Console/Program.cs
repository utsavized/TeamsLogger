using System;
using TeamsLogger.Models;

namespace TeamsLogger.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var logger = new TeamsLogger(
                new TeamsWebhookClient("http://yourcompany.com/teams/channel/randomId"),
                new LoggerConfiguration { AutomaticallySetColor = true },
                "");

            logger.LogMessage(LogSeverity.Info, "Message to log");
            logger.LogMessage(LogSeverity.Warn, "Log message", "CCDDC4");
            logger.BeginRunningLog("Begin logging");
            logger.AddNewSection();
            logger.AddSubSectionEvent(LogSeverity.Info, "Some event happened");
            logger.AddSubSectionEvent(LogSeverity.Warn, "Not so good event happened");
            logger.AddSubSectionEvent(LogSeverity.Error, "Bad event happened");
            var ex = new Exception("Bad stuff");
            logger.AddNewSection(LogSeverity.Error, typeof(Exception).ToString(), ex.Message, $"```{ex.StackTrace}```", null, null, true);
            logger.AddSubSectionEvent(LogSeverity.Info, "Some event happened");
            logger.PostRunningLog();
        }
    }
}
