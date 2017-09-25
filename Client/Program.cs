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
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("API Token Auth Demo");
            Console.WriteLine(@"
                                               
                         .::::::::::.                          .::::::::::.
                       .::::''''''::::.                      .::::''''''::::.
                     .:::'          `::::....          ....::::'          `:::.
                    .::'             `:::::::|        |:::::::'             `::.
                   .::|               |::::::|_ ___ __|::::::|               |::.
                   `--'               |::::::|_()__()_|::::::|               `--'
                    :::               |::-o::|        |::o-::|               :::
                    `::.             .|::::::|        |::::::|.             .::'
                     `:::.          .::\-----'        `-----/::.          .:::'
                       `::::......::::'                      `::::......::::'
                         `::::::::::'                          `::::::::::'
            ");
            Console.ResetColor();
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

            var tokenResponse = await tokenClient.RequestResourceOwnerPasswordAsync("user", "password", "api1 profile imagegalleryapi roles openid address");

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
            }

            Console.WriteLine(tokenResponse.Json);
            Console.WriteLine("\n\n");

            CallApi(tokenResponse);

            RevokeToken(tokenResponse, disco);

            CallApi(tokenResponse);

            Console.ReadLine();
            return string.Empty;
        }

        static void CallApi(TokenResponse response)
        {
            Console.WriteLine("Requesting Userdata from API");
            Console.WriteLine(@"
            Requesting Userdata from API:        ___   _      ___   _      ___   _      ___   _      ___   _
                                                [(_)] |=|    [(_)] |=|    [(_)] |=|    [(_)] |=|    [(_)] |=|
                                                '-`  |_|     '-`  |_|     '-`  |_|     '-`  |_|     '-`  |_|
                                                /mmm/  /     /mmm/  /     /mmm/  /     /mmm/  /     /mmm/  /
                                                    |____________|____________|____________|____________|
                                                                            |            |            |
                                                                        ___  \_      ___  \_      ___  \_
                                                                       [(_)] |=|    [(_)] |=|    [(_)] |=|
                                                                           '-`  |_|     '-`  |_|     '-`  |_|
                                                                       /mmm/        /mmm/        /mmm/

");
            var client = new HttpClient();
            client.SetBearerToken(response.AccessToken);

            Console.WriteLine(client.GetStringAsync("https://localhost:44392/test").Result);

        }

        static async void RevokeToken(TokenResponse response, DiscoveryResponse endpointInfo)
        {
            Console.WriteLine(@"

                Revoking Access     -(_)====y

        ");

            var revocationClient = new TokenRevocationClient(
                endpointInfo.RevocationEndpoint,
                "ro.client",
                "secret");

            var data = await revocationClient.RevokeAccessTokenAsync(response.AccessToken);

            if (data.IsError)
            {
                Console.WriteLine(data.Error);
            }

            Console.WriteLine(data.Json);
            Console.WriteLine("\n\n");
        }
    }
}
