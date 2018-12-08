using System;
using System.Threading.Tasks;

namespace TeamsLogger.Tests.Mocks
{
    public class TeamsWebhookClientMock : ITeamsWebhookClient
    {
        public void Post(string jsonPayload)
        {
        }

        public Task PostAsync(string jsonPayload)
        {
            throw new NotImplementedException();
        }
    }
}
