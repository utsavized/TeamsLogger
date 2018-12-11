using Microsoft.VisualStudio.TestTools.UnitTesting;
using TeamsLogger.Models;

namespace TeamsLogger.Tests
{
    [TestClass]
    public class TeamsLoggerTests
    {
        [TestMethod]
        public void CardIsCreated()
        {
            var logger = new Logger(
                new TeamsWebhookClient("http://yourcompany.com/teams/channel/randomId"),
                new LoggerConfiguration { AutomaticallySetColor = true },
                "ModuleName");

            logger.BeginRunningLog("This is title");
            
        }
    }
}
