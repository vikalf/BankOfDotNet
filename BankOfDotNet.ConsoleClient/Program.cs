using IdentityModel.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BankOfDotNet.ConsoleClient
{
    static class Program
    {
        public static void Main(string[] args) => MainAsync().GetAwaiter().GetResult();

        private static async Task MainAsync()
        {
            var client = new HttpClient();
            
            var tokenResponse = await client.RequestTokenAsync(new TokenRequest
            {
                Address = "http://localhost:5000/connect/token",
                ClientId = "client",
                ClientSecret = "secret",
                Parameters =
                {
                    { "grant_type","client_credentials"},
                    {"scope","bankOfDotNetApi"},
                }
            });

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return;
            }

            Console.WriteLine(tokenResponse.Json);
            Console.WriteLine("\n\n");

            // consume our customer API
            var apiClient = new HttpClient();
            apiClient.SetBearerToken(tokenResponse.AccessToken);

            var customerInfo = new StringContent(
                JsonConvert.SerializeObject(
                    new {
                        Id = 4,
                        FirstName = "Rosalba",
                        LastName = "Martinez_New"
                    }), Encoding.UTF8, "application/json");

            var baseApiUrl = "http://localhost:5001/api/customers";
            var createCustomerResponse = await apiClient.PostAsync(baseApiUrl, customerInfo);

            if (!createCustomerResponse.IsSuccessStatusCode)
            {
                Console.WriteLine("Create Customer Response StatusCode: {0}", createCustomerResponse.StatusCode);
            }

            var getCustomerResponse = await apiClient.GetAsync(baseApiUrl);

            if (!getCustomerResponse.IsSuccessStatusCode)
            {
                Console.WriteLine("Get Customer Response StatusCode: {0}", getCustomerResponse.StatusCode);
            }
            else
            {
                var content = await getCustomerResponse.Content.ReadAsStringAsync();
                Console.WriteLine(JArray.Parse(content));
            }





        }
    }
}
