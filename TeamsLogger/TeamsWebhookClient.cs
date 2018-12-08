using System.Threading;
using System.Threading.Tasks;
using RestSharp;

namespace TeamsLogger
{
    public interface ITeamsWebhookClient
    {
        void Post(string jsonPayload);
        Task PostAsync(string jsonPayload);
    }

    public class TeamsWebhookClient : ITeamsWebhookClient
    {
        private readonly string _restUri;

        public TeamsWebhookClient(string restUri)
        {
            _restUri = restUri;
        }

        public void Post(string jsonPayload)
        {
            var client = new RestClient(_restUri);
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Accept", "application/json");
            request.AddJsonBody(jsonPayload);
            client.Execute(request);

        }

        public async Task PostAsync(string jsonPayload)
        {
            var client = new RestClient(_restUri);
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Accept", "application/json");
            request.AddJsonBody(jsonPayload);
            await client.ExecutePostTaskAsync(request, new CancellationTokenSource().Token);
        }
    }
}
