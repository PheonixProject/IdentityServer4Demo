using IdentityModel.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        private static string authority = "http://localhost:52590/";
        static void Main(string[] args)
        {
            GetTokenAsync2().GetAwaiter().GetResult();
        }

        private static async Task<string> GetTokenAsync2()
        {
            var discos = new DiscoveryClient(authority);
            var disco = await discos.GetAsync();

            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                return null;
            }

            var tokenClient = new TokenClient(disco.TokenEndpoint, "ro.client", "secret");

            var tokenResponse = await tokenClient.RequestResourceOwnerPasswordAsync("user", "password", "api1 profile imagegalleryapi roles");

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
            }

            Console.WriteLine(tokenResponse.Json);
            Console.WriteLine("\n\n");

            CallApi(tokenResponse);

            Console.ReadLine();
            return string.Empty;
        }

        static void CallApi(TokenResponse response)
        {
            var client = new HttpClient();
            client.SetBearerToken(response.AccessToken);

            Console.WriteLine(client.GetStringAsync("https://localhost:44392/test").Result);
        }
    }
}
