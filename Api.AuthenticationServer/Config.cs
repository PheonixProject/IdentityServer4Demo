using IdentityServer4.Models;
using IdentityServer4.Test;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace AuthServer
{
    public static class Config
    {
        // Valid Users & Their claims
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
                        new Claim("role", "newstarter"),
                        new Claim("role", "wizards"),
                        new Claim("country", "uk"),
                        new Claim("subscriptionlevel", "Admin")
                    }
                }
            };
        }

        /// <summary>
        /// Identity Resource map to scopes that give access to iden related claims
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

        // Clients want to access resources (aka scopes)
        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = "ro.client",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,

                    // Prevents redirection to consent page on login
                    RequireConsent = false,

                    //Ref token or Self Containted token (JWT)
                    // Ref can be revoked instantly as it is validated with every call

                    AccessTokenType = AccessTokenType.Reference,

                    // Default 5 min
                    AuthorizationCodeLifetime = 120,

                    // Sliding means new token being requested updated lifetime
                    // Wont exceeed absolute lifetime.
                    //RefreshTokenExpiration = TokenExpiration.Sliding,

                    // Claims will be updated when token refreshed
                    UpdateAccessTokenClaimsOnRefresh = true,

                    //Allows tokens to be refreshed 
                    AllowOfflineAccess = true,

                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes = { "api1", "profile", "imagegalleryapi", "roles", "openid", "address" }
                }
            };
        }
    }
}
