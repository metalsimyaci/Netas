using System;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Newtonsoft.Json.Linq;

namespace Mlient
{
    class Program
    {
        private static TokenResponse _tokenResponse;
        static async Task Main(string[] args)
        {
            await GetTokenRequest();
            await GetApi();
            Console.ReadLine();
        }

        static async Task GetMetaData()
        {
            var client = new HttpClient();
            var disco = await client.GetDiscoveryDocumentAsync("http://localhost:5000");
            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                return;
            }
            else
            {
                Console.WriteLine($"Authorization EndPoint:{disco.AuthorizeEndpoint}");
            }
        }

        static async Task GetTokenRequest()
        {
            var client = new HttpClient();
            var disco = await client.GetDiscoveryDocumentAsync("http://localhost:5000");
            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                return;
            }
            _tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = "client",
                ClientSecret = "511536EF-F270-4058-80CA-1C89C192F69A",
                Scope = "api1"
            });

            if (_tokenResponse.IsError)
            {
                Console.WriteLine(_tokenResponse.Error);
                return;
            }

            Console.WriteLine(_tokenResponse.Json);
        }

        static async Task GetApi()
        {
            var client = new HttpClient();
            client.SetBearerToken(_tokenResponse.AccessToken);

            var response = await client.GetAsync("http://localhost:5001/identity");
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.StatusCode);
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine(JArray.Parse(content));
            }
        }
    }
}
