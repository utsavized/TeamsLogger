using System.Net;
using System.Text;
using System.Threading.Tasks;

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
            var request = GetRequest(jsonPayload);
            var response = request.GetResponse();
        }

        public async Task PostAsync(string jsonPayload)
        {
            var request = GetRequest(jsonPayload);
            await request.GetResponseAsync();
        }

        private HttpWebRequest GetRequest(string jsonPayload)
        {
            var request = (HttpWebRequest)WebRequest.Create(_restUri);
            request.Accept = "application/json";
            request.ContentType = "application/json";
            request.Method = "POST";

            var encoding = new ASCIIEncoding();
            var bytes = encoding.GetBytes(jsonPayload);

            using (var newStream = request.GetRequestStream())
            {
                newStream.Write(bytes, 0, bytes.Length);
            }

            return request;
        }
    }
}
