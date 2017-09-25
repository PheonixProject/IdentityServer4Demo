using IdentityServer4.Models;
using IdentityServer4.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthServer
{
    public static class Config
    {
        public static List<TestUser> GetUsers()
        {
            return new List<TestUser>
            {
                new TestUser
                {
                    SubjectId = Guid.NewGuid().ToString(), //Unique Identifier
                    Username = "user",
                    Password = "password",
                    Claims = new List<Claim> // Info on user (claims are related to scopes)
                    {
                        new Claim("given_name", "Cascade"),
                        new Claim("family_name", "Support")
                    }
                }
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("api1", "My API")
                {
                    Scopes = new List<Scope>()
                    {
                        new Scope()
                        {
                            Name = "api1",
                            UserClaims = new List<string>() { "Email Address" }
                        },
                        new Scope()
                        {
                            Name = "api1.write",
                            UserClaims = new List<string>() { "Email Address" }
                        },
                        new Scope()
                        {
                            Name = "api1.full_access",
                            UserClaims = new List<string>() { "Email Address" }
                        }
                    }
                }
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = "ro.client",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,

                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes = { "api1" }
                }
            };
        }
    }
}
