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
                        new Claim("family_name", "Support"),
                        new Claim("address", "533, Some Place"),
                        new Claim("role", "admin"),
                        new Claim("country", "uk"),
                        new Claim("subscriptionlevel", "Admin")
                    }
                }
            };
        }

        /// <summary>
        /// Iden Resource map to scopes that give access to iden related claims
        /// Api Resource map to scopes that give access to api resources
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(), // required, ensures user id(subjectid) is included
                new IdentityResources.Profile(), // Returns given and family name - profile related claims
                new IdentityResources.Address(),
                new IdentityResource("roles", "Your Role(s)",
                    new List<string>() { "role" }), // custom identity scope. Name, display name, claims
                new IdentityResource("country", "The country your living in",
                    new List<string>() { "country" }),
                new IdentityResource("subscriptionlevel", "Your subscription level",
                    new List<string>() { "subscriptionlevel" })
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
                },
                new ApiResource("imagegalleryapi", "Image Gallery Api",
                new List<string> { "role" })
                {
                    ApiSecrets = { new Secret("secret".Sha256()) }
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
                    AllowedScopes = { "api1", "profile", "imagegalleryapi", "roles" }
                }
            };
        }
    }
}
